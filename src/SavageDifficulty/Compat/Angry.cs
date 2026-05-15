namespace SavageDifficulty.Compat;

using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;

using AngryLevelLoader.Managers;

// TODO: make this difficulty agnostic
// TODO: move this with DifficultyHelper

public static class Angry
{
    public static bool IsLoaded => Chainloader.PluginInfos.ContainsKey(AngryLevelLoader.Plugin.PLUGIN_GUID);

    public static void Init()
    {
        Plugin.Instance.logger.Log(BepInEx.Logging.LogLevel.Info, $"Has angry: {IsLoaded}");
        if (!IsLoaded) return;

        Plugin.Instance.harmony.PatchAll(typeof(Patches));
    }

    public static class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AngryDifficultyManager), "Init")]
        public static void AngryDifficultyManager_Init_Postfix()
        {
            AngryDifficultyManager.difficulties.Add(Plugin.Savage.IntoAngryDifficulty());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AngryDifficultyManager), "SetDifficultyFromPrefs")]
        public static bool AngryDifficultyManager_SetDifficultyFromPrefs_Prefix()
        {
            if (!Plugin.Savage.isEnabled) return true;
            AngryDifficultyManager.SetDifficulty(Plugin.Savage.IntoAngryDifficulty());
            return false;
        }
    }
}
