namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class EnemyPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EnemyIdentifier), "UpdateModifiers")]
    public static void StatsScaling(ref EnemyIdentifier __instance)
    {
        int difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty != 12) return;

        if (__instance.enemyType is EnemyType.Idol or EnemyType.Deathcatcher) return;
        if (__instance.puppet) return;

        __instance.totalDamageModifier *= 1.2f;

        if (__instance.enemyType == EnemyType.Filth) return;
        __instance.totalSpeedModifier *= 1.2f;

        if (!__instance.isBoss) return;
        __instance.totalHealthModifier *= 1.25f;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Enemy), "InitializeDifficulty")]
    public static bool ForceDifficultyOverride(ref int __result, ref EnemyIdentifier eid)
    {
        int difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty != 12) return true;

        __result = 4;
        return false;
    }
}