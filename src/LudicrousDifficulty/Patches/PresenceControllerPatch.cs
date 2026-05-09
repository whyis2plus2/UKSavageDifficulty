namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;

using HarmonyLib;

public static class PresenceControllerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PresenceController), "Start")]
    public static void PresenceController_Start_Postfix(ref PresenceController __instance)
    {
        if (__instance.diffNames.Length < Plugin.DIF_VAL + 1)
        {
            var newArray = new string[Plugin.DIF_VAL + 1];

            for (int i = 0; i < __instance.diffNames.Length; ++i)
                newArray[i] = __instance.diffNames[i];

            newArray[Plugin.DIF_VAL] = Plugin.DIF_NAME.ToUpper();
            __instance.diffNames = newArray;
        }
    }
}
