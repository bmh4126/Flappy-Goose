# Flappy Goose — Requirements

## Screens
- **Home** — title, session high score, start prompt
- **Playing** — goose, obstacles, score, stage
- **Transition** — gravity-shift message, countdown (~3 s), goose immortal
- **GameOver** — final score, high score, restart / home options
- **NewHighScore** — congratulation, new high score, restart / home options

## Gameplay
- Player presses **Space** to flap upward; gravity pulls the goose down.
- KAIST monument-inspired obstacles move right-to-left.
- Each obstacle has one open gap; passing it scores +1.
- Collision with an obstacle or leaving the screen boundary ends the game.

## Obstacle Rules
- Gap size is fixed and large enough to pass through.
- Gap position is randomly constrained — never impossible.
- Consecutive gaps shift by at most `maxGapShift` vertically.

## Stage System
- Stage 1 uses normal gravity.
- Each stage has a fixed number of obstacles (`obstaclesPerStage`).
- After all obstacles in a stage are passed, a ~3 s transition begins.
- Transition: no obstacles, goose immortal, gravity-shift message shown.
- After transition, next stage begins with a randomly chosen gravity from a safe bounded range.

## High Score
- Session only — resets to 0 when the program closes.
- Home screen always displays the current high score.
- Game-over comparison: if `score > highScore`, show NewHighScore screen and update high score.
- Restarting preserves the high score.

## Controls
| Key   | Action                        |
|-------|-------------------------------|
| Enter | Start game from home screen   |
| Space | Flap during gameplay          |
| R     | Restart after game over       |
| H     | Return to home after game over|
| Esc   | Quit                          |

## Collision
- Simple axis-aligned rectangle intersection.
- Slightly inset goose hitbox for fairness.
- Top and bottom obstacle rectangles flank the gap.

## Not in Scope
- Persistent high score (file/database)
- Networking or online leaderboard
- Mobile support
- Pixel-perfect collision
