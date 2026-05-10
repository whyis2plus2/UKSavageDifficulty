namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;

public static class DifficultyTitlePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(DifficultyTitle), "Check")]
    public static bool DifficultyTitle_Check_Prefix(ref DifficultyTitle __instance)
    {
        if (!DifficultyHelper.Savage.isEnabled) return true;

        string text = DifficultyHelper.Savage.name;
        if (__instance.lines) text = $"-- {text} --";
        if (!__instance.txt2) __instance.txt2 = __instance.GetComponent<TMP_Text>();
        __instance.txt2.text = text;
        return false;
    }
}
