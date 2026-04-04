namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

public static class EndlessHighScorePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EndlessHighScore), "OnEnable")]
    public static bool EndlessHighScore_OnEnable_Patch()
    {
        return PrefsManager.Instance.GetInt("difficulty") <= 5;
    }
}