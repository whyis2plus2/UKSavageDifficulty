# Savage Difficulty
A custom difficulty mod that aims to be harder than Brutal, within reason.

## Enemy Changes
### Global Changes
- 1.2x enemy damage
- 1.2x enemy speed (except for filth)
- 1.25x health (for bosses)
### Per-Enemy Changes
- Sentries
  * Now shoot 4 shots, as opposed to 2 on Brutal
  * Reload in 3 seconds
- Earthmover
  * Escape timer is 40 seconds, as opposed to 50 seconds on Brutal
- Providences
  * Now create Virtue beams

## Manual Installation
1. Download and install [BepInEx](https://thunderstore.io/c/ultrakill/p/BepInEx/BepInExPack/)
2. Download this and extract its contents to a folder in BepInEx/plugins

## Building
#### Build-system dependencies:
  - [The 7zip command line utility](https://www.7-zip.org)
  - [dotnet 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
#### Build steps:
1. Create a folder called "lib" in the root directory of the mod code
2. Add the following to it:
  - From BepInEx/core add:
    * BepInEx.dll
    * 0Harmony.dll
  - From ULTRAKILL_Data/Managed add:
    * Assembly-CSharp.dll
    * plog.dll
    * Unity.TextMeshPro.dll
    * UnityEngine.UI.dll

3. run `.\build.bat` or `./build.sh`

The output file will be `whyis2plus2-LudicrousDifficulty-0.1.0.zip`
