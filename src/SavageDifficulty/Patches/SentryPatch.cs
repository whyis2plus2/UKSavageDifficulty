namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;

public static class SentryPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "Start")]
    public static void Sentry_Start_Postfix(ref Turret __instance)
    {
        if (!Plugin.Savage.isEnabled) return;
        __instance.maxAimTime = 3f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "StartAiming")]
    public static void Sentry_StartAiming_Postfix(ref Turret __instance)
    {
        if (!Plugin.Savage.isEnabled) return;

        // add 1 extra shot in a row (for a total of 3)
        __instance.shotsInARow = -1;
    }
}
