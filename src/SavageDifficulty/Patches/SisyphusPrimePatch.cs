namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine.UIElements.Collections;

public static class SisyphusPrimePatch
{
    static Dictionary<SisyphusPrime, bool> EverySisyphus = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SisyphusPrime), "Start")]
    public static void SisyphusPrime_Start_Postfix(ref SisyphusPrime __instance)
    {
        if (!Plugin.Savage.isEnabled) return;
        EverySisyphus.Add(__instance, false);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SisyphusPrime), "Death")]
    public static void SisyphusPrime_Death_Postfix(ref SisyphusPrime __instance) => EverySisyphus.Remove(__instance);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "PickPrimaryAttack", [typeof(int)])]
    public static void SisyphusPrime_PickPrimaryAttack_Prefix(ref SisyphusPrime __instance)
    {
        if (!Plugin.Savage.isEnabled) return;
        if (!__instance.enraged) return;

        EverySisyphus.Remove(__instance);
        EverySisyphus.Add(__instance, false);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "PickSecondaryAttack", [typeof(int)])]
    public static void SisyphusPrime_PickSecondaryAttack_Prefix(ref SisyphusPrime __instance)
    {
        if (!Plugin.Savage.isEnabled) return;
        if (!__instance.enraged) return;

        EverySisyphus.Remove(__instance);
        EverySisyphus.Add(__instance, true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "StompCombo")]
    public static void SisyphusPrime_StompCombo_Prefix(ref SisyphusPrime __instance)
    {
        if (!Plugin.Savage.isEnabled) return;
        if (!__instance.enraged) return;

        EverySisyphus.Remove(__instance);
        EverySisyphus.Add(__instance, true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "Parryable")]
    public static bool SisyphusPrime_Parryable_Prefix(ref SisyphusPrime __instance)
    {
        if (__instance.enraged && EverySisyphus.Get(__instance))
        {
            __instance.mach.parryable = false;
            __instance.Unparryable();
            return false;
        }

        __instance.mach.parryable = true;
        return true;
    }
}
