namespace SavageDifficulty.Patches;

using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class LevelSelectPanelPatch
{
    // for some reason using c# reflection to access this private method doesn't work
    // so i just reimplement it instead
    static void FakeSetup(ref LevelSelectPanel instance, ref LayerSelect _ls, ref Sprite _origSprite)
    {
        if (_ls == null) _ls = instance.transform.parent.GetComponent<LayerSelect>();
        if (_ls == null && instance.transform.parent.parent != null)  _ls = instance.transform.parent.parent.GetComponent<LayerSelect>();
        if (_origSprite == null) _origSprite = instance.transform.Find("Image").GetComponent<Image>().sprite;
        if (instance.unfilledPanel == null && instance.challengeIcon != null) instance.unfilledPanel = instance.challengeIcon.sprite;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(LevelSelectPanel), "CheckScore")]
    public static bool LevelSelectPanel_CheckScore_Prefix(
        ref LevelSelectPanel __instance,
        ref int ___tempInt,
        ref RectTransform ___rectTransform,
        ref string ___origName,
        ref LayerSelect ___ls,
        ref GameObject ___challengeChecker,
        ref bool ___allSecrets,
        ref Color ___defaultColor,
        ref Sprite ___origSprite
    )
    {
        FakeSetup(ref __instance, ref ___ls, ref ___origSprite);
        ___rectTransform = __instance.GetComponent<RectTransform>();
        if (__instance.levelNumber == 666)
        {
            ___tempInt = GameProgressSaver.GetPrime(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0), __instance.levelNumberInLayer);
        }
        else if (__instance.levelNumber == 100)
        {
            ___tempInt = GameProgressSaver.GetEncoreProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0));
        }
        else
        {
            ___tempInt = GameProgressSaver.GetProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0));
        }
        int num = __instance.levelNumber;
        if (__instance.levelNumber == 666 || __instance.levelNumber == 100)
        {
            num += __instance.levelNumberInLayer - 1;
        }
        ___origName = GetMissionName.GetMission(num);
        if ((__instance.levelNumber == 666 && ___tempInt == 0) || (__instance.levelNumber == 100 && ___tempInt < __instance.levelNumberInLayer - 1) || (__instance.levelNumber != 666 && __instance.levelNumber != 100 && ___tempInt < __instance.levelNumber) || __instance.forceOff)
        {
            string str = ___ls.layerNumber.ToString();
            if (___ls.layerNumber == 666)
            {
                str = "P";
            }
            if (___ls.layerNumber == 100)
            {
                __instance.transform.Find("Name").GetComponent<TMP_Text>().text = (__instance.levelNumberInLayer - 1).ToString() + "-E: ???";
            }
            else
            {
                __instance.transform.Find("Name").GetComponent<TMP_Text>().text = str + "-" + __instance.levelNumberInLayer.ToString() + ": ???";
            }
            __instance.transform.Find("Image").GetComponent<Image>().sprite = __instance.lockedSprite;
            __instance.GetComponent<Button>().enabled = false;
            ___rectTransform.sizeDelta = new Vector2(___rectTransform.sizeDelta.x, __instance.collapsedHeight);
            __instance.leaderboardPanel.SetActive(false);
        }
        else
        {
            bool flag;
            if (___tempInt == __instance.levelNumber || (__instance.levelNumber == 100 && ___tempInt == __instance.levelNumberInLayer - 1) || (__instance.levelNumber == 666 && ___tempInt == 1))
            {
                flag = false;
                __instance.transform.Find("Image").GetComponent<Image>().sprite = __instance.unlockedSprite;
                __instance.transform.Find("Name").GetComponent<TMP_Text>().text = (__instance.levelNumberInLayer - 1).ToString() + "-E: ???";
            }
            else
            {
                flag = true;
                __instance.transform.Find("Image").GetComponent<Image>().sprite = ___origSprite;
            }
            if (__instance.levelNumber != 100 || ___tempInt != __instance.levelNumberInLayer - 1)
            {
                __instance.transform.Find("Name").GetComponent<TMP_Text>().text = ___origName;
            }
            __instance.GetComponent<Button>().enabled = true;
            if (__instance.challengeIcon != null)
            {
                if (___challengeChecker == null)
                {
                    ___challengeChecker = __instance.challengeIcon.transform.Find("EventTrigger").gameObject;
                }
                if (___tempInt > __instance.levelNumber)
                {
                    ___challengeChecker.SetActive(true);
                }
            }
            if (LeaderboardController.ShowLevelLeaderboards && flag)
            {
                ___rectTransform.sizeDelta = new Vector2(___rectTransform.sizeDelta.x, __instance.expandedHeight);
                __instance.leaderboardPanel.SetActive(true);
            }
            else
            {
                ___rectTransform.sizeDelta = new Vector2(___rectTransform.sizeDelta.x, __instance.collapsedHeight);
                __instance.leaderboardPanel.SetActive(false);
            }
        }
        RankData rank = GameProgressSaver.GetRank(num, false);

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

        if (rank == null)
        {
            Debug.Log("Didn't Find Level " + __instance.levelNumber.ToString() + " Data");
            Image component = __instance.transform.Find("Stats").Find("Rank").GetComponent<Image>();
            component.color = Color.white;
            component.sprite = __instance.unfilledPanel;
            component.GetComponentInChildren<TMP_Text>().text = "";
            ___allSecrets = false;
            foreach (Image image in __instance.secretIcons)
            {
                image.enabled = true;
                image.sprite = __instance.unfilledPanel;
            }
            return false;
        }
        int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
        if (rank.levelNumber == __instance.levelNumber || ((__instance.levelNumber == 666 || __instance.levelNumber == 100) && rank.levelNumber == __instance.levelNumber + __instance.levelNumberInLayer - 1))
        {
            TMP_Text componentInChildren = __instance.transform.Find("Stats").Find("Rank").GetComponentInChildren<TMP_Text>();
            if (rank.ranks[@int] == 12 && (rank.majorAssists == null || !rank.majorAssists[@int]))
            {
                componentInChildren.text = "<color=#FFFFFF>P</color>";
                Image component2 = componentInChildren.transform.parent.GetComponent<Image>();
                component2.color = new Color(1f, 0.686f, 0f, 1f);
                component2.sprite = __instance.filledPanel;
                ___ls.AddScore(4, true);
            }
            else if (rank.majorAssists != null && rank.majorAssists[@int])
            {
                if (rank.ranks[@int] < 0)
                {
                    componentInChildren.text = "";
                }
                else
                {
                    switch (rank.ranks[@int])
                    {
                    case 1:
                        componentInChildren.text = "C";
                        ___ls.AddScore(1, false);
                        break;
                    case 2:
                        componentInChildren.text = "B";
                        ___ls.AddScore(2, false);
                        break;
                    case 3:
                        componentInChildren.text = "A";
                        ___ls.AddScore(3, false);
                        break;
                    case 4:
                    case 5:
                    case 6:
                        ___ls.AddScore(4, false);
                        componentInChildren.text = "S";
                        break;
                    default:
                        ___ls.AddScore(0, false);
                        componentInChildren.text = "D";
                        break;
                    }
                    Image component3 = componentInChildren.transform.parent.GetComponent<Image>();
                    component3.color = new Color(0.3f, 0.6f, 0.9f, 1f);
                    component3.sprite = __instance.filledPanel;
                }
            }
            else if (rank.ranks[@int] < 0)
            {
                componentInChildren.text = "";
                Image component4 = componentInChildren.transform.parent.GetComponent<Image>();
                component4.color = Color.white;
                component4.sprite = __instance.unfilledPanel;
            }
            else
            {
                switch (rank.ranks[@int])
                {
                case 1:
                    componentInChildren.text = "<color=#4CFF00>C</color>";
                    ___ls.AddScore(1, false);
                    break;
                case 2:
                    componentInChildren.text = "<color=#FFD800>B</color>";
                    ___ls.AddScore(2, false);
                    break;
                case 3:
                    componentInChildren.text = "<color=#FF6A00>A</color>";
                    ___ls.AddScore(3, false);
                    break;
                case 4:
                case 5:
                case 6:
                    ___ls.AddScore(4, false);
                    componentInChildren.text = "<color=#FF0000>S</color>";
                    break;
                default:
                    ___ls.AddScore(0, false);
                    componentInChildren.text = "<color=#0094FF>D</color>";
                    break;
                }
                Image component5 = componentInChildren.transform.parent.GetComponent<Image>();
                component5.color = Color.white;
                component5.sprite = __instance.unfilledPanel;
            }
            if (rank.secretsAmount > 0)
            {
                ___allSecrets = true;
                for (int j = 0; j < 5; j++)
                {
                    if (j < rank.secretsAmount && rank.secretsFound[j])
                    {
                        __instance.secretIcons[j].sprite = __instance.filledPanel;
                    }
                    else
                    {
                        ___allSecrets = false;
                        __instance.secretIcons[j].sprite = __instance.unfilledPanel;
                    }
                }
            }
            else
            {
                Image[] array = __instance.secretIcons;
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].enabled = false;
                }
            }
            if (__instance.challengeIcon)
            {
                if (rank.challenge)
                {
                    __instance.challengeIcon.sprite = __instance.filledPanel;
                    TMP_Text componentInChildren2 = __instance.challengeIcon.GetComponentInChildren<TMP_Text>();
                    componentInChildren2.text = "C O M P L E T E";
                    if (rank.ranks[@int] == 12 && (___allSecrets || rank.secretsAmount == 0))
                    {
                        componentInChildren2.color = new Color(0.6f, 0.4f, 0f, 1f);
                    }
                    else
                    {
                        componentInChildren2.color = Color.black;
                    }
                }
                else
                {
                    __instance.challengeIcon.sprite = __instance.unfilledPanel;
                    TMP_Text componentInChildren3 = __instance.challengeIcon.GetComponentInChildren<TMP_Text>();
                    componentInChildren3.text = "C H A L L E N G E";
                    componentInChildren3.color = Color.white;
                }
            }
        }
        else
        {
            Debug.Log("Error in finding " + __instance.levelNumber.ToString() + " Data");
            Image component6 = __instance.transform.Find("Stats").Find("Rank").GetComponent<Image>();
            component6.color = Color.white;
            component6.sprite = __instance.unfilledPanel;
            component6.GetComponentInChildren<TMP_Text>().text = "";
            ___allSecrets = false;
            foreach (Image image2 in __instance.secretIcons)
            {
                image2.enabled = true;
                image2.sprite = __instance.unfilledPanel;
            }
        }
        if ((rank.challenge || !__instance.challengeIcon) && rank.ranks[@int] == 12 && (___allSecrets || rank.secretsAmount == 0))
        {
            ___ls.Gold();
            __instance.GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 0.75f);
            return false;
        }
        __instance.GetComponent<Image>().color = ___defaultColor;
        return false;
    }
}