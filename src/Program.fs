module FlappyGoose.Program
#nowarn "3391"

open Raylib_cs
open FlappyGoose.Constants
open FlappyGoose.GameLogic
open FlappyGoose.Rendering
open FlappyGoose.Assets

[<EntryPoint>]
let main _ =
    Raylib.InitWindow(screenWidth, screenHeight, "Flappy Goose")
    Raylib.SetTargetFPS(60)

    let assets = load ()
    let mutable state = createInitialState ()

    while not (Raylib.WindowShouldClose()) do
        let dt = Raylib.GetFrameTime()

        let enterPressed = Raylib.IsKeyPressed(KeyboardKey.Enter)
        let spacePressed = Raylib.IsKeyPressed(KeyboardKey.Space)
        let rPressed     = Raylib.IsKeyPressed(KeyboardKey.R)
        let hPressed     = Raylib.IsKeyPressed(KeyboardKey.H)

        state <- updateGame dt enterPressed spacePressed rPressed hPressed state

        draw state assets

    unload assets
    Raylib.CloseWindow()
    0
