namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;

using System.Collections.Generic;

public static class SisyphusPrimePatch
{
    static List<Pair<SisyphusPrime, bool>> everySisyphus = new();

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SisyphusPrime), "Start")]
    public static void SisyphusPrime_Start_Postfix(ref SisyphusPrime __instance)
    {
        everySisyphus.Add(new(__instance, false));
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SisyphusPrime), "Death")]
    public static void SisyphusPrime_Death_Postfix(ref SisyphusPrime __instance)
    {
        int i;
        for (i = 0; i < everySisyphus.Count; ++i)
        {
            if (everySisyphus[i].first != __instance) continue;
            break;
        }

        everySisyphus.RemoveAt(i);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "PickPrimaryAttack", [typeof(int)])]
    public static void SisyphusPrime_PickPrimaryAttack_Prefix(ref SisyphusPrime __instance, ref bool ___enraged)
    {
        foreach (var sisy in everySisyphus)
        {
            if (sisy.first != __instance) continue;
            sisy.second = false;
            break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "PickSecondaryAttack", [typeof(int)])]
    public static void SisyphusPrime_PickSecondaryAttack_Prefix(ref SisyphusPrime __instance, ref bool ___enraged)
    {
        foreach (var sisy in everySisyphus)
        {
            if (sisy.first != __instance) continue;
            if (!___enraged) return;
            sisy.second = true;
            break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "StompCombo")]
    public static void SisyphusPrime_StompCombo_Prefix(ref SisyphusPrime __instance, ref bool ___enraged)
    {
        foreach (var sisy in everySisyphus)
        {
            if (sisy.first != __instance) continue;
            if (!___enraged) return;
            sisy.second = true;
            break;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SisyphusPrime), "Parryable")]
    public static bool SisyphusPrime_Parryable_Prefix(
            ref SisyphusPrime __instance,
            ref SPAttack ___lastPrimaryAttack,
            ref SPAttack ___lastSecondaryAttack,
            ref bool ___enraged,
            ref Enemy ___mach
    )
    {
        if (!___enraged)
        {
            ___mach.parryable = true;
            return true;
        }

        foreach (var sisy in everySisyphus)
        {
            if (sisy.first != __instance) continue;
            if (!sisy.second) 
            {
                ___mach.parryable = true;
                return true;
            }
            break;
        }

        __instance.Unparryable();
        return false;
    }
}
