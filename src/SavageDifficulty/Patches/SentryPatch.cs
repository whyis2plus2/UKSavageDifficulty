namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;

public static class SentryPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "Start")]
    public static void Sentry_Start_Postfix(ref float ___maxAimTime)
    {
        if (!DifficultyHelper.Savage.isEnabled) return;
        ___maxAimTime = 3f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "StartAiming")]
    public static void Sentry_StartAiming_Postfix(ref int ___shotsInARow)
    {
        if (!DifficultyHelper.Savage.isEnabled) return;

        // add 1 extra shot in a row (for a total of 3)
        ___shotsInARow = -1;
    }
}
