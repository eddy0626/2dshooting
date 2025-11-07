# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a Unity 2D vertical scrolling shoot-em-up (슈팅) game built with Unity's Universal Render Pipeline (URP). The project uses Unity's Input System and 2D features. All game scripts are written in C# and use Korean comments.

## Project Structure

```
Assets/
├── 02.Scripts/
│   ├── Player/          # Player movement, firing, and health
│   ├── Enemy/           # Enemy AI, movement patterns, spawning
│   ├── Bullet/          # Bullet behavior and ownership system
│   ├── Item/            # Collectible items (speed, health, attack speed)
│   └── Enviroment/      # Environment elements (destroy zones)
├── 03.Prefabs/          # Reusable game object prefabs
└── Settings/            # Scene templates and project settings
```

## Core Architecture

### Ownership System
The game uses an enum-based ownership system to distinguish between player and enemy bullets:
- `BulletOwner` enum (Player, Enemy) defined in `Assets/02.Scripts/Bullet/Bullet.cs:6`
- Bullets check their owner before applying damage to prevent friendly fire

### Enemy System
Enemies are composed of multiple scripts with clear separation of concerns:
- **Enemy.cs**: Health management, death, item drops (50% drop rate: 70% speed, 20% health, 10% attack speed)
- **EnemyMoveStraight.cs**: Linear movement pattern (70% spawn probability)
- **EnemyMoveTarget.cs**: Player-tracking movement (30% spawn probability)
- **EnemyFire.cs**: Fires bullets toward player at intervals
- **EnemySpawner.cs**: Spawns enemies at random intervals

All enemy movement scripts require `Enemy` and `Rigidbody2D` components via `[RequireComponent]`.

### Player System
Player functionality is split across two scripts:
- **PlayerMove.cs**: Movement with keyboard input, boundary warping, speed adjustment (Q/E keys), Shift speed boost, R key auto-return to origin, health management (max 3 HP)
- **PlayerFire.cs**: Automatic/manual firing modes (1/2 keys), supports main gun + two auxiliary guns, uses cooldown system

### Item System
Items spawn from dead enemies and have two-phase behavior:
1. Wait 2 seconds after spawn
2. Move toward player position
3. Apply effect on collision:
   - **SpeedBoost**: Increases `PlayerMove.Speed`
   - **HealthUp**: Calls `PlayerMove.Heal()`
   - **AttackSpeedUp**: Decreases `PlayerFire.FireCooldown` (respects `MinFireCooldown`)

### Collision & Tags
The game relies heavily on Unity tags for collision detection:
- **"Player"**: Player character
- **"Enemy"**: Enemy characters
- **"Bullet"**: All bullets (ownership determined by `BulletOwner` enum)

Sorting layers: Background, Default, Foreground (defined in `ProjectSettings/TagManager.asset`)

## Building & Running

This project must be opened and built through Unity Editor:
1. Open Unity Hub
2. Open project at: `C:\Users\eddy0\2dshooting`
3. Use Unity Editor's Play button to test
4. Build via File > Build Settings

**Note**: This is a Unity project. There are no CLI commands for building outside Unity Editor.

## Important Implementation Notes

### Physics
- Use `Rigidbody2D.linearVelocity` instead of deprecated `velocity` property
- All bullets and enemies use trigger colliders (`OnTriggerEnter2D`)
- Movement speed is applied per frame using `Time.deltaTime`

### Death Prevention
Enemy scripts check `Enemy.IsDying()` before processing logic to prevent duplicate deaths. Player uses `_isDead` flag similarly.

### Boundary Handling
- **Player**: Warps to opposite side when leaving bounds (defined by minX/maxX/minY/maxY in PlayerMove.cs:23-26)
- **Bullets**: Destroyed at boundaries (±10 X, ±6 Y)
- **Enemies**: Call `Enemy.Die()` at boundaries to trigger item drops

### Bullet Acceleration
Bullets use `Mathf.Lerp` to accelerate from `InitialSpeed` to `FinalSpeed` over `AccelerationTime`. The current speed is calculated in Update using elapsed time.

## Coding Conventions

- Korean language used for comments and debug messages
- PascalCase for public fields (exposed to Unity Inspector)
- _camelCase with underscore prefix for private fields
- XML documentation comments (`///`) on all public methods
- `[Header("...")]` attributes organize Inspector properties
- RequireComponent attributes prevent missing component errors
