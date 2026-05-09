# Savage Difficulty
A custom difficulty mod that aims to be harder than Brutal, within reason.

## Enemy Changes
### Global Changes
<details>
    <ul>
        <li>Enemies do 20% more damage than Brutal</li>
        <li>Enemies are 5% faster than on Brutal (with some exceptions)</li>
        <li>Bosses have 25% more health than on Brutal</li>
        <li>Regular enemies have 5% more health than on Brutal</li>
    </ul>
</details>

### Per-Enemy Changes
<details>
    <ul>
        <li>
        <details>
        <summary>Husks</summary>
            <ul>
                <li>
                <details>
                <summary>Filth</summary>
                    <ul>
                        <li>Do not have boosted speed compared to Brutal</li>
                    </ul>
                </details>
                </li>
                <li>
                <details>
                <summary>Stray</summary>
                    <ul>
                        <li>Have a 20% speed boost as opposed to the base 5%</li>
                    </ul>
                </details>
                </li>
                <li>
                <details>
                <summary>Schism</summary>
                    <ul>
                        <li>Have a 20% speed boost as opposed to the base 5%</li>
                    </ul>
                </details>
                </li>
            </ul>
        </details>
        </li>
        <li>
        <details>
            <summary>Machines</summary>
                <ul>
                    <li>
                    <details>
                    <summary>Sentry</summary>
                        <ul>
                            <li>Shoot 3 shots between reloads (as opposed to 2 on Brutal)</li>
                            <li>Reload in 3 seconds</li>
                        </ul>
                    </details>
                    </li>
                    <li>
                    <details>
                    <summary>Earthmover</summary>
                        <ul>
                            <li>Escape timer is 40 seconds (10 shorter than on Brutal)</li>
                        </ul>
                    </details>
                    </li>
                </ul>
        </details>
        </li>
        <li>
        <details>
            <summary>Angels</summary>
                <ul>
                    <li>
                    <details>
                    <summary>Providence</summary>
                        <ul>
                            <li>Enrages when another Providence dies nearby</li>
                            <li>Shoots Virtue beams when enraged</li>
                        </ul>
                    </details>
                    </li>
                </ul>
        </details>
        </li>
        <li>
        <details>
            <summary>Others</summary>
            <ul>
                <li>
                <details>
                    <summary>Sisyphus Prime</summary>
                    <ul>
                        <li>"This will hurt" and "Destroy" are the only parriable attacks in phase 2</li>
                    </ul>
                </details>
                </li>
            </ul>
        </details>
        </li>
    </ul>
</details>

## Manual Installation
1. Download and install [BepInEx](https://thunderstore.io/c/ultrakill/p/BepInEx/BepInExPack/)
2. Download this and extract its contents to a folder in BepInEx/plugins

## Building
#### Build-system dependencies:
  - [The 7zip command line utility](https://www.7-zip.org)
  - [dotnet 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

run `.\build.bat` or `./build.sh`
The output file will be `whyis2plus2-SavageDifficulty-2026.5.10.zip`
