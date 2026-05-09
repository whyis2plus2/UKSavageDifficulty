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
    public static bool DifficultyTitle_Check_Prefix(ref DifficultyTitle __instance, ref TMP_Text ___txt2)
    {
        if (Tools.difficulty != Plugin.DIF_VAL) return true;

        string text = Plugin.DIF_NAME.ToUpper();
        if (__instance.lines) text = $"-- {text} --";
        if (!___txt2) ___txt2 = __instance.GetComponent<TMP_Text>();
        ___txt2.text = text;
        return false;
    }
}
