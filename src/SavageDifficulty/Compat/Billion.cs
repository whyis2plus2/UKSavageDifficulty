namespace SavageDifficulty.Compat;

using BepInEx.Bootstrap;

public static class Billion
{
    public const string GUID = "billy.billiondifficulty";
    public static bool IsLoaded => Chainloader.PluginInfos.ContainsKey(GUID);
}