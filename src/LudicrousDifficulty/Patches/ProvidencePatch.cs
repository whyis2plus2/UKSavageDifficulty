namespace SavageDifficulty.Patches;

using HarmonyLib;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ProvidencePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Drone), "Shoot")]
    public static void VirtueBeams(ref Drone __instance, ref EnemyIdentifier ___eid)
    {
        var difficulty = PrefsManager.Instance.GetInt("difficulty");
        if (difficulty < 12) return;
        if (___eid.enemyType != EnemyType.Providence) return;

        GameObject insigniaObject = Object.Instantiate(Plugin.instance.virtueInsignia);
        VirtueInsignia insignia = insigniaObject.GetComponent<VirtueInsignia>();

        insignia.target = ___eid.target;
        insignia.parentEnemy = __instance.Enemy;
        insignia.hadParent = true;

        insigniaObject.SetActive(true);
    }
}