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

## 2. Features

- Graphical game window
- KAIST-themed goose character
- KAIST monument-inspired obstacles
- Space-to-flap control
- Gravity-based movement
- Random gravity between stages
- 3-second safe transition phase between stages
- Score counter
- Stage counter
- Session high-score tracking
- Special game-over screen for a new high score
- Restart and return-to-home-screen flow

---

## 3. Requirements

This project requires:

- .NET 10 SDK
- F#
- A desktop environment capable of opening a graphical window

This project was developed for the CS-20200 Programming Principles term project and is implemented in F# with .NET 10.

---

## 4. How to Run

Clone the repository:

```bash
git clone <YOUR_REPOSITORY_URL>
cd <YOUR_REPOSITORY_NAME>
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
dotnet run --project <PROJECT_FILE_NAME>.fsproj
```

Replace `<PROJECT_FILE_NAME>.fsproj` with the actual project file name.

---

## 5. Controls

| Key | Action |
| --- | --- |
| `Space` | Flap upward during gameplay |
| `Enter` | Start the game from the home screen |
| `R` | Restart after game over |
| `H` | Return to the home screen after game over |
| `Esc` | Quit the game |

The exact key bindings are displayed inside the game window.

---

## 6. Game Screens

### 6.1 Home Screen

The home screen displays:

- Game title: **Flappy Goose**
- Current high score
- Instructions for starting the game

Press the start key to begin a new game.

### 6.2 Gameplay Screen

During gameplay, the screen displays:

- Goose character
- KAIST monument obstacles
- Current score
- Current stage number
- Background near the goose pond

The player must guide the goose through the obstacle gaps.

### 6.3 Transition Screen

After the player passes all obstacles in a stage, the game enters a transition phase.

During this phase:

- No new obstacles appear
- The goose is immortal
- A gravity-shift message is displayed
- A countdown is displayed
- The next stage's gravity is randomly selected

After approximately 3 seconds, the next stage begins.

### 6.4 Normal Game Over Screen

If the final score is not greater than the current high score, the normal game-over screen displays:

- Final score
- Current high score
- Restart instruction
- Home-screen instruction

### 6.5 New High Score Screen

If the final score is greater than the previous high score, a special game-over screen displays:

- Congratulation message
- Final score
- New high score
- Restart instruction
- Home-screen instruction

---

## 7. Gameplay Rules

1. The goose starts on the left side of the screen.
2. Gravity continuously pulls the goose downward.
3. Pressing `Space` gives the goose upward velocity.
4. Monument obstacles move from right to left.
5. Each obstacle has one open gap.
6. The player earns 1 point after successfully passing an obstacle.
7. The player loses if the goose collides with an obstacle during normal gameplay.
8. The player loses if the goose moves outside the top or bottom boundary during normal gameplay.
9. After all obstacles in a stage are passed, the game enters a transition phase.
10. During transition, the goose cannot die.
11. After transition, a new stage begins with a new gravity value.
12. The high score is updated only when the final score is greater than the previous high score.

---

## 8. High Score System

The game tracks a high score during the current program session.

- The high score starts at 0 when the program opens.
- The home screen displays the current high score.
- After each game, the final score is compared with the high score.
- If the final score is higher, the high score is updated.
- If the player returns to the home screen, the updated high score is displayed.
- Closing and reopening the program may reset the high score to 0.

The high score is intentionally session-based. It is not required to persist after the program closes.

---

## 9. Obstacle and Collision Design

The visible obstacle is a KAIST monument-inspired image.

For simpler and fairer gameplay, collision detection does not use the exact visual shape of the monument. Instead, each obstacle uses two invisible rectangular hitboxes:

1. A top rectangle above the gap
2. A bottom rectangle below the gap

The goose can safely pass through the gap between these two rectangles.

This design keeps the gameplay readable and avoids unfair collisions with decorative visual details.

---

## 10. Random Gravity and Fairness

The game uses random gravity after the first stage, but gravity is selected from a predefined safe range.

To keep the game reasonably passable:

- The obstacle gap is large enough for the goose to pass through.
- Gap positions are generated within safe vertical bounds.
- Consecutive gap positions do not move too far vertically from each other.
- Obstacle speed may increase gradually, but not suddenly.
- The transition phase gives the player time to adapt before obstacles return.

---

## 11. Project Structure

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
│   └── background.png
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

## 12. Requirement Mapping

This section explains how the implementation corresponds to the requirements document.

| Requirement Area | Implementation Behavior |
| --- | --- |
| Home screen | The game starts at a home screen showing the title and high score. |
| Player control | The player presses `Space` to flap upward. |
| Gravity | The goose is continuously pulled downward by gravity. |
| Obstacles | KAIST monument obstacles move from right to left. |
| Collision | Two invisible rectangles are used for each obstacle. |
| Scoring | Passing an obstacle increases the score by 1. |
| Stage system | After a fixed number of obstacles, the game enters transition. |
| Transition | Transition lasts approximately 3 seconds and disables losing. |
| Random gravity | A new random gravity value is selected for each new stage after stage 1. |
| Game over | Collision or leaving the screen during normal play ends the game. |
| High score | Final score is compared with the session high score. |
| New high score | A special congratulation screen appears when the high score is beaten. |
| Restart | The player can restart after game over. |
| Home return | The player can return to the home screen after game over. |

---

## 13. Development Notes

The game is intentionally designed to be small and focused.

The main goal is not to build a large game engine, but to implement a clear, playable game that satisfies the requirements document. The visual theme is KAIST-specific, but the core mechanics are kept simple:

- one-button movement
- rectangular collision
- stage-based gravity changes
- score and high-score tracking

---

## 14. Known Limitations

- The high score is stored only during the current program session.
- The game is designed for desktop execution.
- Mobile controls are not required.
- Collision uses simplified rectangular hitboxes instead of exact pixel-perfect monument shapes.
- The random gravity system uses bounded values rather than unlimited randomness.

---

## 15. Troubleshooting

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

## 16. LLM Usage

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

The main difficulty with LLM assistance was that visual design suggestions sometimes needed correction. For example, the monument obstacle design had to be refined so that all three colored monument lines had a consistent gap and equal height. The final design and implementation choices were manually reviewed and adjusted.

---

## 17. License and Asset Notice

All source code and assets included in this repository are intended for this course project.

The game is inspired by the general mechanics of Flappy Bird, but uses original project-specific code and KAIST-themed visual assets. The project does not include copied Flappy Bird source code or original Flappy Bird assets.

---

## 18. Author

Name: `<YOUR_NAME>`

Student ID: `<YOUR_STUDENT_ID>`

Course: CS-20200 Programming Principles, Spring 2026
