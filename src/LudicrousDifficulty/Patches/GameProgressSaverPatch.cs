namespace SavageDifficulty.Patches;

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

public static class GameProgressSaverPatch
{
    static MethodInfo methodInfo_GetGameProgress = typeof(GameProgressSaver).GetMethod(name: "GetGameProgress", bindingAttr: BindingFlags.Static | BindingFlags.NonPublic, types: [typeof(int)], binder: null, modifiers: null);
    static GameProgressData GetGameProgress(int difficulty = -1) =>
        (GameProgressData)methodInfo_GetGameProgress.Invoke(null, [difficulty]);

    static MethodInfo methodInfo_ReadFile = typeof(GameProgressSaver).GetMethod(name: "ReadFile", bindingAttr: BindingFlags.Static | BindingFlags.NonPublic, types: [typeof(string)], binder: null, modifiers: null);
    static object ReadFile(string path) => methodInfo_ReadFile.Invoke(null, [path]);

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetProgress")]
    public static bool GameProgressSaver_GetProgress_Prefix(ref int __result, ref int difficulty)
    {
        int levelNum = 1;
        for (int i = difficulty; i < Plugin.DIF_VAL + 1; ++i)
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
        for (int i = difficulty; i < Plugin.DIF_VAL + 1; ++i)
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
        for (int i = difficulty; i < Plugin.DIF_VAL + 1; ++i)
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

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetCyberRankData")]
    public static bool GameProgressSaver_GetCyberRankData_Prefix(ref CyberRankData __result)
    {
        var cgHighScorePath = Path.Combine(GameProgressSaver.SavePath, "cybergrindhighscore.bepis");
        var cgRankData = (CyberRankData)ReadFile(cgHighScorePath);

        if (cgRankData == null) cgRankData = new();

        if (cgRankData.preciseWavesByDifficulty == null)
        {
            cgRankData.preciseWavesByDifficulty = new float[Plugin.DIF_VAL + 1];
        }
        else if (cgRankData.preciseWavesByDifficulty.Length < Plugin.DIF_VAL + 1)
        {
           Array.Resize(ref cgRankData.preciseWavesByDifficulty, Plugin.DIF_VAL + 1);
        }

        if (cgRankData.style == null)
        {
            cgRankData.style = new int[Plugin.DIF_VAL + 1];
        }
        else if (cgRankData.style.Length < Plugin.DIF_VAL + 1)
        {
           Array.Resize(ref cgRankData.style, Plugin.DIF_VAL + 1);
        }

        if (cgRankData.kills == null)
        {
            cgRankData.kills = new int[Plugin.DIF_VAL + 1];
        }
        else if (cgRankData.kills.Length < Plugin.DIF_VAL + 1)
        {
           Array.Resize(ref cgRankData.kills, Plugin.DIF_VAL + 1);
        }

        if (cgRankData.time == null)
        {
            cgRankData.time = new float[Plugin.DIF_VAL + 1];
        }
        else if (cgRankData.time.Length < Plugin.DIF_VAL + 1)
        {
           Array.Resize(ref cgRankData.time, Plugin.DIF_VAL + 1);
        }

        __result = cgRankData;
        return false;
    }
}
