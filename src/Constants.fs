module FlappyGoose.Constants

let screenWidth  = 800
let screenHeight = 600

// Goose
let gooseStartX   = 150.0f
let gooseStartY   = 270.0f
let gooseWidth    = 72.0f
let gooseHeight   = 54.0f

// Physics
let normalGravity     = 800.0f
let minRandomGravity  = 600.0f
let maxRandomGravity  = 4000.0f
let flapVelocity      = -380.0f

// Death animation
let deathGravity           = 1800.0f
let deathInitialVelocity   = 100.0f
let deathAnimationMaxTime  = 1.5f

// Obstacles
let obstacleWidth         = 110.0f
let gapHeight             = 200.0f
let minGapY               = 90.0f
let maxGapY               = float32 screenHeight - 90.0f - gapHeight
let obstacleSpeed         = 120.0f
let speedIncreasePerStage = 100.0f
let obstacleSpacing       = 320.0f

// Stage-based gap shift — directional movement between consecutive obstacles
let baseGapShift     = 70.0f    // minimum desired shift per obstacle
let gapShiftPerStage = 25.0f    // added per stage index
let maxGapShiftCap   = 220.0f   // absolute ceiling

// Stage and transition
let transitionDuration = 3.0f

// Rendering — separate thickness for each category
let gooseOutlineThickness    = 1.0f
let obstacleOutlineThickness = 3.0f
let lightRingOutlineThickness = 3.0f
let textOutlineThickness      = 5     // int pixels, used in outline loop
