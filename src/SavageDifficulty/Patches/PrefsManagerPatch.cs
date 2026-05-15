namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;

using HarmonyLib;

public static class PrefsManagerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PrefsManager), MethodType.Constructor)]
    public static void PrefsManager_Ctor_Postfix(ref PrefsManager __instance)
    {
        // remove the difficulty check
        __instance.propertyValidators.Remove("difficulty");
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PrefsManager), "EnsureValid")]
    public static bool PrefsManager_EnsureValid_Prefix(ref object __result, string __0, object __1)
    {
        __result = __1;
        return false;
    }
}
