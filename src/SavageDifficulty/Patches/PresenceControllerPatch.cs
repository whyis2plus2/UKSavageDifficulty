namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

public static class PresenceControllerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PresenceController), "Start")]
    public static void PresenceController_Start_Postfix(ref PresenceController __instance)
    {
        Array.Resize(ref __instance.diffNames, DifficultyHelper.MAX_DIFFICULTY_VAL + 1);
        for (int i = 5; i < DifficultyHelper.MAX_DIFFICULTY_VAL + 1; ++i)
        {
            var diff = DifficultyHelper.KnownDifficulties.Find(d => d.difficulty == i);
            if (diff != null) __instance.diffNames[i] = diff.name;
        }
    }
}
