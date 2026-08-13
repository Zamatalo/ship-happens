# Credits

The game code in `Assets/Scripts` is MIT-licensed (see [LICENSE](LICENSE)). The **art,
audio and third-party plugins under `Assets/` are not** — each carries the license of
whoever made it, and the MIT grant does not extend to them.

## Attribution required (CC-BY-4.0)

- **Shield Generator** — this work is based on
  ["Shield Generator"](https://sketchfab.com/3d-models/shield-generator-69bde44d6abe41f58240687a5f25a441)
  by [anthwolf](https://sketchfab.com/anthmurphy), licensed under
  [CC-BY-4.0](http://creativecommons.org/licenses/by/4.0/).
  (`Assets/Resources/Imports/Shield_Generator/`, license text shipped alongside the model.)

## Imported models and textures with unverified provenance

These arrived as raw imports without a license file. Most follow the Sketchfab download
layout (a `source/` folder), so each one most likely has a Sketchfab or Asset Store license
that has to be looked up and honoured — several CC-BY models require the same credit block
as the one above.

`Astronaut`, `CrashedShip`, `Enemy`, `Environment`, `Fabric`, `Glass`, `Jetpack`,
`jetpack 1`, `Large_Asteroid___Small_Moon_(FREE)`, `Loot`, `Magnet`, `Plastic`,
`RasberryPie`, `SpaceColony`, `Spear`, `StarWarsShip`, `Stone`, `Tablet`, `Wrench`,
`gravity-gun`, `handpainted-watercolor-cake`, `rope`, `spacex-water-bottle`,
`wooden-planks`.

## Third-party plugins and content packs

| Asset | Notes |
|---|---|
| `Assets/Resources/Imports/KinematicCharacterController` | Character controller package |
| `Assets/Resources/Imports/QuickOutline` | Outline shader package (see its `Readme.txt`) |
| `Assets/Ultimate 10 Plus Shaders` | Unity Asset Store package |
| `Assets/Thaleah_PixelFont`, `Assets/Resources/UI/Assets/Modern Menu 1` | Unity Asset Store packages |
| `Assets/Pixel Font - Tripfive` | Font package (see its `_README.txt`) |
| `Assets/TextMesh Pro` | Unity package, Unity Companion License |

## Known licensing issues

Two of these need a decision before the repository is used commercially or advertised
widely — they are recorded here rather than silently ignored:

1. **`StarWarsShip`** is a fan model of Lucasfilm/Disney intellectual property. Shipping it
   in a public repository is a trademark/copyright exposure regardless of who modelled it.
2. **Unity Asset Store packages** (`Ultimate 10 Plus Shaders`, `Modern Menu 1`,
   `Thaleah_PixelFont`, …): the Asset Store EULA does not permit redistributing the asset
   files themselves — only the built game. A public repository containing their sources is
   a breach of that EULA even though the game code around them is MIT.
