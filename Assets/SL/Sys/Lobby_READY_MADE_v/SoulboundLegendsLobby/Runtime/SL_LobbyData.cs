using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace SoulboundLegends.Lobby
{
    public enum SL_EntryStep { Startup, Loading, Introduction, Account, Name, CharacterCreation, SaveCharacter, Lobby, ModeSelection, Gameplay }
    public enum SL_Mode { AdventureWorld, ClashCS, BattleRoyale, Siege, Custom }
    public enum SL_DrawType { Free, Ticket, Normal, Custom, FateVault }
    public enum SL_RequirementType { None, Level, Pass, Quest, Item, Event, System, Custom }

    [Serializable] public class SL_StringEvent : UnityEvent<string> { }
    [Serializable] public class SL_IntEvent : UnityEvent<int> { }
    [Serializable] public class SL_BoolEvent : UnityEvent<bool> { }
    [Serializable] public class SL_FloatEvent : UnityEvent<float> { }

    [Serializable]
    public class SL_Requirement
    {
        public SL_RequirementType type = SL_RequirementType.None;
        public int requiredLevel;
        public string requiredID;
        public string lockedMessage = "Locked";
    }

    [Serializable]
    public class SL_ButtonVisual
    {
        public string name = "Button";
        public Sprite icon;
        public Sprite background;
        public Sprite pressedBackground;
        public Sprite lockedBackground;
        public AudioClip clickSound;
        public Color normalTint = Color.white;
        public Color selectedTint = Color.white;
        public Color lockedTint = new Color(0.55f, 0.55f, 0.55f, 1f);
        public float pressedScale = 0.96f;
    }

    [Serializable]
    public class SL_ModeDefinition
    {
        public string modeName = "Adventure World";
        public string modeID = "adventure_world";
        public SL_Mode mode = SL_Mode.AdventureWorld;
        public Sprite icon;
        public Sprite lockedIcon;
        public Sprite background;
        [TextArea] public string description = "Explore, learn and discover.";
        public string sceneName;
        public string systemID = "AdventureWorld";
        public SL_Requirement requirement = new SL_Requirement();
        public bool visibleWhenLocked = true;
        public bool serverVerification = true;
    }

    [Serializable]
    public class SL_PanelDefinition
    {
        public string panelName;
        public string panelID;
        public GameObject panelObject;
        public Sprite background;
        public bool startsOpen;
        public SL_Requirement requirement = new SL_Requirement();
        public UnityEvent onOpened;
        public UnityEvent onClosed;
    }

    [Serializable]
    public class SL_CameraDefinition
    {
        public string cameraName;
        public Camera cameraObject;
        public Transform lookTarget;
        public float fieldOfView = 55f;
        public float transitionTime = 0.5f;
        public int priority;
    }

    [Serializable]
    public class SL_EnvironmentElement
    {
        public string name;
        public GameObject prefab;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.one;
        public bool active = true;
        public bool animate;
        public Vector3 animationAxis = Vector3.up;
        public float animationSpeed = 1f;
    }

    [Serializable]
    public class SL_EnvironmentProfile
    {
        public string profileName = "Default Fantasy Lobby";
        public bool generateProceduralEnvironment = true;
        public Color skyTop = new Color(0.18f, 0.45f, 0.9f, 1f);
        public Color skyBottom = new Color(0.65f, 0.88f, 1f, 1f);
        public Color atmosphere = new Color(0.65f, 0.82f, 1f, 1f);
        public float fogDensity = 0.008f;
        public float islandDensity = 1f;
        public float castleScale = 1f;
        public List<SL_EnvironmentElement> sky = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> clouds = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> floatingIslands = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> kingdoms = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> castles = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> buildings = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> waterfalls = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> water = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> mountains = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> vegetation = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> platforms = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> bridges = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> decorations = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> particles = new List<SL_EnvironmentElement>();
        public List<SL_EnvironmentElement> specialEffects = new List<SL_EnvironmentElement>();
    }

    [Serializable]
    public class SL_LoadingDefinition
    {
        public string name = "Default Loading";
        public Sprite background;
        public VideoClip video;
        public AudioClip music;
        public List<string> tips = new List<string> { "Explore the world.", "Learn enemy weaknesses.", "Grow before you conquer." };
        public float minimumDisplayTime = 0.5f;
    }

    [Serializable]
    public class SL_EntranceEmoteDefinition
    {
        public string name;
        public string id;
        public Sprite icon;
        public AnimationClip animation;
        public AudioClip audio;
        public ParticleSystem vfx;
        public float duration = 3f;
        public bool lobbyUse = true;
        public bool gameEntryUse = true;
        public bool inGameUse = true;
        public SL_Requirement requirement = new SL_Requirement();
        public bool serverValidation = true;
    }

    [Serializable]
    public class SL_EventDefinition
    {
        public string eventName;
        public string eventID;
        public Sprite banner;
        public Sprite icon;
        public Sprite background;
        public string environmentProfile;
        public string startDate;
        public string endDate;
        public string countdownText = "Event Active";
        public SL_Requirement requirement = new SL_Requirement();
    }

    [Serializable]
    public class SL_DrawDefinition
    {
        public string drawName;
        public string drawID;
        public SL_DrawType drawType;
        public Sprite icon;
        public Sprite banner;
        public Sprite background;
        public string costType = "Ticket";
        public int cost;
        public int freeAttempts;
        public int maximumAttempts;
        public int pityAt = 0;
        public int guaranteeAt = 0;
        public float softPityStart = 0f;
        public bool serverValidation = true;
    }

    [Serializable]
    public class SL_NotificationDefinition
    {
        public string type;
        public Sprite icon;
        public AudioClip sound;
        public float duration = 3f;
        public int priority;
    }

    [Serializable]
    public class SL_SystemDefinition
    {
        public string systemName;
        public string systemID;
        public MonoBehaviour controller;
        public GameObject openingPanel;
        public SL_Requirement requirement = new SL_Requirement();
        public bool serverRequired;
        public bool initializeOnStartup;
        public bool enabled = true;
        public UnityEvent onSystemOpen;
        public UnityEvent onSystemClose;
        public UnityEvent onSystemReady;
        public UnityEvent onSystemError;
    }

    [Serializable]
    public class SL_HUDPreset
    {
        public string presetName;
        public List<RectTransform> elements = new List<RectTransform>();
    }

    [Serializable]
    public class SL_SettingsCategory
    {
        public string categoryName;
        public Sprite icon;
        public GameObject panel;
        public bool enabled = true;
    }

    [Serializable]
    public class SL_VisualLibrary
    {
        [Header("Default graphics supplied by the package")]
        public Sprite defaultButton;
        public Sprite defaultButtonPressed;
        public Sprite defaultPanel;
        public Sprite defaultBanner;
        public Sprite defaultModeCard;
        public Sprite defaultLocked;
        public Sprite defaultIcon;
        public Sprite defaultLoading;
        public Sprite defaultBackground;
        public Sprite defaultNotification;
        public List<Sprite> additionalGraphics = new List<Sprite>();
        public List<AudioClip> uiSounds = new List<AudioClip>();
        public List<AnimationClip> uiAnimations = new List<AnimationClip>();
    }
}
