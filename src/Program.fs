module FlappyGoose.Program
#nowarn "3391"

open Raylib_cs
open FlappyGoose.Constants
open FlappyGoose.GameTypes
open FlappyGoose.GameLogic
open FlappyGoose.Rendering
open FlappyGoose.Assets

[<EntryPoint>]
let main _ =
    Raylib.InitWindow(screenWidth, screenHeight, "Flappy Goose")
    Raylib.SetTargetFPS(60)
    // Disable default Esc-closes-window so we can handle Esc context-sensitively.
    Raylib.SetExitKey(KeyboardKey.Null)

    let assets = load ()
    let mutable state = createInitialState ()
    let mutable running = true

    while running && not (Raylib.WindowShouldClose()) do
        let dt           = Raylib.GetFrameTime()
        let spacePressed = Raylib.IsKeyPressed(KeyboardKey.Space)
        let escPressed   = Raylib.IsKeyPressed(KeyboardKey.Escape)

        // Esc on Home screen quits the program; everywhere else it goes handled by updateGame.
        if state.Screen = Home && escPressed then
            running <- false
        else
            state <- updateGame dt spacePressed escPressed state

        draw state assets

    unload assets
    Raylib.CloseWindow()
    0
