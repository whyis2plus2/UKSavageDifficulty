namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;

using HarmonyLib;

// TODO: separate this to put it with the difficulty helper

class RankDataPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(RankData), MethodType.Constructor, [typeof(StatsManager)])]
    public static bool RankData_Ctor_Prefix(ref RankData __instance, ref StatsManager sman)
    {
        __instance.levelNumber = sman.levelNumber;
        RankData rank = GameProgressSaver.GetRank(true, -1);
        if (rank != null)
        {
            __instance.ranks = rank.ranks;
            if (rank.majorAssists != null)
            {
                __instance.majorAssists = rank.majorAssists;
            }
            else
            {
                __instance.majorAssists = new bool[DifficultyHelper.MaxDifficulty + 1];
            }
            if (rank.stats != null)
            {
                __instance.stats = rank.stats;
            }
            else
            {
                __instance.stats = new RankScoreData[DifficultyHelper.MaxDifficulty + 1];
            }

            if (__instance.majorAssists.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref __instance.majorAssists, DifficultyHelper.MaxDifficulty + 1);
            if (__instance.ranks.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref __instance.ranks, DifficultyHelper.MaxDifficulty + 1);
            if (__instance.stats.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref __instance.stats, DifficultyHelper.MaxDifficulty + 1);

            if (rank.majorAssists.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref rank.majorAssists, DifficultyHelper.MaxDifficulty + 1);
            if (rank.ranks.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref rank.ranks, DifficultyHelper.MaxDifficulty + 1);
            if (rank.stats.Length < DifficultyHelper.MaxDifficulty + 1) Array.Resize(ref rank.stats, DifficultyHelper.MaxDifficulty + 1);

            if ((sman.rankScore >= rank.ranks[DifficultyHelper.CurrentDifficulty] && (rank.majorAssists == null || (!sman.majorUsed && rank.majorAssists[DifficultyHelper.CurrentDifficulty]))) || sman.rankScore > rank.ranks[DifficultyHelper.CurrentDifficulty] || rank.levelNumber != __instance.levelNumber)
            {
                __instance.majorAssists[DifficultyHelper.CurrentDifficulty] = sman.majorUsed;
                __instance.ranks[DifficultyHelper.CurrentDifficulty] = sman.rankScore;
                if (__instance.stats[DifficultyHelper.CurrentDifficulty] == null)
                {
                    __instance.stats[DifficultyHelper.CurrentDifficulty] = new RankScoreData();
                }
                __instance.stats[DifficultyHelper.CurrentDifficulty].kills = sman.kills;
                __instance.stats[DifficultyHelper.CurrentDifficulty].style = sman.stylePoints;
                __instance.stats[DifficultyHelper.CurrentDifficulty].time = sman.seconds;
            }
            __instance.secretsAmount = sman.secretObjects.Length;
            __instance.secretsFound = new bool[__instance.secretsAmount];
            int num = 0;
            while (num < __instance.secretsAmount && num < rank.secretsFound.Length)
            {
                if (sman.secretObjects[num] == null || rank.secretsFound[num])
                {
                    __instance.secretsFound[num] = true;
                }
                num++;
            }
            __instance.challenge = rank.challenge;
            return false;
        }
        __instance.ranks = new int[DifficultyHelper.MaxDifficulty + 1];
        __instance.stats = new RankScoreData[DifficultyHelper.MaxDifficulty + 1];
        if (__instance.stats[DifficultyHelper.CurrentDifficulty] == null)
        {
            __instance.stats[DifficultyHelper.CurrentDifficulty] = new RankScoreData();
        }
        __instance.majorAssists = new bool[DifficultyHelper.MaxDifficulty + 1];
        for (int i = 0; i < __instance.ranks.Length; i++)
        {
            __instance.ranks[i] = -1;
        }
        __instance.ranks[DifficultyHelper.CurrentDifficulty] = sman.rankScore;
        __instance.majorAssists[DifficultyHelper.CurrentDifficulty] = sman.majorUsed;
        __instance.stats[DifficultyHelper.CurrentDifficulty].kills = sman.kills;
        __instance.stats[DifficultyHelper.CurrentDifficulty].style = sman.stylePoints;
        __instance.stats[DifficultyHelper.CurrentDifficulty].time = sman.seconds;
        __instance.secretsAmount = sman.secretObjects.Length;
        __instance.secretsFound = new bool[__instance.secretsAmount];
        for (int j = 0; j < __instance.secretsAmount; j++)
        {
            if (sman.secretObjects[j] == null)
            {
                __instance.secretsFound[j] = true;
            }
        }

        return false;
    }
}
