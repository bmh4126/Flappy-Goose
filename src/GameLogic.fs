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

let obstaclesForStage (stage: int) = min 10 (4 + stage)
// stage 1 -> 5, stage 2 -> 6, ..., stage 6+ -> 10

// Physics-constrained, directionally-biased gap center selection.
// stageIndex = stage - 1 (0-based) controls desired shift magnitude.
let private nextGapYConstrained (prevGapY: float32) (gravity: float32) (speed: float32) (stageIndex: int) =
    let t = obstacleSpacing / speed
    let birdMargin = gooseHeight / 2.0f
    let safeHalfGap = gapHeight / 2.0f - birdMargin
    let prevSafeTop    = prevGapY - safeHalfGap
    let prevSafeBottom = prevGapY + safeHalfGap

    let trajectory (y0: float32) (v0: float32) (flapDelay: float32 option) =
        match flapDelay with
        | None ->
            y0 + v0 * t + 0.5f * gravity * t * t
        | Some d when d <= 0.0f ->
            y0 + (v0 + flapVelocity) * t + 0.5f * gravity * t * t
        | Some d ->
            let y1 = y0 + v0 * d + 0.5f * gravity * d * d
            let v1 = v0 + gravity * d + flapVelocity
            let r  = t - d
            y1 + v1 * r + 0.5f * gravity * r * r

    let delays = [0.0f; 0.15f; 0.3f; 0.5f; 0.7f; 0.9f]
    let samples =
        [ yield trajectory prevSafeTop    0.0f None
          yield trajectory prevSafeBottom 0.0f None
          for d in delays do
              if d < t then
                  yield trajectory prevSafeTop    0.0f (Some d)
                  yield trajectory prevSafeBottom 0.0f (Some d) ]

    let reachMin = List.min samples
    let reachMax = List.max samples

    // Legal, physically-reachable interval for the next gap center
    let validLo = clamp minGapY maxGapY (reachMin - safeHalfGap)
    let validHi = clamp minGapY maxGapY (reachMax + safeHalfGap)

    if validHi <= validLo then
        clamp minGapY maxGapY prevGapY
    else
        // Stage-based desired shift magnitude (directional, not "stay near previous")
        let desiredShift = clamp baseGapShift maxGapShiftCap (baseGapShift + float32 stageIndex * gapShiftPerStage)

        // Randomly pick up or down
        let direction = if rng.NextDouble() < 0.5 then -1.0f else 1.0f
        let target = prevGapY + direction * desiredShift

        // Clamp target into the valid interval; if direction is blocked, use the far end
        let clampedTarget = clamp validLo validHi target

        // Add small random noise (±20% of desiredShift) around the clamped target
        let noise = float32 (rng.NextDouble() - 0.5) * desiredShift * 0.4f
        clamp validLo validHi (clampedTarget + noise)

let private generateObstacles (count: int) (gravity: float32) (speed: float32) (stageIndex: int) =
    let startX = float32 screenWidth + obstacleSpacing
    let firstGapY = (minGapY + maxGapY) / 2.0f
    let mutable prevGapY = firstGapY
    [ for i in 0 .. count - 1 do
        let gapY =
            if i = 0 then firstGapY
            else nextGapYConstrained prevGapY gravity speed stageIndex
        prevGapY <- gapY
        yield { X = startX + float32 i * obstacleSpacing
                Width = obstacleWidth
                GapY = gapY
                GapHeight = gapHeight
                Passed = false } ]

let createInitialState () =
    { Goose = { X = gooseStartX; Y = gooseStartY; Velocity = 0.0f }
      Obstacles = []
      Score = 0
      HighScore = 0
      PreviousHighScore = 0
      Stage = 1
      Gravity = normalGravity
      Speed = obstacleSpeed
      Screen = Home
      TransitionTime = 0.0f
      NextGravity = normalGravity
      ObstaclesPassedInStage = 0
      LastGapY = (minGapY + maxGapY) / 2.0f
      FlapTimer = 0.0f
      DyingTimer = 0.0f
      ResultScreen = GameOver }

let startNewGame (state: GameState) =
    let obs = generateObstacles (obstaclesForStage 1) normalGravity obstacleSpeed 0
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
        LastGapY = (minGapY + maxGapY) / 2.0f
        FlapTimer = 0.0f
        DyingTimer = 0.0f
        ResultScreen = GameOver
        PreviousHighScore = state.HighScore }  // snapshot current high score for mid-run comparison

let returnHome (state: GameState) =
    { state with Screen = Home }

let flapGoose (state: GameState) =
    { state with
        Goose = { state.Goose with Velocity = flapVelocity }
        FlapTimer = 0.15f }

let updateGoose (dt: float32) (state: GameState) =
    let g = state.Goose
    let newVel = g.Velocity + state.Gravity * dt
    let newY   = g.Y + newVel * dt
    let newFlapTimer = max 0.0f (state.FlapTimer - dt)
    { state with
        Goose = { g with Velocity = newVel; Y = newY }
        FlapTimer = newFlapTimer }

let private clampGooseToBounds (g: Goose) =
    let minY = 0.0f
    let maxY = float32 screenHeight - gooseHeight
    if g.Y < minY then
        { g with Y = minY; Velocity = max 0.0f g.Velocity }
    elif g.Y > maxY then
        { g with Y = maxY; Velocity = min 0.0f g.Velocity }
    else g

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
        Gravity = nextGrav
        NextGravity = nextGrav
        Obstacles = [] }

let updateTransition (dt: float32) (spacePressed: bool) (state: GameState) =
    let s0 = if spacePressed then flapGoose state else state
    let s1 = updateGoose dt s0
    let s2 = { s1 with Goose = clampGooseToBounds s1.Goose }
    let remaining = state.TransitionTime - dt
    if remaining <= 0.0f then
        let nextStage = state.Stage + 1
        let newSpeed  = obstacleSpeed + float32 state.Stage * speedIncreasePerStage
        let obs = generateObstacles (obstaclesForStage nextStage) state.Gravity newSpeed (nextStage - 1)
        { s2 with
            Screen = Playing
            Stage = nextStage
            Speed = newSpeed
            Obstacles = obs
            ObstaclesPassedInStage = 0
            TransitionTime = 0.0f }
    else
        { s2 with TransitionTime = remaining }

// Determine result screen using PreviousHighScore (snapshot at run start).
let startDying (state: GameState) =
    let newHigh  = max state.Score state.HighScore
    let resultSc = if state.Score > state.PreviousHighScore then NewHighScore else GameOver
    { state with
        Screen       = Dying
        HighScore    = newHigh
        ResultScreen = resultSc
        Goose        = { state.Goose with Velocity = deathInitialVelocity }
        DyingTimer   = 0.0f }

let updateDying (dt: float32) (state: GameState) =
    let g = state.Goose
    let newVel   = g.Velocity + deathGravity * dt
    let newY     = g.Y + newVel * dt
    let newTimer = state.DyingTimer + dt
    let state'   = { state with Goose = { g with Velocity = newVel; Y = newY }; DyingTimer = newTimer }
    if newY > float32 screenHeight + 60.0f || newTimer > deathAnimationMaxTime then
        { state' with Screen = state.ResultScreen }
    else
        state'

let checkCollision (state: GameState) =
    let dead =
        isOutOfBounds state.Goose ||
        collidesWithAnyObstacle state.Goose state.Obstacles
    if dead then startDying state else state

let updatePlaying (dt: float32) (spacePressed: bool) (state: GameState) =
    let s0 = if spacePressed then flapGoose state else state
    let s1 = updateGoose dt s0
    let s2 = moveObstacles dt s1
    let s3 = updateScore s2
    let limit = obstaclesForStage state.Stage
    if s3.ObstaclesPassedInStage >= limit then
        startTransition s3
    else
        checkCollision s3

let updateGame (dt: float32) (spacePressed: bool) (escPressed: bool) (state: GameState) =
    match state.Screen with
    | Home ->
        if spacePressed then startNewGame state else state
    | Playing ->
        if escPressed then returnHome state
        else updatePlaying dt spacePressed state
    | Transition ->
        updateTransition dt spacePressed state
    | Dying ->
        updateDying dt state
    | GameOver | NewHighScore ->
        if escPressed then returnHome state
        elif spacePressed then startNewGame state
        else state
