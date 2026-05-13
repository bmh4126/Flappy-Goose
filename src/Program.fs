module FlappyGoose.Program
#nowarn "3391"

open Raylib_cs
open System.Numerics
open FlappyGoose.Constants
open FlappyGoose.GameTypes
open FlappyGoose.GameLogic
open FlappyGoose.Rendering
open FlappyGoose.Assets

let private drawScaledGame (state: GameState) (assets: Assets) (target: RenderTexture2D) =
    Raylib.BeginTextureMode(target)
    draw state assets
    Raylib.EndTextureMode()

    let windowWidth = Raylib.GetScreenWidth()
    let windowHeight = Raylib.GetScreenHeight()

    let scaleX = float32 windowWidth / float32 screenWidth
    let scaleY = float32 windowHeight / float32 screenHeight
    let scale = if scaleX < scaleY then scaleX else scaleY

    let destWidth = float32 screenWidth * scale
    let destHeight = float32 screenHeight * scale
    let offsetX = (float32 windowWidth - destWidth) / 2.0f
    let offsetY = (float32 windowHeight - destHeight) / 2.0f

    Raylib.BeginDrawing()
    Raylib.ClearBackground(Color.Black)

    let srcRect = Rectangle(0.0f, 0.0f, float32 screenWidth, -float32 screenHeight)
    let dstRect = Rectangle(offsetX, offsetY, destWidth, destHeight)
    Raylib.DrawTexturePro(target.Texture, srcRect, dstRect, Vector2.Zero, 0.0f, Color.White)

    Raylib.EndDrawing()

[<EntryPoint>]
let main _ =
    // Initialize window (fullscreen feature removed due to macOS compatibility issues)
    Raylib.InitWindow(int scaledScreenWidth, int scaledScreenHeight, "Flappy Goose")
    Raylib.SetTargetFPS(60)
    Raylib.SetExitKey(KeyboardKey.Null)

    let target = Raylib.LoadRenderTexture(screenWidth, screenHeight)
    let assets = load ()
    let mutable state = createInitialState ()
    let mutable running = true

    while running && not (Raylib.WindowShouldClose()) do
        let dt           = Raylib.GetFrameTime()
        let spacePressed = Raylib.IsKeyPressed(KeyboardKey.Space)
        let escPressed   = Raylib.IsKeyPressed(KeyboardKey.Escape)

        if state.Screen = Home && escPressed then
            running <- false
        else
            state <- updateGame dt spacePressed escPressed state

        // Note: F11 fullscreen toggle can be added later with proper CBool handling

        drawScaledGame state assets target

    Raylib.UnloadRenderTexture(target)
    unload assets
    Raylib.CloseWindow()
    0
