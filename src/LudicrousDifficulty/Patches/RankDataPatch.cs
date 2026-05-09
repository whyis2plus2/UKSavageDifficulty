namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;

using HarmonyLib;

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
                __instance.majorAssists = new bool[Plugin.DIF_VAL + 1];
            }
            if (rank.stats != null)
            {
                __instance.stats = rank.stats;
            }
            else
            {
                __instance.stats = new RankScoreData[Plugin.DIF_VAL + 1];
            }

            if (__instance.majorAssists.Length < Plugin.DIF_VAL + 1) Array.Resize(ref __instance.majorAssists, Plugin.DIF_VAL + 1);
            if (__instance.ranks.Length < Plugin.DIF_VAL + 1) Array.Resize(ref __instance.ranks, Plugin.DIF_VAL + 1);
            if (__instance.stats.Length < Plugin.DIF_VAL + 1) Array.Resize(ref __instance.stats, Plugin.DIF_VAL + 1);

            if (rank.majorAssists.Length < Plugin.DIF_VAL + 1) Array.Resize(ref rank.majorAssists, Plugin.DIF_VAL + 1);
            if (rank.ranks.Length < Plugin.DIF_VAL + 1) Array.Resize(ref rank.ranks, Plugin.DIF_VAL + 1);
            if (rank.stats.Length < Plugin.DIF_VAL + 1) Array.Resize(ref rank.stats, Plugin.DIF_VAL + 1);

            if ((sman.rankScore >= rank.ranks[Tools.difficulty] && (rank.majorAssists == null || (!sman.majorUsed && rank.majorAssists[Tools.difficulty]))) || sman.rankScore > rank.ranks[Tools.difficulty] || rank.levelNumber != __instance.levelNumber)
            {
                __instance.majorAssists[Tools.difficulty] = sman.majorUsed;
                __instance.ranks[Tools.difficulty] = sman.rankScore;
                if (__instance.stats[Tools.difficulty] == null)
                {
                    __instance.stats[Tools.difficulty] = new RankScoreData();
                }
                __instance.stats[Tools.difficulty].kills = sman.kills;
                __instance.stats[Tools.difficulty].style = sman.stylePoints;
                __instance.stats[Tools.difficulty].time = sman.seconds;
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
        __instance.ranks = new int[Plugin.DIF_VAL + 1];
        __instance.stats = new RankScoreData[Plugin.DIF_VAL + 1];
        if (__instance.stats[Tools.difficulty] == null)
        {
            __instance.stats[Tools.difficulty] = new RankScoreData();
        }
        __instance.majorAssists = new bool[Plugin.DIF_VAL + 1];
        for (int i = 0; i < __instance.ranks.Length; i++)
        {
            __instance.ranks[i] = -1;
        }
        __instance.ranks[Tools.difficulty] = sman.rankScore;
        __instance.majorAssists[Tools.difficulty] = sman.majorUsed;
        __instance.stats[Tools.difficulty].kills = sman.kills;
        __instance.stats[Tools.difficulty].style = sman.stylePoints;
        __instance.stats[Tools.difficulty].time = sman.seconds;
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
