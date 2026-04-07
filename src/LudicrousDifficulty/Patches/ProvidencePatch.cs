namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ProvidencePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Drone), "Awake")]
    public static void Drone_Awake_Postfix(ref Drone __instance, ref EnemyIdentifier ___eid)
    {
        if (___eid.enemyType != EnemyType.Providence) return;
        __instance.enrageEffect = Plugin.instance.enrageEffect;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Drone), "Enrage")]
    public static void ScaleCurrentEnrageEffect(ref Drone __instance, ref EnemyIdentifier ___eid)
    {
        if (___eid.enemyType != EnemyType.Providence) return;
        __instance.currentEnrageEffect.transform.localScale *= 4;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(Drone), "Death")]
    public static void EnrageOthersOnDeath(ref Drone __instance, ref EnemyIdentifier ___eid, ref GoreZone ___gz)
    {
        if (___eid.enemyType != EnemyType.Providence) return;
        if (__instance.Enemy.health > 0f) return;

        foreach (var drone in ___gz.GetComponentsInChildren<Drone>())
        {
            if (drone == null) continue;
            if (drone.Enemy.health <= 0f) continue;
            if (drone == __instance) continue;
            if (drone.Enemy.EID.enemyType != EnemyType.Providence) continue;

            if (!drone.isEnraged) drone.Enrage();
            Plugin.instance.logger.LogInfo("Enraging providence");
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Drone), "Shoot")]
    public static void VirtueBeams(ref Drone __instance, ref EnemyIdentifier ___eid)
    {
        var difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty < 12) return;
        if (___eid.enemyType != EnemyType.Providence) return;
        if (!__instance.isEnraged) return; // don't spawn an extra virtue insignia when not enraged

        GameObject insigniaObject = Object.Instantiate(Plugin.instance.virtueInsignia);
        VirtueInsignia insignia = insigniaObject.GetComponent<VirtueInsignia>();

        insignia.target = ___eid.target;
        insignia.parentEnemy = __instance.Enemy;
        insignia.hadParent = true;

        insigniaObject.SetActive(true);
    }
}