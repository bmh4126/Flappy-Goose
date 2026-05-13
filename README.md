# Flappy Goose

**Flappy Goose** is a KAIST-themed Flappy Bird-style game written in **F#** using **.NET 10**.

The player controls a goose flying near the KAIST goose pond. Instead of classic green pipes, the obstacles are inspired by the tall three-color KAIST monument. The game includes a stage system where gravity changes randomly between stages, forcing the player to adapt to a new flying behavior.

---

## 1. Project Overview

In Flappy Goose, the player presses a key to make the goose flap upward. Gravity pulls the goose downward. The goal is to fly through gaps in KAIST monument obstacles and survive for as long as possible.

The game is divided into stages:

1. The first stage uses normal gravity.
2. After each stage, the game enters a transition phase.
3. During transition, no obstacles appear and the goose is temporarily immortal.
4. A new random gravity value is selected for the next stage.
5. After approximately 3 seconds, the next stage begins.

The player earns points by passing obstacles. When the goose collides with an obstacle or leaves the screen during normal gameplay, the game ends. The final score is compared with the current high score.

---

## 2. Gameplay Showcase

![Gameplay Screenshot](Assets/screenshot_high_score.png)

**My current high score is 54. Try to beat me! :D**

---

## 3. Features

- Graphical game window with resizable support and aspect ratio preservation
- KAIST-themed goose character
- KAIST monument-inspired obstacles
- Space-to-flap control
- Gravity-based movement
- Stage-based gravity variation (increases by 10% per stage up to 80-90%)
- Biased gap movement (gaps prefer opposite side from previous gap)
- 3-second safe transition phase between stages
- Score counter
- Stage counter
- Session high-score tracking
- Special game-over screen for a new high score
- Restart and return-to-home-screen flow
- Pause feature with Esc key
- 3-second countdown timer when resuming from pause
- Progressive obstacle spacing that increases slower at higher speeds

---

## 4. Requirements

This project requires:

- .NET 10 SDK
- F#
- A desktop environment capable of opening a graphical window

This project was developed for the CS-20200 Programming Principles term project and is implemented in F# with .NET 10.

---

## 4.5. Installing .NET 10

If you don't have .NET 10 installed, follow the instructions for your operating system:

### Windows

**Using Windows Package Manager (recommended):**

```bash
winget install Microsoft.DotNet.SDK.10
```

**Or manually from official website:**

1. Visit https://dotnet.microsoft.com/download/dotnet/10.0
2. Download the Windows installer (x64 or x86)
3. Run the installer and follow the on-screen instructions
4. Verify installation:

```bash
dotnet --version
```

### macOS

**Using Homebrew (recommended):**

```bash
brew install dotnet
```

If you don't have Homebrew, install it first:

```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

**Or manually from official website:**

1. Visit https://dotnet.microsoft.com/download/dotnet/10.0
2. Download the macOS installer (x64 or ARM64 for Apple Silicon)
3. Run the installer and follow the on-screen instructions
4. Verify installation:

```bash
dotnet --version
```

### Linux

**Using package manager (Ubuntu/Debian):**

```bash
sudo apt update
sudo apt install -y dotnet-sdk-10.0
```

**Using package manager (Fedora/RHEL):**

```bash
sudo dnf install -y dotnet-sdk-10.0
```

**Using package manager (Arch):**

```bash
sudo pacman -S dotnet-sdk
```

**Or manually from official website:**

1. Visit https://dotnet.microsoft.com/download/dotnet/10.0
2. Download the Linux installer for your distribution
3. Follow the provided instructions
4. Verify installation:

```bash
dotnet --version
```

---

## 5. How to Run

Clone the repository:

```bash
git clone https://github.com/bmh4126/Flappy-Goose.git
cd FLAPPY-GOOSE
```

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

Run the game:

```bash
dotnet run
```

If the project uses a specific project folder, run:

```bash
cd src
dotnet run
```

or:

```bash
dotnet run --project FlappyGoose.fsproj
```

---

## 6. Controls

| Key | Action |
| --- | --- |
| `Space` | Start game (Home), flap upward (Playing / Transition), continue game after countdown (Unpausing), restart (Game Over) |
| `Esc` | Quit (Home), pause game (Playing / Transition), return to Home screen (Paused / Game Over) |

**Pause Behavior:**
- Press `Esc` during gameplay to pause
- Press `Space` to resume with a 3-second countdown
- The countdown displays the current game state so you can see where the goose is
- After the countdown ends, gameplay resumes

The exact key bindings are displayed inside the game window.

---

---

## 7. Game Screens

### 7.1 Home Screen

The home screen displays:

- Game title: **Flappy Goose**
- Current high score
- Instructions for starting the game

Press the start key to begin a new game.

### 7.2 Gameplay Screen

During gameplay, the screen displays:

- Goose character
- KAIST monument obstacles
- Current score
- Current stage number
- Background near the goose pond

The player must guide the goose through the obstacle gaps.

### 7.3 Transition Screen

After the player passes all obstacles in a stage, the game enters a transition phase.

During this phase:

- Existing obstacles continue to move and eventually leave the screen
- No new obstacles spawn
- The goose is immortal and cannot die
- A gravity-shift message is displayed
- The new gravity value for the next stage is displayed
- A countdown timer is displayed
- The next stage's gravity is selected based on the cumulative percentage range for that stage

After approximately 3 seconds (when all remaining obstacles have moved off-screen), the next stage begins with new obstacles generated using the selected gravity.

### 7.4 Normal Game Over Screen

If the final score is not greater than the current high score, the normal game-over screen displays:

- Final score
- Current high score
- Restart instruction
- Home-screen instruction

### 7.5 New High Score Screen

If the final score is greater than the previous high score, a special game-over screen displays:

- Congratulation message
- Final score
- New high score
- Restart instruction
- Home-screen instruction

---

## 8. Gameplay Rules

1. The goose starts on the left side of the screen.
2. Gravity continuously pulls the goose downward.
3. Pressing `Space` gives the goose upward velocity.
4. Monument obstacles move from right to left.
5. Each obstacle has one open gap.
6. The player earns 1 point after successfully passing an obstacle.
7. The player loses if the goose collides with an obstacle during normal gameplay.
8. The player loses if the goose moves outside the top or bottom boundary during normal gameplay.
9. After passing all obstacles in a stage (max 7 obstacles), the game enters a transition phase.
10. During transition, the goose cannot die.
11. Obstacles remain active and move off-screen before the transition phase completes.
12. After transition, a new stage begins with a new gravity value.
13. Gap positions are biased to alternate sides (gaps prefer opposite vertical half from previous gap).
14. The high score is updated only when the final score is greater than the previous high score.
15. The game can be paused with `Esc` during Playing or Transition screens, with a 3-second countdown before resuming.

---

## 9. High Score System

The game tracks a high score during the current program session.

- The high score starts at 0 when the program opens.
- The home screen displays the current high score.
- After each game, the final score is compared with the high score.
- If the final score is higher, the high score is updated.
- If the player returns to the home screen, the updated high score is displayed.
- Closing and reopening the program may reset the high score to 0.

The high score is intentionally session-based. It is not required to persist after the program closes.

---

## 10. Obstacle and Collision Design

The visible obstacle is a KAIST monument-inspired image.

For simpler and fairer gameplay, collision detection does not use the exact visual shape of the monument. Instead, each obstacle uses two invisible rectangular hitboxes:

1. A top rectangle above the gap
2. A bottom rectangle below the gap

The goose can safely pass through the gap between these two rectangles.

This design keeps the gameplay readable and avoids unfair collisions with decorative visual details.

---

## 11. Random Gravity and Fairness

The game uses stage-based gravity variation to gradually increase difficulty:

**Gravity Variation by Stage:**
Gravity change is calculated as a percentage of the gravity range (max: 3500 - min: 600 = 2900):
- Stage 1: Gravity changes by 0-10% of range (~0-290)
- Stage 2: Gravity changes by 10-25% of range (~290-725)
- Stage 3: Gravity changes by 20-40% of range (~580-1160)
- Stage 4: Gravity changes by 30-55% of range (~870-1595)
- Stage 5+: Gravity changes by 40-70% of range (~1160-2030)

**Gravity Selection Logic:**
1. A random change amount is selected within the stage's range
2. A direction (increase or decrease) is randomly chosen
3. If the new gravity stays within bounds [600, 3500], it's accepted
4. If it goes outside bounds, the opposite direction is tried
5. If both directions overflow, the direction producing the larger absolute change from current gravity is chosen and then clamped

This ensures gravity always changes meaningfully while staying within playable bounds.

**Fair Obstacle Design:**
- The obstacle gap is large enough for the goose to pass through
- Gap positions are generated within safe vertical bounds (90 pixels from top/bottom)
- Gap positions are biased to alternate between upper and lower screen halves, increasing switch probability by 10% per stage (40% at stage 1, up to 90% max)
- Obstacle speed increases gradually as stages progress
- Obstacle spacing increases with speed, but uses square-root scaling to prevent spacing from growing too quickly
- The transition phase (3 seconds) gives the player time to adapt to new gravity before obstacles return
- During transition, the goose is rendered in front of shifted obstacles with a bright yellow glow for better visibility

---

## 12. Project Structure

A possible project structure is:

```text
FlappyGoose/
├── README.md
├── FlappyGoose.fsproj
├── Program.fs
├── GameTypes.fs
├── GameLogic.fs
├── Rendering.fs
├── Assets/
│   ├── goose.png
│   ├── goose_flap.png
│   ├── monument_obstacle.png
│   ├── lights_ring.png
│   ├── background.png
│   └── screenshot_high_score.png
└── Requirements.pdf
```

Possible file roles:

- `Program.fs`: main game loop and application entry point
- `GameTypes.fs`: game state, goose, obstacle, phase, and screen types
- `GameLogic.fs`: physics, scoring, collision, stage transition, and high-score logic
- `Rendering.fs`: drawing the goose, background, obstacles, UI text, and screens
- `Assets/`: image files used by the game

The actual file structure may differ, but all required source files and assets should be included in the repository.

---

## 13. Requirement Mapping

This section explains how the implementation corresponds to the requirements document.

| Requirement Area | Implementation Behavior |
| --- | --- |
| Home screen | The game starts at a home screen showing the title and high score. |
| Player control | The player presses `Space` to flap upward. |
| Gravity | The goose is continuously pulled downward by gravity, with stage-based variation. |
| Obstacles | KAIST monument obstacles move from right to left (max 7 per stage). |
| Collision | Two invisible rectangles are used for each obstacle. |
| Scoring | Passing an obstacle increases the score by 1. |
| Stage system | After all obstacles in a stage are passed (max 7), the game enters transition. |
| Transition | Transition lasts approximately 3 seconds, obstacles move off-screen, and losing is disabled. |
| Random gravity | Gravity varies by cumulative percentage ranges per stage (Stage 1: 0-10%, Stage 2: 10-20%, etc., up to 80-90%). |
| Gap variation | Gap positions are biased to alternate between upper and lower halves. |
| Game over | Collision or leaving the screen during normal play ends the game. |
| High score | Final score is compared with the session high score. |
| New high score | A special congratulation screen appears when the high score is beaten. |
| Restart | The player presses `Space` on the result screen to restart. |
| Home return | The player presses `Esc` on the result screen to return home. |
| Pause feature | The player can pause during gameplay with `Esc` and resume with a 3-second countdown. |

---

## 14. Development Notes

The game is intentionally designed to be small and focused.

The main goal is not to build a large game engine, but to implement a clear, playable game that satisfies the requirements document. The visual theme is KAIST-specific, but the core mechanics are kept simple:

- one-button movement
- rectangular collision
- stage-based gravity changes with cumulative difficulty scaling
- probability-biased gap positioning
- score and high-score tracking
- pause functionality with resume countdown

**Window Rendering:**
The game uses a RenderTexture2D-based rendering pipeline that maintains a logical 800x600 game coordinate system while supporting arbitrary window sizes. The game is rendered to a virtual texture and then scaled to fit the window with black letterboxing to preserve aspect ratio.

**Fullscreen Feature:**
The fullscreen feature has been removed due to macOS compatibility constraints. The game window can be resized manually by the user while maintaining aspect ratio preservation.

---

## 15. Known Limitations

- The high score is stored only during the current program session.
- The game is designed for desktop execution.
- Collision uses simplified rectangular hitboxes instead of exact pixel-perfect monument shapes.
- Fullscreen mode is not available (window resizing is supported instead).

---

## 16. Troubleshooting

### `dotnet` command not found

Install the .NET 10 SDK and make sure `dotnet` is available in your terminal path.

Check installation:

```bash
dotnet --version
```

### The project does not build

Try restoring dependencies:

```bash
dotnet restore
```

Then rebuild:

```bash
dotnet build
```

### The game window does not open

Make sure you are running the project on a desktop environment with graphical window support.

If running through SSH or a headless server, the graphical window may not open.

### Assets are missing

Make sure the `Assets/` directory is present and included in the repository.

---

## 17. LLM Usage

Large language models were used during development of this project.

LLM assistance was used for:

- brainstorming the game concept
- refining the KAIST-themed Flappy Goose idea
- drafting the requirements document
- drafting this README
- discussing possible implementation structure
- reasoning about fair obstacle generation and collision design

Manual work was still required for:

- deciding the final game design
- checking that the requirements are concrete and testable
- implementing the F# code
- testing gameplay behavior
- adjusting gravity, obstacle gaps, speed, and collision boxes
- verifying that the final implementation matches the submitted requirements

The main difficulty with LLM assistance was that visual design suggestions sometimes needed correction.

---

## 18. License and Asset Notice

All source code and assets included in this repository are intended for this course project.

The game is inspired by the general mechanics of Flappy Bird, but uses original project-specific code and KAIST-themed visual assets. The project does not include copied Flappy Bird source code or original Flappy Bird assets.

---

## 19. Author

Name: `Bui Minh Hieu`

Student ID: `20240942`

Course: CS-20200 Programming Principles, Spring 2026
