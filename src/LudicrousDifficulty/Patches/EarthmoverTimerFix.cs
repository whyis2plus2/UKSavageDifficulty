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
        if (Tools.difficulty != Plugin.DIF_VAL) return true;

        __result = 40f;
        return false;
    }
}
