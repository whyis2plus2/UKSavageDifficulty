namespace SavageDifficulty.Compat;

using System.Collections.Generic;
using BepInEx.Bootstrap;
using HarmonyLib;

public static class Angry
{
    public static bool angryLoaded => Chainloader.PluginInfos.ContainsKey(AngryLevelLoader.Plugin.PLUGIN_GUID);
    public static readonly AngryLevelLoader.Managers.AngryDifficulty SAVAGE = new(Plugin.DIF_NAME, Plugin.DIF_VAL);

    public static void Init()
    {
        Plugin.instance.logger.Log(BepInEx.Logging.LogLevel.Info, $"Has angry: {angryLoaded}");
        if (!angryLoaded) return;

        Plugin.instance.harmony.PatchAll(typeof(Angry.Patches));
    }

    public static class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AngryLevelLoader.Managers.AngryDifficultyManager), "Init")]
        public static void AngryDifficultyManager_Init_Postfix(ref List<AngryLevelLoader.Managers.AngryDifficulty> ___difficulties)
        {
            ___difficulties.Add(SAVAGE);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(AngryLevelLoader.Managers.AngryDifficultyManager), "SetDifficultyFromPrefs")]
        public static bool AngryDifficultyManager_SetDifficultyFromPrefs_Prefix(ref List<AngryLevelLoader.Managers.AngryDifficulty> ___difficulties)
        {
            if (Tools.difficulty != Plugin.DIF_VAL) return true;
            AngryLevelLoader.Managers.AngryDifficultyManager.SetDifficulty(SAVAGE);
            return false;
        }
    }
}
