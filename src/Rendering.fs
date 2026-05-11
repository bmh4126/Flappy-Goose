module FlappyGoose.Rendering

open Raylib_cs
open FlappyGoose.GameTypes
open FlappyGoose.Constants
open FlappyGoose.Assets

let private skyBlue   = Color(135uy, 206uy, 235uy, 255uy)
let private darkGreen = Color(34uy, 139uy, 34uy, 255uy)
let private white     = Color.White
let private black     = Color.Black
let private yellow    = Color(255uy, 200uy, 0uy, 255uy)
let private orange    = Color(220uy, 100uy, 20uy, 255uy)
let private gray      = Color(100uy, 100uy, 120uy, 255uy)
let private darkGray  = Color(60uy, 60uy, 70uy, 255uy)
let private red       = Color(200uy, 40uy, 40uy, 255uy)
let private gold      = Color(255uy, 215uy, 0uy, 255uy)

let private centeredText (text: string) (y: int) (size: int) (color: Color) =
    let w = Raylib.MeasureText(text, size)
    Raylib.DrawText(text, (screenWidth - w) / 2, y, size, color)

let private drawBackground () =
    Raylib.ClearBackground(skyBlue)
    // Grass strip at bottom
    Raylib.DrawRectangle(0, screenHeight - 40, screenWidth, 40, darkGreen)

let private drawGoose (g: Goose) =
    // Placeholder: orange body + yellow beak
    let x = int g.X
    let y = int g.Y
    let w = int gooseWidth
    let h = int gooseHeight
    Raylib.DrawEllipse(x + w/2, y + h/2, float32 w / 2.0f, float32 h / 2.0f, white)
    // Head
    Raylib.DrawCircle(x + w - 6, y + 8, 10.0f, white)
    // Beak
    Raylib.DrawTriangle(
        System.Numerics.Vector2(float32 (x + w + 4), float32 (y + 8)),
        System.Numerics.Vector2(float32 (x + w - 2), float32 (y + 5)),
        System.Numerics.Vector2(float32 (x + w - 2), float32 (y + 11)),
        orange)
    // Eye
    Raylib.DrawCircle(x + w - 3, y + 7, 2.0f, black)

let private drawObstacle (o: Obstacle) =
    let x = int o.X
    let w = int o.Width
    // Top pillar
    let topH = int o.GapY
    Raylib.DrawRectangle(x, 0, w, topH, gray)
    Raylib.DrawRectangle(x - 5, topH - 20, w + 10, 20, darkGray)
    // Bottom pillar
    let bottomY = int (o.GapY + o.GapHeight)
    let bottomH = screenHeight - bottomY
    Raylib.DrawRectangle(x, bottomY, w, bottomH, gray)
    Raylib.DrawRectangle(x - 5, bottomY, w + 10, 20, darkGray)

let private drawHud (state: GameState) =
    Raylib.DrawText(sprintf "Score: %d" state.Score, 10, 10, 28, white)
    Raylib.DrawText(sprintf "Stage: %d" state.Stage, 10, 44, 22, white)
    Raylib.DrawText(sprintf "Best: %d"  state.HighScore, screenWidth - 130, 10, 22, white)

let drawHome (state: GameState) (_assets: Assets) =
    drawBackground ()
    centeredText "FLAPPY GOOSE" (screenHeight / 2 - 120) 60 yellow
    centeredText "KAIST Goose Pond Edition" (screenHeight / 2 - 50) 24 white
    centeredText (sprintf "Best: %d" state.HighScore) (screenHeight / 2 + 10) 30 gold
    centeredText "Press ENTER to start" (screenHeight / 2 + 70) 26 white
    centeredText "SPACE to flap  |  ESC to quit" (screenHeight / 2 + 110) 20 white

let drawPlaying (state: GameState) (_assets: Assets) =
    drawBackground ()
    List.iter drawObstacle state.Obstacles
    drawGoose state.Goose
    drawHud state

let drawTransition (state: GameState) (_assets: Assets) =
    drawBackground ()
    drawGoose state.Goose
    centeredText "GRAVITY SHIFT!" (screenHeight / 2 - 80) 50 yellow
    centeredText (sprintf "Next gravity: %.0f" state.NextGravity) (screenHeight / 2 - 20) 28 white
    centeredText (sprintf "Stage %d starting in %.1f..." (state.Stage + 1) state.TransitionTime) (screenHeight / 2 + 30) 26 white

let drawGameOver (state: GameState) (_assets: Assets) =
    drawBackground ()
    centeredText "GAME OVER" (screenHeight / 2 - 100) 56 red
    centeredText (sprintf "Score: %d" state.Score) (screenHeight / 2 - 30) 36 white
    centeredText (sprintf "Best:  %d" state.HighScore) (screenHeight / 2 + 20) 30 gold
    centeredText "R - Restart   H - Home" (screenHeight / 2 + 90) 26 white

let drawNewHighScore (state: GameState) (_assets: Assets) =
    drawBackground ()
    centeredText "NEW HIGH SCORE!" (screenHeight / 2 - 110) 52 gold
    centeredText "HONK HONK!" (screenHeight / 2 - 50) 36 yellow
    centeredText (sprintf "Score: %d" state.Score) (screenHeight / 2 + 10) 36 white
    centeredText (sprintf "Best:  %d" state.HighScore) (screenHeight / 2 + 55) 30 gold
    centeredText "R - Restart   H - Home" (screenHeight / 2 + 110) 26 white

let draw (state: GameState) (assets: Assets) =
    Raylib.BeginDrawing()
    match state.Screen with
    | Home        -> drawHome state assets
    | Playing     -> drawPlaying state assets
    | Transition  -> drawTransition state assets
    | GameOver    -> drawGameOver state assets
    | NewHighScore -> drawNewHighScore state assets
    Raylib.EndDrawing()
