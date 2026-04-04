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
        int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
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
                __instance.majorAssists = new bool[13];
            }
            if (rank.stats != null)
            {
                __instance.stats = rank.stats;
            }
            else
            {
                __instance.stats = new RankScoreData[13];
            }

            if (__instance.majorAssists.Length < 13)
            {
                var newArray = new bool[13];
                for (int i = 0; i < 13; ++i)
                {
                    if (i < __instance.majorAssists.Length) newArray[i] = __instance.majorAssists[i];
                    else newArray[i] = false;
                }
                __instance.majorAssists = newArray;
            }
            if (__instance.ranks.Length < 13)
            {
                var newArray = new int[13];
                for (int i = 0; i < 13; ++i)
                {
                    if (i < __instance.ranks.Length) newArray[i] = __instance.ranks[i];
                    else newArray[i] = -1;
                }
                __instance.ranks = newArray;
            }
            if (__instance.stats.Length < 13)
            {
                var newArray = new RankScoreData[13];
                for (int i = 0; i < __instance.stats.Length; ++i)
                {
                    newArray[i] = __instance.stats[i];
                }
                __instance.stats = newArray;
            }

            if (rank.majorAssists.Length < 13)
            {
                var newArray = new bool[13];
                for (int i = 0; i < 13; ++i)
                {
                    if (i < rank.majorAssists.Length) newArray[i] = rank.majorAssists[i];
                    else newArray[i] = false;
                }
                rank.majorAssists = newArray;
            }
            if (rank.ranks.Length < 13)
            {
                var newArray = new int[13];
                for (int i = 0; i < 13; ++i)
                {
                    if (i < rank.ranks.Length) newArray[i] = rank.ranks[i];
                    else newArray[i] = -1;
                }
                rank.ranks = newArray;
            }
            if (rank.stats.Length < 13)
            {
                var newArray = new RankScoreData[13];
                for (int i = 0; i < rank.stats.Length; ++i)
                {
                    newArray[i] = rank.stats[i];
                }
                rank.stats = newArray;
            }

            if ((sman.rankScore >= rank.ranks[@int] && (rank.majorAssists == null || (!sman.majorUsed && rank.majorAssists[@int]))) || sman.rankScore > rank.ranks[@int] || rank.levelNumber != __instance.levelNumber)
            {
                __instance.majorAssists[@int] = sman.majorUsed;
                __instance.ranks[@int] = sman.rankScore;
                if (__instance.stats[@int] == null)
                {
                    __instance.stats[@int] = new RankScoreData();
                }
                __instance.stats[@int].kills = sman.kills;
                __instance.stats[@int].style = sman.stylePoints;
                __instance.stats[@int].time = sman.seconds;
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
        __instance.ranks = new int[13];
        __instance.stats = new RankScoreData[13];
        if (__instance.stats[@int] == null)
        {
            __instance.stats[@int] = new RankScoreData();
        }
        __instance.majorAssists = new bool[13];
        for (int i = 0; i < __instance.ranks.Length; i++)
        {
            __instance.ranks[i] = -1;
        }
        __instance.ranks[@int] = sman.rankScore;
        __instance.majorAssists[@int] = sman.majorUsed;
        __instance.stats[@int].kills = sman.kills;
        __instance.stats[@int].style = sman.stylePoints;
        __instance.stats[@int].time = sman.seconds;
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