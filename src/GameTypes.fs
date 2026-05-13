module FlappyGoose.GameTypes

type Goose = {
    X: float32
    Y: float32
    Velocity: float32
}

type Obstacle = {
    X: float32
    Width: float32
    GapY: float32
    GapHeight: float32
    Passed: bool
}

type GameScreen =
    | Home
    | Playing
    | Transition
    | Paused
    | UnpausingCountdown
    | Dying
    | GameOver
    | NewHighScore

type GameState = {
    Goose: Goose
    Obstacles: Obstacle list
    Score: int
    HighScore: int
    PreviousHighScore: int     // high score at the start of this run, for mid-game comparison
    Stage: int
    Gravity: float32
    Speed: float32
    Screen: GameScreen
    TransitionTime: float32
    NextGravity: float32
    ObstaclesPassedInStage: int
    ObstaclesSpawnedInStage: int
    LastGapY: float32
    FlapTimer: float32
    DyingTimer: float32
    ResultScreen: GameScreen   // GameOver or NewHighScore, resolved when dying starts
    WaitingForObstacleClear: bool
    ScreenBeforePause: GameScreen  // which screen we were on before pausing
    CountdownTime: float32  // countdown timer when resuming from pause
}
