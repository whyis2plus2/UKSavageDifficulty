namespace SavageDifficulty.Patches;

using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

public static class GameProgressSaverPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetProgress")]
    public static bool GameProgressSaver_GetProgress_Prefix(ref int __result, ref int difficulty)
    {
        int levelNum = 1;
        for (int i = difficulty; i < DifficultyHelper.MaxDifficulty + 1; ++i)
        {
            var progress = GameProgressSaver.GetGameProgress(i);
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
        for (int i = difficulty; i < DifficultyHelper.MaxDifficulty + 1; ++i)
        {
            var progress = GameProgressSaver.GetGameProgress(i);
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
        for (int i = difficulty; i < DifficultyHelper.MaxDifficulty + 1; ++i)
        {
            var progress = GameProgressSaver.GetGameProgress(i);
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
        var cgRankData = (CyberRankData)GameProgressSaver.ReadFile(cgHighScorePath);

        if (cgRankData == null) cgRankData = new();

        if (cgRankData.preciseWavesByDifficulty == null)
        {
            cgRankData.preciseWavesByDifficulty = new float[DifficultyHelper.MaxDifficulty + 1];
        }
        else if (cgRankData.preciseWavesByDifficulty.Length < DifficultyHelper.MaxDifficulty + 1)
        {
           Array.Resize(ref cgRankData.preciseWavesByDifficulty, DifficultyHelper.MaxDifficulty + 1);
        }

        if (cgRankData.style == null)
        {
            cgRankData.style = new int[DifficultyHelper.MaxDifficulty + 1];
        }
        else if (cgRankData.style.Length < DifficultyHelper.MaxDifficulty + 1)
        {
           Array.Resize(ref cgRankData.style, DifficultyHelper.MaxDifficulty + 1);
        }

        if (cgRankData.kills == null)
        {
            cgRankData.kills = new int[DifficultyHelper.MaxDifficulty + 1];
        }
        else if (cgRankData.kills.Length < DifficultyHelper.MaxDifficulty + 1)
        {
           Array.Resize(ref cgRankData.kills, DifficultyHelper.MaxDifficulty + 1);
        }

        if (cgRankData.time == null)
        {
            cgRankData.time = new float[DifficultyHelper.MaxDifficulty + 1];
        }
        else if (cgRankData.time.Length < DifficultyHelper.MaxDifficulty + 1)
        {
           Array.Resize(ref cgRankData.time, DifficultyHelper.MaxDifficulty + 1);
        }

        __result = cgRankData;
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameProgressSaver), "GetRank", [typeof(int), typeof(bool)])]
    public static void GameProgressSaver_GetRank_Postfix(ref RankData __result)
    {
        if (__result.majorAssists.Length < DifficultyHelper.MaxDifficulty + 1)
            Array.Resize(ref __result.majorAssists, DifficultyHelper.MaxDifficulty + 1);

        if (__result.ranks.Length < DifficultyHelper.MaxDifficulty + 1)
            Array.Resize(ref __result.ranks, DifficultyHelper.MaxDifficulty + 1);

        if (__result.stats.Length < DifficultyHelper.MaxDifficulty + 1)
            Array.Resize(ref __result.stats, DifficultyHelper.MaxDifficulty + 1);
    }
}
