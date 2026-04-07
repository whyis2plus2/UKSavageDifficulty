# Savage Difficulty
A custom difficulty mod that aims to be harder than Brutal, within reason.

## Enemy Changes
### Global Changes
- Enemies do 20% more damage than Brutal
- Enemies are 5% faster than on Brutal (with some exceptions)
- Bosses have 25% more health than on Brutal
- Regular enemies have 5% more health than on Brutal
### Per-Enemy Changes
- Filth
  * Do not have different speed than Brutal
- Stray/Schism
  * Have a 20% speed boost as opposed to 5% like other enemies
- Sentry
  * Now shoot 4 shots, as opposed to 2 on Brutal
  * Reload in 3 seconds
- Earthmover
  * Escape timer is 40 seconds, as opposed to 50 seconds on Brutal
- Providence
  * Now enrage when another Providence dies
  * Now create Virtue beams when enraged

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

The output file will be `whyis2plus2-SavageDifficulty-0.1.2.zip`
