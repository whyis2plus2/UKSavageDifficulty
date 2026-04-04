namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

public static class GameProgressSaverPatch
{
    static MethodInfo GetGameProgressMethodInfoA = typeof(GameProgressSaver).GetMethod(name: "GetGameProgress", bindingAttr: BindingFlags.Static | BindingFlags.NonPublic, types: [typeof(int)], binder: null, modifiers: null);
    static GameProgressData GetGameProgress(int difficulty = -1) =>
        (GameProgressData)GetGameProgressMethodInfoA.Invoke(null, [difficulty]);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetProgress")]
    public static bool GameProgressSaver_GetProgress_Prefix(ref int __result, ref int difficulty)
    {
        int levelNum = 1;
        for (int i = difficulty; i < 13; ++i)
        {
            var progress = GetGameProgress(i);
            if (progress != null && progress.difficulty == i && progress.levelNum > levelNum)
            {
                levelNum = progress.levelNum;
            }
        }

        __result = levelNum;
        return false; // skip original method
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetEncoreProgress")]
    public static bool GameProgressSaver_GetEncoreProgress_Prefix(ref int __result, ref int difficulty)
    {
        int levelNum = 0;
        for (int i = difficulty; i < 13; ++i)
        {
            var progress = GetGameProgress(i);
            if (progress != null && progress.difficulty == i && progress.encores > levelNum)
            {
                levelNum = progress.encores;
            }
        }

        __result = levelNum;
        return false; // skip original method
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetPrime")]
    public static bool GameProgressSaver_GetPrime_Prefix(ref int __result, ref int difficulty, ref int level)
    {
        if (SceneHelper.IsPlayingCustom)
        {
            __result = 0;
            return false;
        }

        --level;
        int levelNum = 0;
        for (int i = difficulty; i < 13; ++i)
        {
            var progress = GetGameProgress(i);
            if (progress != null && progress.difficulty == i && progress.primeLevels != null && progress.primeLevels.Length > level && progress.primeLevels[level] > levelNum)
            {
                if (progress.primeLevels[level] >= 2)
                {
                    __result = 2;
                    return false;
                }

                levelNum = progress.primeLevels[level];
            }
        }

        __result = levelNum;
        return false; // skip original method
    }
}