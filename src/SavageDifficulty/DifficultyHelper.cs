namespace SavageDifficulty;

using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AlgalDifficulty
{
    public readonly string name;
    public readonly int difficulty;
    public readonly int baseDifficulty;
    public string info = null;

    public bool isEnabled => DifficultyHelper.CurrentDifficulty == difficulty;

    public AlgalDifficulty(string name, int difficulty, int? baseDifficulty = null)
    {
        this.name = name.Trim();
        this.difficulty = difficulty;
        this.baseDifficulty = (baseDifficulty == null)? difficulty : baseDifficulty.Value;
    }

    public void Enable() => DifficultyHelper.CurrentDifficulty = difficulty;

    public AngryLevelLoader.Managers.AngryDifficulty IntoAngryDifficulty()
    {
        if (!Compat.Angry.IsLoaded) return null;
        return new(name, difficulty);
    }
}

public static class DifficultyHelper
{
    public static readonly AlgalDifficulty Harmless = new("HARMLESS", 0);
    public static readonly AlgalDifficulty Lenient  = new("LENIENT",  1);
    public static readonly AlgalDifficulty Standard = new("STANDARD", 2);
    public static readonly AlgalDifficulty Violent  = new("VIOLENT",  3);
    public static readonly AlgalDifficulty Brutal   = new("BRUTAL",   4);
 
    public static int CurrentDifficulty
    {
        set => PrefsManager.Instance.SetInt("difficulty", value);
        get => PrefsManager.Instance.GetInt("difficulty", -1);
    }

    public static bool IsCustom => CurrentDifficulty > 5;

    private static List<AlgalDifficulty> KnownDifficulties = [];
    public static int MaxDifficulty {get; private set;} = 5;

    public static class Patches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DifficultyTitle), "Check")]
        public static bool DifficultyTitle_Check_Prefix(ref DifficultyTitle __instance)
        {
            if (!IsCustom) return true;
            var dif = GetDifficultyByIntVal(CurrentDifficulty);
            if (dif == null) return true;

            string text = dif.name;
            if (__instance.lines) text = $"-- {text} --";
            if (!__instance.txt2) __instance.txt2 = __instance.GetComponent<TMP_Text>();
            __instance.txt2.text = text;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PrefsManager), MethodType.Constructor)]
        public static void PrefsManager_Ctor_Postfix(ref PrefsManager __instance)
        {
            // remove the difficulty check
            __instance.propertyValidators.Remove("difficulty");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PrefsManager), "EnsureValid")]
        public static void PrefsManager_EnsureValid_Postfix(ref object __result, string __0, object __1)
        {
            __result = __1;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PresenceController), "Start")]
        public static void PresenceController_Start_Postfix(ref PresenceController __instance)
        {
            Array.Resize(ref __instance.diffNames, MaxDifficulty + 1);
            for (int i = 6; i < MaxDifficulty + 1; ++i)
            {
                var diff = GetDifficultyByIntVal(i);
                if (diff != null) __instance.diffNames[i] = diff.name;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Enemy), "InitializeDifficulty")]
        public static void ForceDifficultyOverride(ref int __result)
        {
            if (IsCustom) __result = GetDifficultyByIntVal(CurrentDifficulty).baseDifficulty;
        }
    }

    static DifficultyHelper()
    {
        AddDifficulty(Harmless);
        AddDifficulty(Lenient);
        AddDifficulty(Standard);
        AddDifficulty(Violent);
        AddDifficulty(Brutal);
        AddDifficulty(new("UKMD", 5));

        if (Compat.Billion.IsLoaded) AddDifficulty(new("BILLION", 20, 5));

        foreach (var d in KnownDifficulties)
            if (d.difficulty > MaxDifficulty) MaxDifficulty = d.difficulty;

        Plugin.Instance.logger.LogInfo($"Max Difficulty: {GetDifficultyByIntVal(MaxDifficulty).name} ({MaxDifficulty})");
        Plugin.Instance.harmony.PatchAll(typeof(Patches));
        Plugin.Instance.logger.LogInfo($"Applied difficulty patches");
    }

    public static AlgalDifficulty GetDifficultyByName(string name)
    {
        var idx = KnownDifficulties.FindIndex(d => d.name == name);
        return (idx < 0)? null : KnownDifficulties[idx];
    }

    public static AlgalDifficulty GetDifficultyByIntVal(int difficulty)
    {
        var idx = KnownDifficulties.FindIndex(d => d.difficulty == difficulty);
        return (idx < 0)? null : KnownDifficulties[idx];
    }

    public static bool AddDifficulty(AlgalDifficulty newDifficulty)
    {
        foreach (var d in KnownDifficulties)
        {
            if (d.name == newDifficulty.name)
            {
                Plugin.Instance.logger.LogWarning($"Tried to add \"{newDifficulty.name}\" to KnownDifficulties, but name is already taken");
                return false;
            }

            if (d.difficulty == newDifficulty.difficulty)
            {
                Plugin.Instance.logger.LogWarning($"Tried to add \"{newDifficulty.name}\" to KnownDifficulties, but difficulty value ({d.difficulty}) is already taken by \"{d.name}\"");
                return false;
            }
        }

        if (MaxDifficulty < newDifficulty.difficulty) MaxDifficulty = newDifficulty.difficulty;
        KnownDifficulties.Add(newDifficulty);
        return true;
    }

    public static void CreateDifficultyButtons()
    {
        EventTrigger.Entry CreateTriggerEntry(EventTriggerType id, UnityAction<BaseEventData> call)
        {
            EventTrigger.Entry ret = new() { eventID = id };
            ret.callback.AddListener(call);
            return ret;
        }

        if (SceneHelper.CurrentScene != "Main Menu") return;
        var canvas = (from obj in SceneManager.GetActiveScene().GetRootGameObjects() where obj.name == "Canvas" select obj).First();
        if (canvas == null)
        {
            Plugin.Instance.logger.LogError("Failed to get canvas in main menu");
            return;
        }

        var interactables = canvas.transform.Find("Difficulty Select (1)/Interactables");
        if (interactables == null)
        {
            Plugin.Instance.logger.LogError("Failed to get interactables from canvas in main menu");
            return;
        }

        GameObject FindElem(string name) => interactables.Find(name).gameObject;

        GameObject[] buttons = [
            FindElem("Casual Easy"), // Harmless
            FindElem("Casual Hard"), // Lenient
            FindElem("Standard"),
            FindElem("Violent"),
            FindElem("Brutal"),
            FindElem("V1 Must Die"), // UKMD button
        ];

        GameObject[] infos = [
            FindElem("Harmless Info"),
            FindElem("Lenient Info"),
            FindElem("Standard Info"),
            FindElem("Violent Info"),
            FindElem("Brutal Info"),
        ];

        int ncols = 1;
        int nrows = 0;
        foreach (var d in KnownDifficulties)
        {
            if (d.difficulty < 6) continue; // skip Harmless thru UKMD
            if (d.difficulty == 19) continue; // skip billion difficulty

            var button = GameObject.Instantiate(buttons[0], interactables);
            button.transform.localPosition = new(
                buttons[nrows].transform.localPosition.x + 410 * ncols,
                buttons[nrows].transform.localPosition.y,
                buttons[nrows].transform.localPosition.z
            );

            button.name = d.name.ToPascalCase();
            button.transform.Find("Name").GetComponent<TMP_Text>().text = d.name;

            var activationSequence = interactables.GetComponent<ObjectActivateInSequence>();
            activationSequence.objectsToActivate = activationSequence.objectsToActivate.AddItem(button).ToArray();

            Plugin.Instance.logger.LogInfo($"Added difficulty button for \"{d.name}\" ({d.difficulty})");

            var info = GameObject.Instantiate(infos[4], interactables);
            info.transform.Find("Title (1)").GetComponent<TMP_Text>().text = $"--{d.name}--";
            info.transform.Find("Text").GetComponent<TMP_Text>().text =
                d.info.IsNullOrWhiteSpace()? "(no info was provided)" : d.info.Trim();

            var trigger = button.GetComponent<EventTrigger>();
            trigger.triggers.Clear();

            trigger.triggers.AddRange([
                CreateTriggerEntry(EventTriggerType.PointerEnter, _ =>
                {
                    info.SetActive(true);
                    foreach (var info in infos) info.SetActive(false);
                }),

                CreateTriggerEntry(EventTriggerType.PointerExit,  _ => info.SetActive(false)),
                CreateTriggerEntry(EventTriggerType.PointerClick, eventData =>
                {
                    d.Enable();
                    info.SetActive(false);
                }),
            ]);

            Plugin.Instance.logger.LogInfo($"Added difficulty info for \"{d.name}\" ({d.difficulty})");
            nrows += 1;

            if (nrows == 6)
            {
                nrows = 0;
                ++ncols;
            }
        }
    }
}
