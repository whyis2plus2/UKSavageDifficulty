namespace SavageDifficulty.Compat;

using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;

public static class Angry
{
    public static bool AngryLoaded => Chainloader.PluginInfos.ContainsKey(AngryLevelLoader.Plugin.PLUGIN_GUID);

    public static void Init()
    {
        Plugin.instance.logger.Log(BepInEx.Logging.LogLevel.Info, $"Has angry: {AngryLoaded}");
        if (!AngryLoaded) return;

        Plugin.instance.harmony.PatchAll(typeof(Angry.Patches));
    }

    public static class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AngryLevelLoader.Managers.AngryDifficultyManager), "Init")]
        public static void AngryDifficultyManager_Init_Postfix(ref List<AngryLevelLoader.Managers.AngryDifficulty> ___difficulties)
        {
            ___difficulties.Add(DifficultyHelper.Savage.IntoAngryDifficulty());
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AngryLevelLoader.Managers.AngryDifficultyManager), "SetDifficultyFromPrefs")]
        public static bool AngryDifficultyManager_SetDifficultyFromPrefs_Prefix(ref List<AngryLevelLoader.Managers.AngryDifficulty> ___difficulties)
        {
            if (!DifficultyHelper.Savage.isEnabled) return true;
            AngryLevelLoader.Managers.AngryDifficultyManager.SetDifficulty(DifficultyHelper.Savage.IntoAngryDifficulty());
            return false;
        }
    }
}
