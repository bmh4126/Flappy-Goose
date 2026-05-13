module FlappyGoose.Constants

// Logical screen size (800x600) - do not change these
let screenWidth  = 800
let screenHeight = 600

// Scale factor - change this to scale the entire game
// 1.0 = 800x600, 1.5 = 1200x900, 2.0 = 1600x1200
let scale = 1.0f

// Scaled window dimensions
let scaledScreenWidth  = float32 screenWidth * scale
let scaledScreenHeight = float32 screenHeight * scale

// Goose (scaled)
let gooseStartX   = 150.0f * scale
let gooseStartY   = 270.0f * scale
let gooseWidth    = 72.0f * scale
let gooseHeight   = 54.0f * scale

// Physics (not scaled - relative to game logic)
let normalGravity     = 800.0f
let minRandomGravity  = 600.0f
let maxRandomGravity  = 3500.0f
let flapVelocity      = -380.0f

// Death animation
let deathGravity           = 1800.0f
let deathInitialVelocity   = 100.0f
let deathAnimationMaxTime  = 1.5f

// Obstacles (scaled)
let obstacleWidth         = 110.0f * scale
let gapHeight             = 200.0f * scale
let minGapY               = 90.0f * scale
let maxGapY               = scaledScreenHeight - (90.0f * scale) - gapHeight
let obstacleSpeed         = 120.0f
let speedIncreasePerStage = 80.0f
let baseObstacleSpacing   = 220.0f * scale
let minObstacleSpacing    = 280.0f * scale
let maxObstacleSpacing    = 500.0f * scale

// Stage and transition
let transitionDuration = 3.0f

// Rendering — separate thickness for each category
let gooseOutlineThickness    = 1.0f
let obstacleOutlineThickness = 3.0f
let lightRingOutlineThickness = 3.0f
let textOutlineThickness      = 5     // int pixels, used in outline loop
