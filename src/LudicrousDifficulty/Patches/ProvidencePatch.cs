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
    public static void Providence_Enrage_Postfix(ref Drone __instance, ref EnemyIdentifier ___eid)
    {
        if (___eid.enemyType != EnemyType.Providence) return;
        __instance.currentEnrageEffect.transform.localScale *= 4;

        var provGameObject = __instance.transform.Find("Providence").gameObject;
        if (provGameObject == null) return;

        var primaryWings = provGameObject.transform.Find("Primary Wings").gameObject;
        if (primaryWings == null) return;
        primaryWings.GetComponent<SkinnedMeshRenderer>().material.color = new(1f, 0f, 0f);

        var secondaryWings = provGameObject.transform.Find("SecondaryWings").gameObject;
        if (secondaryWings == null) return;
        secondaryWings.GetComponent<SkinnedMeshRenderer>().material.color = new(1f, 0.5f, 0f);

        var bigRainbow = provGameObject.transform.Find("Rainbow_Large").gameObject;
        if (!bigRainbow) return;
        bigRainbow.SetActive(false);

        var smallRainbow = provGameObject.transform.Find("Rainbow_Small").gameObject;
        if (!smallRainbow) return;
        smallRainbow.SetActive(false);

        var backLight = provGameObject.transform.Find("Plane").gameObject;
        if (!backLight) return;
        backLight.GetComponent<MeshRenderer>().material.color = new(1f, 0.5f, 0f);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Drone), "Death")]
    public static void EnrageOthersOnDeath(ref Drone __instance, ref EnemyIdentifier ___eid, ref GoreZone ___gz)
    {
        if (Tools.difficulty != Plugin.DIF_VAL) return;
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
        if (Tools.difficulty != Plugin.DIF_VAL) return;
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
