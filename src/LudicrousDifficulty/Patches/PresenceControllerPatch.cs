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
        if (__instance.diffNames.Length < 13)
        {
            var newArray = new string[13];

            for (int i = 0; i < __instance.diffNames.Length; ++i)
                newArray[i] = __instance.diffNames[i];

            newArray[12] = Plugin.DIF_NAME.ToUpper();
            __instance.diffNames = newArray;
        }
    }
}