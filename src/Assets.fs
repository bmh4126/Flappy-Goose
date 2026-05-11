module FlappyGoose.Assets

open Raylib_cs

type Assets = {
    Goose: Texture2D
    GooseFlap: Texture2D
    Background: Texture2D
    Monument: Texture2D
    LightsRing: Texture2D
}

let load () = {
    Goose      = Raylib.LoadTexture("Assets/goose.png")
    GooseFlap  = Raylib.LoadTexture("Assets/goose_flap.png")
    Background = Raylib.LoadTexture("Assets/background.png")
    Monument   = Raylib.LoadTexture("Assets/monument_obstacle.png")
    LightsRing = Raylib.LoadTexture("Assets/lights_ring.png")
}

let unload (a: Assets) =
    Raylib.UnloadTexture(a.Goose)
    Raylib.UnloadTexture(a.GooseFlap)
    Raylib.UnloadTexture(a.Background)
    Raylib.UnloadTexture(a.Monument)
    Raylib.UnloadTexture(a.LightsRing)
