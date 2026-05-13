module FlappyGoose.GameLogic

open FlappyGoose.GameTypes
open FlappyGoose.Constants
open FlappyGoose.Collision

let private rng = System.Random()

let private clamp (lo: float32) (hi: float32) (v: float32) =
    if v < lo then lo elif v > hi then hi else v

let chooseRandomGravity (currentGravity: float32) (stage: int) =
    let gravityRange = maxRandomGravity - minRandomGravity

    // Stage-based percentage ranges (% of gravity range, not current gravity)
    let (minChangePercent, maxChangePercent) =
        match stage with
        | 1 -> (0.0f, 10.0f)
        | 2 -> (10.0f, 25.0f)
        | 3 -> (20.0f, 40.0f)
        | 4 -> (30.0f, 55.0f)
        | _ -> (40.0f, 70.0f)  // Stage 5+

    let changePercent = minChangePercent + float32 (rng.NextDouble()) * (maxChangePercent - minChangePercent)
    let changeAmount = gravityRange * changePercent / 100.0f

    // Randomly pick initial direction
    let direction = if rng.NextDouble() < 0.5 then 1.0f else -1.0f

    let newGravityUp = currentGravity + changeAmount
    let newGravityDown = currentGravity - changeAmount

    let isUpValid = newGravityUp >= minRandomGravity && newGravityUp <= maxRandomGravity
    let isDownValid = newGravityDown >= minRandomGravity && newGravityDown <= maxRandomGravity

    // Try preferred direction first, then opposite, then pick best overflow
    match (isUpValid, isDownValid, direction > 0.0f) with
    | (true, _, true) -> newGravityUp  // Up is valid and preferred
    | (_, true, false) -> newGravityDown  // Down is valid and preferred
    | (true, _, _) -> newGravityUp  // Up is valid
    | (_, true, _) -> newGravityDown  // Down is valid
    | (false, false, _) ->
        // Both overflow, choose the one with larger change magnitude
        let upDiff = System.MathF.Abs(newGravityUp - currentGravity)
        let downDiff = System.MathF.Abs(newGravityDown - currentGravity)
        if upDiff >= downDiff then clamp minRandomGravity maxRandomGravity newGravityUp
        else clamp minRandomGravity maxRandomGravity newGravityDown

let obstaclesForStage (stage: int) = min 7 (4 + stage)
// stage 1 -> 5, stage 2 -> 6, stage 3+ -> 7

// Physics-constrained, directionally-biased gap center selection.
// stageIndex = stage - 1 (0-based) controls desired shift magnitude.
let spacingForSpeed (speed: float32) =
    // Use square root for slower spacing increase as speed increases
    let speedRatio = speed / obstacleSpeed
    let Ratio = 1.5f * System.MathF.Sqrt(speedRatio)
    clamp minObstacleSpacing maxObstacleSpacing (baseObstacleSpacing * Ratio)

let private generateObstacles (count: int) (gravity: float32) (speed: float32) (stageIndex: int) (firstObstacleFullRange: bool) =
    let spacing = spacingForSpeed speed
    let startX = float32 screenWidth + spacing
    let midGapY = (minGapY + maxGapY) / 2.0f
    let switchProbability = min 0.9f (0.4f + float32 stageIndex * 0.1f)

    let mutable prevGapY = midGapY
    let mutable prevAboveMiddle = false

    [ for i in 0 .. count - 1 do
        let gapY =
            if i = 0 then
                // First obstacle uses full range
                let range = maxGapY - minGapY
                minGapY + float32 (rng.NextDouble()) * range
            else
                // Subsequent obstacles prefer the opposite side with probability
                let shouldSwitch = float32 (rng.NextDouble()) < switchProbability
                let aboveMiddle = if shouldSwitch then not prevAboveMiddle else prevAboveMiddle

                let range = maxGapY - midGapY
                if aboveMiddle then
                    // Upper half: between midGapY and maxGapY
                    midGapY + float32 (rng.NextDouble()) * range
                else
                    // Lower half: between minGapY and midGapY
                    minGapY + float32 (rng.NextDouble()) * range

        prevGapY <- gapY
        prevAboveMiddle <- gapY > midGapY

        yield { X = startX + float32 i * spacing
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
      ObstaclesSpawnedInStage = 0
      LastGapY = (minGapY + maxGapY) / 2.0f
      FlapTimer = 0.0f
      DyingTimer = 0.0f
      ResultScreen = GameOver
      WaitingForObstacleClear = false
      ScreenBeforePause = Home
      CountdownTime = 0.0f }

let startNewGame (state: GameState) =
    let obs = generateObstacles (obstaclesForStage 1) normalGravity obstacleSpeed 0 true
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
        ObstaclesSpawnedInStage = obstaclesForStage 1
        LastGapY = (minGapY + maxGapY) / 2.0f
        FlapTimer = 0.0f
        DyingTimer = 0.0f
        ResultScreen = GameOver
        WaitingForObstacleClear = false
        ScreenBeforePause = Home
        CountdownTime = 0.0f
        PreviousHighScore = state.HighScore }  // snapshot current high score for mid-run comparison

let returnHome (state: GameState) =
    { state with Screen = Home }

let pauseGame (state: GameState) =
    match state.Screen with
    | Playing | Transition ->
        { state with Screen = Paused; ScreenBeforePause = state.Screen }
    | _ -> state

let resumeGame (state: GameState) =
    if state.Screen = Paused then
        { state with Screen = UnpausingCountdown; CountdownTime = 3.0f }
    else state

let updateCountdown (dt: float32) (state: GameState) =
    let remaining = state.CountdownTime - dt
    if remaining <= 0.0f then
        { state with Screen = state.ScreenBeforePause; CountdownTime = 0.0f }
    else
        { state with CountdownTime = remaining }

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
    let nextGrav = chooseRandomGravity state.Gravity state.Stage
    { state with
        Screen = Transition
        TransitionTime = transitionDuration
        Gravity = nextGrav
        NextGravity = nextGrav
        Obstacles = []
        WaitingForObstacleClear = false }

let updateTransition (dt: float32) (spacePressed: bool) (state: GameState) =
    let s0 = if spacePressed then flapGoose state else state
    let s1 = updateGoose dt s0
    let s2 = { s1 with Goose = clampGooseToBounds s1.Goose }
    let remaining = state.TransitionTime - dt
    if remaining <= 0.0f then
        let nextStage = state.Stage + 1
        let newSpeed  = obstacleSpeed + float32 state.Stage * speedIncreasePerStage
        let obs = generateObstacles (obstaclesForStage nextStage) state.Gravity newSpeed (nextStage - 1) true
        { s2 with
            Screen = Playing
            Stage = nextStage
            Speed = newSpeed
            Obstacles = obs
            ObstaclesPassedInStage = 0
            ObstaclesSpawnedInStage = obstaclesForStage nextStage
            TransitionTime = 0.0f
            WaitingForObstacleClear = false
            CountdownTime = 0.0f }
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
        Goose        = { state.Goose with Velocity = deathInitialVelocity}
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

    let s4 =
        if s3.ObstaclesPassedInStage >= limit && not state.WaitingForObstacleClear then
            { s3 with WaitingForObstacleClear = true }
        else s3

    let allObstaclesClear = s4.Obstacles |> List.forall (fun o -> o.X + o.Width < -10.0f)
    if state.WaitingForObstacleClear && allObstaclesClear then
        startTransition s4
    else
        checkCollision s4

let updateGame (dt: float32) (spacePressed: bool) (escPressed: bool) (state: GameState) =
    match state.Screen with
    | Home ->
        if spacePressed then startNewGame state else state
    | Playing ->
        if escPressed then pauseGame state
        else updatePlaying dt spacePressed state
    | Transition ->
        if escPressed then pauseGame state
        else updateTransition dt spacePressed state
    | Paused ->
        if escPressed then returnHome state
        elif spacePressed then resumeGame state
        else state
    | UnpausingCountdown ->
        updateCountdown dt state
    | Dying ->
        updateDying dt state
    | GameOver | NewHighScore ->
        if escPressed then returnHome state
        elif spacePressed then startNewGame state
        else state
