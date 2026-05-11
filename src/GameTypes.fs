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
    | GameOver
    | NewHighScore

type GameState = {
    Goose: Goose
    Obstacles: Obstacle list
    Score: int
    HighScore: int
    Stage: int
    Gravity: float32
    Speed: float32
    Screen: GameScreen
    TransitionTime: float32
    NextGravity: float32
    ObstaclesPassedInStage: int
    LastGapY: float32
}
