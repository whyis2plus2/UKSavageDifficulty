namespace SavageDifficulty;

using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using HarmonyLib;

public class AlgalDifficulty
{
    public readonly string name;
    public readonly int    difficulty;
    public readonly int    baseDifficulty;

    public bool isEnabled => DifficultyHelper.CurrentDifficulty == difficulty;

    public AlgalDifficulty(string name, int difficulty, int? baseDifficulty = null)
    {
        this.name = name;
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
    public static readonly AlgalDifficulty Savage   = new("SAVAGE",  12, 4);
 
    public static int CurrentDifficulty
    {
        set => PrefsManager.Instance.SetInt("difficulty", value);
        get => PrefsManager.Instance.GetInt("difficulty", -1);
    }

    public static bool IsCustom => CurrentDifficulty > 4;

    public static List<AlgalDifficulty> KnownDifficulties = [];
    public static readonly int MaxDifficulty = 0;

    static AlgalDifficulty GetDifficultyByName(string name)
    {
        var idx = KnownDifficulties.FindIndex(d => d.name == name);
        return (idx < 0)? null : KnownDifficulties[idx];
    }

    static AlgalDifficulty GetDifficultyByIntVal(int difficulty)
    {
        var idx = KnownDifficulties.FindIndex(d => d.difficulty == difficulty);
        return (idx < 0)? null : KnownDifficulties[idx];
    }

    static DifficultyHelper()
    {
        KnownDifficulties.Add(Harmless);
        KnownDifficulties.Add(Lenient);
        KnownDifficulties.Add(Standard);
        KnownDifficulties.Add(Violent);
        KnownDifficulties.Add(Brutal);
        KnownDifficulties.Add(Savage);

        if (Compat.Billion.IsLoaded) KnownDifficulties.Add(new("BILLION", 20, 5));

        foreach (var d in KnownDifficulties)
            if (d.difficulty > MaxDifficulty) MaxDifficulty = d.difficulty;

        Plugin.Instance.logger.Log(LogLevel.Info, $"Max Difficulty: {GetDifficultyByIntVal(MaxDifficulty).name} ({MaxDifficulty})");
    }
}
