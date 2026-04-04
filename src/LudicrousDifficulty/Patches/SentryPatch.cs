namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;

public static class SentryPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "Start")]
    public static void Sentry_Start_Postfix(ref float ___maxAimTime)
    {
        ___maxAimTime = 3f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Turret), "StartAiming")]
    public static void Sentry_StartAiming_Postfix(ref int ___shotsInARow)
    {
        int difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty != 12) return;

        // add 2 extra shots in a row (for a total of 4)
        ___shotsInARow = -2;
    }
}