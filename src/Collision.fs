module FlappyGoose.Collision

open FlappyGoose.GameTypes
open FlappyGoose.Constants

type Rectangle = { X: float32; Y: float32; Width: float32; Height: float32 }

let intersects (a: Rectangle) (b: Rectangle) =
    a.X < b.X + b.Width  &&
    a.X + a.Width  > b.X &&
    a.Y < b.Y + b.Height &&
    a.Y + a.Height > b.Y

// Slightly inset hitbox for fairness
let gooseRect (g: Goose) =
    { X = g.X + 4.0f; Y = g.Y + 4.0f
      Width = gooseWidth - 8.0f; Height = gooseHeight - 8.0f }

let topObstacleRect (o: Obstacle) =
    { X = o.X; Y = 0.0f; Width = o.Width; Height = o.GapY }

let bottomObstacleRect (o: Obstacle) =
    let bottom = o.GapY + o.GapHeight
    { X = o.X; Y = bottom; Width = o.Width; Height = float32 screenHeight - bottom }

let collidesWithObstacle (g: Goose) (o: Obstacle) =
    let gr = gooseRect g
    intersects gr (topObstacleRect o) || intersects gr (bottomObstacleRect o)

let collidesWithAnyObstacle (g: Goose) (obstacles: Obstacle list) =
    List.exists (collidesWithObstacle g) obstacles

let isOutOfBounds (g: Goose) =
    g.Y < 0.0f || g.Y + gooseHeight > float32 screenHeight
