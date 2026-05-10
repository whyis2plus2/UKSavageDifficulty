namespace SavageDifficulty;

using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using static BepInEx.BepInDependency;

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity;
using UnityEngine.AddressableAssets;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
[BepInDependency("Hydraxous.ULTRAKILL.EasyPZ", DependencyFlags.SoftDependency)] // the game crashes w/o this if EasyPZ is enabled
[BepInDependency(AngryLevelLoader.Plugin.PLUGIN_GUID, DependencyFlags.SoftDependency)]
public class Plugin : BaseUnityPlugin
{
    // angry level loader does this, and I quite like it
    public const string PLUGIN_GUID = "com.whyis2plus2.SavageDifficulty";
    public const string PLUGIN_NAME = "SavageDifficulty";
    public const string PLUGIN_VERSION = "0.3.0";

    /// <summary> The current instance of the plugin, accessable by all parts of the code </summary>
    public static Plugin instance;

    /// <summary> The "interactable" components of the difficulty select menu (mostly just difficulty buttons and infos) </summary>
    public Transform interactables {private set; get;}

    /// <summary> Easy and convenient variable for accessing the Canvas </summary>
    public Transform canvas {private set; get;}

    public GameObject difficultyButton = null;
    public GameObject difficultyInfo = null;

    /// <summary> Public version of the Logger so that the rest of the mod can acess it </summary>
    public ManualLogSource logger => Logger;

    /// <summary> We need to have an instance of this in order to do patches </summary>
    public readonly Harmony harmony = new(PLUGIN_GUID);


    // useful prefabs
    public GameObject homingProjectile;
    public GameObject providenceProjectile;
    public GameObject virtueInsignia;
    public GameObject enrageEffect;

    static bool addressablesInit = false;
    T LoadAsset<T>(string path)
    {
        if (!addressablesInit)
        {
            Addressables.InitializeAsync().WaitForCompletion();
            addressablesInit = true;
        }

        return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
    }

    void Awake()
    {
        instance = this;
        SceneManager.activeSceneChanged += (_, _) => OnSceneChange();
        
        // load prefabls
        homingProjectile = LoadAsset<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Homing.prefab");
        providenceProjectile = LoadAsset<GameObject>("Assets/Prefabs/Attacks and Projectiles/Projectile Providence.prefab");
        virtueInsignia = LoadAsset<GameObject>("f53d12327d16b8c4cb8c0ddd759db126");
        enrageEffect = LoadAsset<GameObject>("Assets/Particles/Enemies/RageEffect.prefab");

        // load core patches
        harmony.PatchAll(typeof(Patches.PresenceControllerPatch));
        harmony.PatchAll(typeof(Patches.DifficultyTitlePatch));
        harmony.PatchAll(typeof(Patches.PrefsManagerPatch));
        harmony.PatchAll(typeof(Patches.RankDataPatch));
        harmony.PatchAll(typeof(Patches.GameProgressSaverPatch));

        // load enemy patches
        harmony.PatchAll(typeof(Patches.EnemyPatch));
        harmony.PatchAll(typeof(Patches.ProvidencePatch));
        harmony.PatchAll(typeof(Patches.SentryPatch));
        harmony.PatchAll(typeof(Patches.EarthmoverTimerFix));
        harmony.PatchAll(typeof(Patches.SisyphusPrimePatch));

        // handle compat
        Compat.Angry.Init();

        logger.LogInfo($"Loaded {PLUGIN_NAME}");
    }

    void OnSceneChange()
    {
        // LeaderboardProperties.Difficulties[5] = DIF_NAME;
        if (SceneHelper.CurrentScene != "Main Menu") return;

        canvas = (from obj in SceneManager.GetActiveScene().GetRootGameObjects() where obj.name == "Canvas" select obj).First().transform;

        // difficulty buttons and difficulty infos
        interactables = canvas.Find("Difficulty Select (1)/Interactables");

        // create the new UKMD button and Info
        AddInfo();
        AddButton();
    }

    /// <summary> Add the UKMD button and info to the difficulty select menu </summary>
    void AddButton()
    {
        KeyValuePair<string, GameObject> FindElem(string name) =>
            new(name, interactables.Find(name).gameObject);

        logger.LogInfo("Adding difficulty button...");

        Dictionary<string, GameObject> buttons = new([
            FindElem("Casual Easy"), // Harmless
            FindElem("Casual Hard"), // Lenient
            FindElem("Standard"),
            FindElem("Violent"),
            FindElem("Brutal"),
            FindElem("V1 Must Die"), // Real UKMD button
        ]);

        Dictionary<string, GameObject> infos = new([
            FindElem("Harmless Info"),
            FindElem("Lenient Info"),
            FindElem("Standard Info"),
            FindElem("Violent Info"),
            FindElem("Brutal Info"),
        ]);

        // clone the brutal button
        difficultyButton = Instantiate(buttons.GetValueSafe("Brutal"), interactables);
        difficultyButton.GetComponent<DifficultySelectButton>().difficulty = 12;
        difficultyButton.transform.Find("Name").GetComponent<TMP_Text>().text = DifficultyHelper.Savage.name;
        difficultyButton.transform.position = buttons.GetValueSafe("V1 Must Die").transform.position;
        difficultyButton.transform.position = new(difficultyButton.transform.position.x + 600, difficultyButton.transform.position.y, difficultyButton.transform.position.z);
        difficultyButton.name = $"{DifficultyHelper.Savage.name}";

        // disable the original ukmd button so that it doesn't get in the way
        buttons.GetValueSafe("V1 Must Die").gameObject.SetActive(false);

        // the event triggers that the button uses to show/hide its description
        var buttonTrigger = difficultyButton.GetComponent<EventTrigger>();

        // remove old triggers because those use Brutal's description instead of UKMD's description
        buttonTrigger.triggers.Clear();

        // If the info hasn't been created yet, try to create it
        if (!difficultyInfo) AddInfo();

        // hide ukmd info if any of the other buttons are hovered over
        foreach (var button in buttons.Values) {
            var trigger = button.GetComponent<EventTrigger>();
            if (!trigger) continue;

            trigger.triggers.Add(
                Tools.CreateTriggerEntry(EventTriggerType.PointerEnter, _ => difficultyInfo.SetActive(false))
            );
        }

        // add new triggers to ukmd button
        buttonTrigger.triggers.AddRange([
            Tools.CreateTriggerEntry(EventTriggerType.PointerEnter, _ =>
            {
                difficultyInfo.SetActive(true);
                foreach (var info in infos.Values) info.SetActive(false);
            }),

            Tools.CreateTriggerEntry(EventTriggerType.PointerExit,  _ => difficultyInfo.SetActive(false)),
            Tools.CreateTriggerEntry(EventTriggerType.PointerClick, eventData =>
            {
                PrefsManager.Instance.SetInt("difficulty", 12);
                difficultyInfo.SetActive(false);
            }),
        ]);

        // add button to the button activation sequence
        var activationSequence = interactables.GetComponent<ObjectActivateInSequence>();
        activationSequence.objectsToActivate = activationSequence.objectsToActivate.AddItem(difficultyButton).ToArray();

        logger.LogInfo("Added difficulty button");
    }

    void AddInfo()
    {
        logger.LogInfo("Adding difficulty Info...");

        difficultyInfo = Instantiate(interactables.Find("Brutal Info").gameObject, interactables);
        difficultyInfo.name = $"{DifficultyHelper.Savage.name} Info";

        var difficultyTitle = difficultyInfo.transform.Find("Title (1)").GetComponent<TMP_Text>();

        difficultyTitle.text = $"--{DifficultyHelper.Savage.name}--";

        // set the description of difficulty
        difficultyInfo.transform.Find("Text").GetComponent<TMP_Text>().text = 
            """
            <color=white>Extremely agressive enemies and higher damage.
            
            A full arsenal and extensive knowledge of the game are expected. Every small mistake could be a fatal error.</color>

            <b>Recommended for those who are used to the difficulty of Brutal and are looking for a new challenge.</b>
            """;

        logger.LogInfo($"Added difficulty Info");
    }
}
