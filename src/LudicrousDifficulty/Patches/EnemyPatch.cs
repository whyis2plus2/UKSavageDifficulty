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
        __instance.totalSpeedModifier *= __instance.enemyType switch
        {
            EnemyType.Filth  => 1f,
            EnemyType.Stray  => 1.2f,
            EnemyType.Schism => 1.2f,
            _ => 1.05f
        };

        __instance.totalHealthModifier *= __instance.isBoss? 1.25f : 1.05f;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Enemy), "InitializeDifficulty")]
    public static bool ForceDifficultyOverride(ref int __result)
    {
        int difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty != 12) return true;

        __result = 4;
        return false;
    }
}