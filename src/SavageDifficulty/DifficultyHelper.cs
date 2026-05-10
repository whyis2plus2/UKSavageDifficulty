namespace SavageDifficulty;

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;

public class AlgalDifficulty
{
    public readonly string name;
    public readonly int    difficulty;
    public readonly int    baseDifficulty;

    public bool isEnabled => DifficultyHelper.CurrentDifficulty == difficulty;

    public AlgalDifficulty(string name, int difficuty, int baseDifficulty = -1)
    {
        this.name = name;
        this.difficulty = difficuty;
        
        if (baseDifficulty > 5) this.baseDifficulty = 5;
        if (baseDifficulty < 0) this.baseDifficulty = (difficuty > 5)? 5 : difficuty;
    }

    public void Enable() => DifficultyHelper.CurrentDifficulty = difficulty;

    public AngryLevelLoader.Managers.AngryDifficulty IntoAngryDifficulty()
    {
        if (!Compat.Angry.AngryLoaded) return null;
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
    public const int MAX_DIFFICULTY_VAL = 24;

    static DifficultyHelper()
    {
        KnownDifficulties.Add(Harmless);
        KnownDifficulties.Add(Lenient);
        KnownDifficulties.Add(Standard);
        KnownDifficulties.Add(Violent);
        KnownDifficulties.Add(Brutal);
        KnownDifficulties.Add(Savage);
    }
}
