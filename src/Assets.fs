module FlappyGoose.Assets

// Placeholder module — no image files are required to run the game yet.
// Final assets to add under Assets/:
//   goose.png            — goose character sprite
//   goose_flap.png       — goose mid-flap variant
//   monument_obstacle.png — KAIST monument obstacle skin
//   background.png       — goose pond / campus background

// When assets are ready, load them here after InitWindow using Raylib-cs:
//   let texture = Raylib.LoadTexture("Assets/goose.png")
// and unload them on shutdown:
//   Raylib.UnloadTexture(texture)

type Assets = { Placeholder: unit }

let load () = { Placeholder = () }
let unload (_: Assets) = ()
