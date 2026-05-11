module FlappyGoose.Rendering

open Raylib_cs
open System.Numerics
open FlappyGoose.GameTypes
open FlappyGoose.Constants
open FlappyGoose.Assets

let private yellow     = Color(255uy, 200uy, 0uy,  255uy)
let private red        = Color(200uy, 40uy,  40uy, 255uy)
let private gold       = Color(255uy, 215uy, 0uy,  255uy)
let private white      = Color.White
let private black      = Color.Black
let private yellowTint = Color(255uy, 220uy, 0uy,  140uy)   // semi-transparent yellow for immortal glow

// ── Core outline primitive ───────────────────────────────────────────────────

let private outlineOffsets (t: float32) : (float32 * float32) list =
    [ (-t, 0f); (t, 0f); (0f, -t); (0f, t)
      (-t, -t); (t, -t); (-t,  t); (t,  t) ]

// Draw a texture region (src) into dst with rotation; first renders 8 black outline copies, then the sprite.
let private drawProOutlined (tex: Texture2D) (src: Rectangle) (dst: Rectangle) (origin: Vector2) (rotation: float32) (thickness: float32) =
    for (dx, dy) in outlineOffsets thickness do
        let dstOff = Rectangle(dst.X + dx, dst.Y + dy, dst.Width, dst.Height)
        Raylib.DrawTexturePro(tex, src, dstOff, origin, rotation, black)
    Raylib.DrawTexturePro(tex, src, dst, origin, rotation, white)

// Plain draw (no outline) — used for tinted overlays and background.
let private drawPro (tex: Texture2D) (src: Rectangle) (dst: Rectangle) (tint: Color) =
    Raylib.DrawTexturePro(tex, src, dst, Vector2.Zero, 0.0f, tint)

// ── Convenience wrappers ─────────────────────────────────────────────────────

// Full texture → destination rect, with per-category outline thickness.
let private drawTexOutlined (tex: Texture2D) (x: float32) (y: float32) (w: float32) (h: float32) (thickness: float32) =
    let src = Rectangle(0.0f, 0.0f, float32 tex.Width, float32 tex.Height)
    let dst = Rectangle(x, y, w, h)
    drawProOutlined tex src dst Vector2.Zero 0.0f thickness

// Full texture → destination rect with given tint (for overlay effects, no outline).
let private drawTexTinted (tex: Texture2D) (x: float32) (y: float32) (w: float32) (h: float32) (tint: Color) =
    let src = Rectangle(0.0f, 0.0f, float32 tex.Width, float32 tex.Height)
    let dst = Rectangle(x, y, w, h)
    drawPro tex src dst tint

// Horizontal slice [srcY .. srcY+srcH] of texture → destination rect, with outline.
let private drawTexSliceOutlined (tex: Texture2D) (srcY: float32) (srcH: float32)
                                  (dstX: float32) (dstY: float32) (dstW: float32) (dstH: float32) (thickness: float32) =
    let src = Rectangle(0.0f, srcY, float32 tex.Width, srcH)
    let dst = Rectangle(dstX, dstY, dstW, dstH)
    drawProOutlined tex src dst Vector2.Zero 0.0f thickness

// Texture centered at (cx, cy) with optional rotation, with outline.
let private drawTexCenteredOutlined (tex: Texture2D) (cx: float32) (cy: float32) (w: float32) (h: float32) (rotation: float32) (thickness: float32) =
    let src    = Rectangle(0.0f, 0.0f, float32 tex.Width, float32 tex.Height)
    let dst    = Rectangle(cx, cy, w, h)
    let origin = Vector2(w / 2.0f, h / 2.0f)
    drawProOutlined tex src dst origin rotation thickness

// ── Background ───────────────────────────────────────────────────────────────

let private drawBackground (assets: Assets) =
    Raylib.ClearBackground(Color.Black)
    let src = Rectangle(0.0f, 0.0f, float32 assets.Background.Width, float32 assets.Background.Height)
    let dst = Rectangle(0.0f, 0.0f, float32 screenWidth, float32 screenHeight)
    drawPro assets.Background src dst white

// ── Goose ────────────────────────────────────────────────────────────────────

let private drawGoose (g: Goose) (assets: Assets) (flapTimer: float32) (immortalOverlay: bool) =
    let tex = if flapTimer > 0.0f then assets.GooseFlap else assets.Goose
    drawTexOutlined tex g.X g.Y gooseWidth gooseHeight gooseOutlineThickness
    if immortalOverlay then
        drawTexTinted tex g.X g.Y gooseWidth gooseHeight yellowTint

// ── Obstacles ────────────────────────────────────────────────────────────────

let private drawObstacle (o: Obstacle) (assets: Assets) =
    let gapTop    = o.GapY
    let gapBottom = o.GapY + o.GapHeight
    let x  = o.X
    let w  = o.Width
    let sh = float32 screenHeight
    let texH = float32 assets.Monument.Height
    let scaleY = texH / sh   // maps screen y to texture y for continuous-monument look

    if gapTop > 0.0f then
        drawTexSliceOutlined assets.Monument 0.0f (gapTop * scaleY) x 0.0f w gapTop obstacleOutlineThickness

    let bottomSrcY = gapBottom * scaleY
    let bottomSrcH = texH - bottomSrcY
    let bottomDstH = sh - gapBottom
    if bottomDstH > 0.0f then
        drawTexSliceOutlined assets.Monument bottomSrcY bottomSrcH x gapBottom w bottomDstH obstacleOutlineThickness

    // Light rings overlaid at gap boundaries
    let ringAspect = float32 assets.LightsRing.Height / float32 assets.LightsRing.Width
    let ringW = w
    let ringH = ringW * ringAspect
    let cx    = x + w / 2.0f

    drawTexCenteredOutlined assets.LightsRing cx gapTop    ringW ringH   0.0f lightRingOutlineThickness
    drawTexCenteredOutlined assets.LightsRing cx gapBottom ringW ringH 180.0f lightRingOutlineThickness

// ── Text helpers ─────────────────────────────────────────────────────────────

// All text uses textOutlineThickness (thick black outline for readability on bright background).
let private outlinedText (text: string) (x: int) (y: int) (size: int) (fg: Color) =
    let t = textOutlineThickness
    for dx in -t .. t do
        for dy in -t .. t do
            if dx <> 0 || dy <> 0 then
                Raylib.DrawText(text, x + dx, y + dy, size, black)
    Raylib.DrawText(text, x, y, size, fg)

let private centeredText (text: string) (y: int) (size: int) (fg: Color) =
    let w = Raylib.MeasureText(text, size)
    outlinedText text ((screenWidth - w) / 2) y size fg

// ── HUD ──────────────────────────────────────────────────────────────────────

let private drawHud (state: GameState) =
    let isBeatingRecord = state.Score > state.PreviousHighScore
    if isBeatingRecord then
        // Score turns yellow; Best display is hidden; NEW HIGH SCORE label appears.
        outlinedText (sprintf "Score: %d" state.Score) 10 10 28 yellow
        outlinedText "NEW HIGH SCORE" 10 46 18 yellow
        // Yellow accent line below the label
        let labelW = Raylib.MeasureText("NEW HIGH SCORE", 18)
        Raylib.DrawRectangle(10, 68, labelW, 4, yellow)
        outlinedText (sprintf "Stage: %d" state.Stage) 10 78 20 white
    else
        outlinedText (sprintf "Score: %d" state.Score)      10               10 28 white
        outlinedText (sprintf "Stage: %d" state.Stage)      10               44 22 white
        outlinedText (sprintf "Best: %d"  state.HighScore) (screenWidth-130) 10 22 white

// ── Screens ──────────────────────────────────────────────────────────────────

let drawHome (state: GameState) (assets: Assets) =
    drawBackground assets
    centeredText "FLAPPY GOOSE"                        (screenHeight / 2 - 120) 60 yellow
    centeredText "KAIST Goose Pond Edition"             (screenHeight / 2 - 50)  24 white
    centeredText (sprintf "Best: %d" state.HighScore)  (screenHeight / 2 + 10)  30 gold
    centeredText "Press SPACE to start"                (screenHeight / 2 + 70)  26 white
    centeredText "ESC to quit"                         (screenHeight / 2 + 110) 20 white

let drawPlaying (state: GameState) (assets: Assets) =
    drawBackground assets
    List.iter (fun o -> drawObstacle o assets) state.Obstacles
    drawGoose state.Goose assets state.FlapTimer false
    drawHud state

let drawTransition (state: GameState) (assets: Assets) =
    drawBackground assets
    let showGlow =
        if state.TransitionTime > 1.0f then true
        else (int (state.TransitionTime / 0.2f)) % 2 = 0
    drawGoose state.Goose assets state.FlapTimer showGlow
    centeredText "GRAVITY SHIFT!"                                          (screenHeight / 2 - 90) 50 yellow
    centeredText "The goose is immortal in this stage"                     (screenHeight / 2 - 30) 22 yellow
    centeredText (sprintf "New gravity: %.0f" state.NextGravity)           (screenHeight / 2 + 12) 24 white
    centeredText (sprintf "Stage %d in %.1f..." (state.Stage + 1) state.TransitionTime) (screenHeight / 2 + 50) 24 white

let drawDying (state: GameState) (assets: Assets) =
    drawBackground assets
    List.iter (fun o -> drawObstacle o assets) state.Obstacles
    drawTexOutlined assets.Goose state.Goose.X state.Goose.Y gooseWidth gooseHeight gooseOutlineThickness

let drawGameOver (state: GameState) (assets: Assets) =
    drawBackground assets
    centeredText "GAME OVER"                            (screenHeight / 2 - 100) 56 red
    centeredText (sprintf "Score: %d" state.Score)      (screenHeight / 2 - 30)  36 white
    centeredText (sprintf "Best:  %d" state.HighScore)  (screenHeight / 2 + 20)  30 gold
    centeredText "SPACE - Restart   ESC - Home"         (screenHeight / 2 + 90)  26 white

let drawNewHighScore (state: GameState) (assets: Assets) =
    drawBackground assets
    centeredText "NEW HIGH SCORE!"                      (screenHeight / 2 - 110) 52 gold
    centeredText "HONK HONK!"                           (screenHeight / 2 - 50)  36 yellow
    centeredText (sprintf "Score: %d" state.Score)      (screenHeight / 2 + 10)  36 white
    centeredText (sprintf "Best:  %d" state.HighScore)  (screenHeight / 2 + 55)  30 gold
    centeredText "SPACE - Restart   ESC - Home"         (screenHeight / 2 + 110) 26 white

let draw (state: GameState) (assets: Assets) =
    Raylib.BeginDrawing()
    match state.Screen with
    | Home         -> drawHome state assets
    | Playing      -> drawPlaying state assets
    | Transition   -> drawTransition state assets
    | Dying        -> drawDying state assets
    | GameOver     -> drawGameOver state assets
    | NewHighScore -> drawNewHighScore state assets
    Raylib.EndDrawing()
