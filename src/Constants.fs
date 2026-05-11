module FlappyGoose.Constants

let screenWidth  = 800
let screenHeight = 600

let gooseStartX   = 150.0f
let gooseStartY   = 300.0f
let gooseWidth    = 48.0f
let gooseHeight   = 36.0f

let normalGravity     = 800.0f
let minRandomGravity  = 500.0f
let maxRandomGravity  = 1200.0f

let flapVelocity = -350.0f

let obstacleWidth        = 80.0f
let gapHeight            = 160.0f
let minGapY              = 80.0f
let maxGapY              = float32 screenHeight - 80.0f - gapHeight
let maxGapShift          = 100.0f

let obstacleSpeed        = 220.0f
let speedIncreasePerStage = 20.0f
let obstaclesPerStage    = 5
let transitionDuration   = 3.0f

let obstacleSpacing      = 280.0f
