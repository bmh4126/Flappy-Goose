module FlappyGoose.GameLogic

open FlappyGoose.GameTypes
open FlappyGoose.Constants
open FlappyGoose.Collision

let private rng = System.Random()

let chooseRandomGravity () =
    let range = float (maxRandomGravity - minRandomGravity)
    minRandomGravity + float32 (rng.NextDouble() * range)

let private clamp (lo: float32) (hi: float32) (v: float32) =
    if v < lo then lo elif v > hi then hi else v

let private nextGapY (prevGapY: float32) =
    let shift = float32 (rng.NextDouble() * 2.0 - 1.0) * maxGapShift
    clamp minGapY maxGapY (prevGapY + shift)

let private spawnObstacle (prevGapY: float32) (x: float32) =
    let gapY = nextGapY prevGapY
    { X = x; Width = obstacleWidth; GapY = gapY; GapHeight = gapHeight; Passed = false }

let private initialObstacles () =
    let startX = float32 screenWidth + obstacleSpacing
    let firstGapY = (minGapY + maxGapY) / 2.0f
    [ for i in 0 .. obstaclesPerStage - 1 ->
        spawnObstacle firstGapY (startX + float32 i * obstacleSpacing) ]

let createInitialState () =
    { Goose = { X = gooseStartX; Y = gooseStartY; Velocity = 0.0f }
      Obstacles = []
      Score = 0
      HighScore = 0
      Stage = 1
      Gravity = normalGravity
      Speed = obstacleSpeed
      Screen = Home
      TransitionTime = 0.0f
      NextGravity = normalGravity
      ObstaclesPassedInStage = 0
      LastGapY = (minGapY + maxGapY) / 2.0f }

let startNewGame (state: GameState) =
    let obs = initialObstacles ()
    let midGap = obs |> List.head |> fun o -> o.GapY
    { state with
        Goose = { X = gooseStartX; Y = gooseStartY; Velocity = 0.0f }
        Obstacles = obs
        Score = 0
        Stage = 1
        Gravity = normalGravity
        Speed = obstacleSpeed
        Screen = Playing
        TransitionTime = 0.0f
        ObstaclesPassedInStage = 0
        LastGapY = midGap }

let returnHome (state: GameState) =
    { state with Screen = Home }

let flapGoose (state: GameState) =
    { state with Goose = { state.Goose with Velocity = flapVelocity } }

let updateGoose (dt: float32) (state: GameState) =
    let g = state.Goose
    let newVel = g.Velocity + state.Gravity * dt
    let newY   = g.Y + newVel * dt
    { state with Goose = { g with Velocity = newVel; Y = newY } }

let moveObstacles (dt: float32) (state: GameState) =
    let moved =
        state.Obstacles
        |> List.map (fun o -> { o with X = o.X - state.Speed * dt })
        |> List.filter (fun o -> o.X + o.Width > -10.0f)
    { state with Obstacles = moved }

let updateScore (state: GameState) =
    let gooseRight = state.Goose.X + gooseWidth
    let updated =
        state.Obstacles
        |> List.map (fun o ->
            if not o.Passed && gooseRight > o.X + o.Width then
                { o with Passed = true }
            else o)
    let newPassed = updated |> List.filter (fun o -> o.Passed) |> List.length
    let oldPassed = state.Obstacles |> List.filter (fun o -> o.Passed) |> List.length
    let gained = newPassed - oldPassed
    { state with
        Obstacles = updated
        Score = state.Score + gained
        ObstaclesPassedInStage = state.ObstaclesPassedInStage + gained }

let startTransition (state: GameState) =
    let nextGrav = chooseRandomGravity ()
    { state with
        Screen = Transition
        TransitionTime = transitionDuration
        NextGravity = nextGrav
        Obstacles = [] }

let updateTransition (dt: float32) (state: GameState) =
    let remaining = state.TransitionTime - dt
    if remaining <= 0.0f then
        let obs = initialObstacles ()
        { state with
            Screen = Playing
            Stage = state.Stage + 1
            Gravity = state.NextGravity
            Speed = obstacleSpeed + float32 state.Stage * speedIncreasePerStage
            Obstacles = obs
            ObstaclesPassedInStage = 0
            TransitionTime = 0.0f }
    else
        { state with TransitionTime = remaining }

let checkGameOver (state: GameState) =
    let dead =
        isOutOfBounds state.Goose ||
        collidesWithAnyObstacle state.Goose state.Obstacles
    if dead then
        let newHigh = max state.Score state.HighScore
        let screen  = if state.Score > state.HighScore then NewHighScore else GameOver
        { state with Screen = screen; HighScore = newHigh }
    else
        state

let restartAfterGameOver (state: GameState) =
    startNewGame state

let updatePlaying (dt: float32) (spacePressed: bool) (state: GameState) =
    let s0 = if spacePressed then flapGoose state else state
    let s1 = updateGoose dt s0
    let s2 = moveObstacles dt s1
    let s3 = updateScore s2
    if s3.ObstaclesPassedInStage >= obstaclesPerStage then
        startTransition s3
    else
        checkGameOver s3

let updateGame (dt: float32) (enterPressed: bool) (spacePressed: bool) (rPressed: bool) (hPressed: bool) (state: GameState) =
    match state.Screen with
    | Home ->
        if enterPressed then startNewGame state else state
    | Playing ->
        updatePlaying dt spacePressed state
    | Transition ->
        updateTransition dt state
    | GameOver | NewHighScore ->
        if rPressed then restartAfterGameOver state
        elif hPressed then returnHome state
        else state
