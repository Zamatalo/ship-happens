# Ship Happens

![Version](https://img.shields.io/badge/version-v0.001-blue)
[![Play](https://img.shields.io/badge/play-Unity%20Play-black)](https://play.unity.com/en/games/e063b909-f3e3-4f11-9619-0f4bf9d328f3/ship-happens)

**▶ Play in the browser:** https://play.unity.com/en/games/e063b909-f3e3-4f11-9619-0f4bf9d328f3/ship-happens

Welcome to Ship Happens, an exciting open-world game where you find yourself stranded on an
asteroid. Your mission is to gather resources, build pathways, and navigate through the cosmos to
reach your space shuttle. Explore various planets, collect loot, and craft essential items to
survive in this thrilling space environment.

Features:

- Resource Gathering: Collect loot from asteroids and planets to craft tools and build structures.
- Building Mechanics: Create roads and platforms to connect your location to your space shuttle.
- Dynamic Environment: Experience a vibrant universe filled with unique planets and asteroids.
- Crafting System: Utilize gathered resources to craft essential items, tools, and upgrades.
- Survival Elements: Manage your health, hunger, and oxygen levels as you explore the vastness of
  space.
- Engaging Gameplay: Complete tasks and challenges to progress and unlock new features.

## Installation

The game is playable in the browser - no setup needed:
https://play.unity.com/en/games/e063b909-f3e3-4f11-9619-0f4bf9d328f3/ship-happens

To open the project itself:

1. Install **Unity 6000.1.5f1** (the version in `ProjectSettings/ProjectVersion.txt`) via
   Unity Hub. A different patch release will prompt an upgrade and may rewrite assets.
2. Clone and open the folder as a project:

```sh
$ git clone git@github.com:Zamatalo/ship-happens.git
$ cd ship-happens
```

3. Open it from Unity Hub (*Add project from disk*). Unity restores `Library/` and the
   packages listed in `Packages/manifest.json` on first import, which takes a while.

> **Heads-up on project settings.** `ProjectSettings/ProjectSettings.asset`,
> `InputManager.asset`, `TagManager.asset`, `EditorBuildSettings.asset`,
> `GraphicsSettings.asset`, `QualitySettings.asset`, `TimeManager.asset`,
> `AudioManager.asset` and `DynamicsManager`'s 2D counterpart were excluded by the old
> `.gitignore` and were never committed. `.gitignore` no longer excludes them, but the
> files themselves still have to be added from a machine that has a working copy -
> otherwise Unity regenerates them with defaults and the scene list, tags/layers and input
> bindings come up empty.

## Assets

The game includes various assets such as:

- 3D Models: Asteroids, planets, and space shuttles.
- Textures: High-quality textures for a realistic look.
- Audio Clips: Immersive sound effects and background music.

## Documentation

For detailed information on gameplay mechanics, crafting recipes, and building strategies, please
refer to the [Wiki](https://inv.nadeko.net/watch?v=dQw4w9WgXcQ). Contributing

We welcome contributions from the community! If you would like to contribute to Ship Happens, please
follow these guidelines:

- Fork the repository.
- Create a new branch for your feature or bug fix.
- Make your changes and commit them.
- Push your changes to your forked repository.
- Submit a pull request.

## License

The game code is licensed under the MIT License - see [LICENSE](LICENSE).

The art, audio and third-party plugins under `Assets/` are **not** covered by that
grant; each has its own license and required attribution. See [CREDITS.md](CREDITS.md),
which also records two known licensing issues.
