namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;

public static class EarthmoverTimerFix
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Countdown), "GetCountdownLength")]
    public static bool Coundown_GetCountdownLength_Prefix(ref float __result)
    {
        int difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty != 12) return true;

        __result = 40f;
        return false;
    }
}