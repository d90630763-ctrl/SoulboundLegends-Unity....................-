using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SoulboundLegends.Lobby
{
    public class SL_LobbySystem : MonoBehaviour
    {
        [Header("CORE")]
        public bool autoStart = true;
        public bool autoBuildUI = true;
        public bool firstTimeIntroductionEnabled = true;
        public bool playIntroductionOnce = true;
        public bool hidePremiumBeforeLevel10 = true;
        public int playerLevel = 1;
        public bool kingdomPassOwned;
        public string playerName = "Player";

        [Header("ENTRY / STARTUP")]
        public float initializationDelay = 0.15f;
        public bool checkConnection = true;
        public bool checkSaveData = true;
        public bool checkServer = true;
        public bool firstTimePlayerCheck = true;
        public UnityEvent onStartupComplete;
        public UnityEvent onStartupFailed;
        public UnityEvent onConnectionFailed;

        [Header("INTRODUCTION")]
        public VideoClip introductionVideo;
        public Sprite introductionBackground;
        public AnimationClip introductionMotion;
        public AudioClip introductionMusic;
        public AudioClip introductionSFX;
        public bool canSkipIntroduction = true;
        public float skipDelay = 1f;
        public bool autoPlayIntroduction = true;
        public UnityEvent onIntroductionStarted;
        public UnityEvent onIntroductionCompleted;
        public UnityEvent onIntroductionSkipped;

        [Header("ACCOUNT")]
        public bool googleEnabled = true;
        public bool appleEnabled = true;
        public bool emailEnabled = true;
        public bool serverAuthenticationRequired = true;
        public UnityEvent onAccountAccepted;
        public UnityEvent onAccountRejected;

        [Header("IN-GAME NAME")]
        public SL_NameValidator nameValidator;
        public bool checkNameOnInput = true;
        public bool checkNameOnConfirm = true;
        public bool nameServerValidationRequired = true;
        public UnityEvent onNameAccepted;
        public UnityEvent onNameRejected;
        public UnityEvent onNameSaved;

        [Header("CHARACTER CREATION")]
        public GameObject characterPreviewPrefab;
        public Transform characterPreviewPoint;
        public Camera characterPreviewCamera;
        public List<GameObject> bodyOptions = new List<GameObject>();
        public List<GameObject> faceOptions = new List<GameObject>();
        public List<GameObject> headOptions = new List<GameObject>();
        public List<GameObject> hairOptions = new List<GameObject>();
        public List<GameObject> hairStyleOptions = new List<GameObject>();
        public List<GameObject> bodySizeOptions = new List<GameObject>();
        public bool characterServerSaveRequired = true;
        public UnityEvent onCharacterSaved;

        [Header("3D LOBBY ENVIRONMENT")]
        public Texture2D referenceImage;
        public SL_EnvironmentGenerator environmentGenerator;
        public List<SL_EnvironmentProfile> environmentProfiles = new List<SL_EnvironmentProfile>();
        public int activeEnvironmentProfile;

        [Header("CHARACTER PRESENTATION")]
        public Transform characterDisplayPoint;
        public Transform petDisplayPoint;
        public Transform weaponDisplayPoint;
        public AnimationClip idleAnimation;
        public AnimationClip entranceAnimation;
        public bool showWeapon = true;
        public bool showEquipment = true;
        public bool showPet = true;
        public GameObject petPrefab;
        public GameObject weaponPrefab;

        [Header("CAMERAS")]
        public List<SL_CameraDefinition> cameras = new List<SL_CameraDefinition>();
        public Camera lobbyCamera;

        [Header("GAME MODES")]
        public List<SL_ModeDefinition> modes = new List<SL_ModeDefinition>();

        [Header("PANELS")]
        public List<SL_PanelDefinition> panels = new List<SL_PanelDefinition>();

        [Header("VISUAL LIBRARY")]
        public SL_VisualLibrary visualLibrary = new SL_VisualLibrary();

        [Header("LOADING SCREENS")]
        public List<SL_LoadingDefinition> loadingScreens = new List<SL_LoadingDefinition>();

        [Header("ENTRANCE EMOTES")]
        public List<SL_EntranceEmoteDefinition> entranceEmotes = new List<SL_EntranceEmoteDefinition>();

        [Header("EMOTES")]
        public List<SL_EntranceEmoteDefinition> emotes = new List<SL_EntranceEmoteDefinition>();

        [Header("SETTINGS")]
        public List<SL_SettingsCategory> settingsCategories = new List<SL_SettingsCategory>();
        public bool saveSettingsAutomatically = true;

        [Header("HUD CONNECTION")]
        public MonoBehaviour generalHUDController;
        public List<SL_HUDPreset> hudPresets = new List<SL_HUDPreset>();

        [Header("SOCIAL")]
        public MonoBehaviour friendsController;
        public MonoBehaviour partyController;
        public MonoBehaviour guildController;
        public MonoBehaviour chatController;

        [Header("CONTENT")]
        public List<SL_EventDefinition> events = new List<SL_EventDefinition>();
        public List<SL_NotificationDefinition> notificationTypes = new List<SL_NotificationDefinition>();
        public List<SL_DrawDefinition> draws = new List<SL_DrawDefinition>();
        public List<string> currencies = new List<string> { "Gold", "Diamonds", "Aether" };

        [Header("INTEGRATION")]
        public SL_SystemIntegrationManager integrationManager;
        public SL_ServerAuthorityBridge serverBridge;
        public SL_CompatibilityChecker compatibilityChecker;
        public SL_SceneLoader sceneLoader;
        public List<string> requiredSystems = new List<string>
        {
            "Character", "Inventory", "Equipment", "Skills", "Quest", "Pet", "Emote", "Social", "Party", "Guild",
            "Events", "Matchmaking", "AdventureWorld", "ClashCS", "BattleRoyale", "Siege", "Server", "Authentication"
        };

        private Canvas canvas;
        private Font font;
        private GameObject loadingPanel, introPanel, accountPanel, namePanel, characterPanel, lobbyPanel, modePanel, settingsPanel, emotePanel;
        private Text loadingText, nameErrorText, playerNameText, levelText, modeTitle, modeDescription, notificationText;
        private Image loadingFill, introImage, selectedModeImage, lobbyBackdrop;
        private InputField nameInput;
        private VideoPlayer introVideoPlayer;
        private bool introStarted;
        private float introStartedAt;
        private SL_ModeDefinition selectedMode;
        private readonly List<GameObject> dynamicObjects = new List<GameObject>();
        private readonly Dictionary<string, GameObject> panelLookup = new Dictionary<string, GameObject>();
        private GameObject presentationCharacter;

        private void Awake()
        {
            EnsureDependencies();
            LoadDefaultVisuals();
            LoadSavedPreferences();
            BuildDefaultDefinitionsIfNeeded();
            if (autoBuildUI) BuildUI();
            if (environmentGenerator != null)
            {
                environmentGenerator.referenceImage = referenceImage;
                if (environmentProfiles.Count > 0) environmentGenerator.profile = environmentProfiles[Mathf.Clamp(activeEnvironmentProfile, 0, environmentProfiles.Count - 1)];
            }
        }

        private void Start()
        {
            if (autoStart) StartCoroutine(BootSequence());
        }

        private void Update()
        {
            if (introStarted && canSkipIntroduction && Time.unscaledTime - introStartedAt >= skipDelay && Input.GetMouseButtonDown(0)) CompleteIntroduction(true);
        }

        private void EnsureDependencies()
        {
            if (nameValidator == null) nameValidator = GetComponent<SL_NameValidator>() ?? gameObject.AddComponent<SL_NameValidator>();
            if (integrationManager == null) integrationManager = GetComponent<SL_SystemIntegrationManager>() ?? gameObject.AddComponent<SL_SystemIntegrationManager>();
            if (serverBridge == null) serverBridge = GetComponent<SL_ServerAuthorityBridge>();
            if (serverBridge == null) serverBridge = gameObject.AddComponent<SL_LocalServerBridge>();
            if (sceneLoader == null) sceneLoader = GetComponent<SL_SceneLoader>() ?? gameObject.AddComponent<SL_SceneLoader>();
            if (environmentGenerator == null) environmentGenerator = GetComponent<SL_EnvironmentGenerator>() ?? gameObject.AddComponent<SL_EnvironmentGenerator>();
            if (lobbyCamera == null)
            {
                var cameraGO = new GameObject("SL Lobby Camera");
                cameraGO.transform.SetParent(transform, false);
                lobbyCamera = cameraGO.AddComponent<Camera>();
                lobbyCamera.transform.position = new Vector3(0f, 4.5f, -11f);
                lobbyCamera.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
            }
            environmentGenerator.lobbyCamera = lobbyCamera;
            if (compatibilityChecker == null) compatibilityChecker = GetComponent<SL_CompatibilityChecker>() ?? gameObject.AddComponent<SL_CompatibilityChecker>();
            if (GetComponent<SL_ReadyMadeUI>() == null) gameObject.AddComponent<SL_ReadyMadeUI>();
            compatibilityChecker.lobby = this;
            compatibilityChecker.integration = integrationManager;
            compatibilityChecker.server = serverBridge;
        }

        private void LoadDefaultVisuals()
        {
            if (visualLibrary.defaultButton == null) visualLibrary.defaultButton = LoadSprite("UI_Button");
            if (visualLibrary.defaultButtonPressed == null) visualLibrary.defaultButtonPressed = LoadSprite("UI_ButtonPressed");
            if (visualLibrary.defaultPanel == null) visualLibrary.defaultPanel = LoadSprite("UI_Panel");
            if (visualLibrary.defaultBanner == null) visualLibrary.defaultBanner = LoadSprite("UI_Banner");
            if (visualLibrary.defaultModeCard == null) visualLibrary.defaultModeCard = LoadSprite("UI_ModeCard");
            if (visualLibrary.defaultLocked == null) visualLibrary.defaultLocked = LoadSprite("UI_Locked");
            if (visualLibrary.defaultIcon == null) visualLibrary.defaultIcon = LoadSprite("UI_Icon");
            if (visualLibrary.defaultLoading == null) visualLibrary.defaultLoading = LoadSprite("UI_Loading");
            if (visualLibrary.defaultBackground == null) visualLibrary.defaultBackground = LoadSprite("UI_Background");
            if (visualLibrary.defaultNotification == null) visualLibrary.defaultNotification = LoadSprite("UI_Notification");
            if (introductionBackground == null) introductionBackground = LoadSprite("Intro_Background");
            if (referenceImage == null) referenceImage = Resources.Load<Texture2D>("LobbyReference");
            if (introductionVideo == null) introductionVideo = Resources.Load<VideoClip>("SampleIntro");
        }

        private Sprite LoadSprite(string name)
        {
            var tex = Resources.Load<Texture2D>(name);
            if (tex == null) return null;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private void BuildDefaultDefinitionsIfNeeded()
        {
            if (modes.Count == 0)
            {
                modes.Add(new SL_ModeDefinition { modeName = "Adventure World", modeID = "adventure_world", mode = SL_Mode.AdventureWorld, description = "Learn & Discover. Explore the continent, villages, kingdoms, ruins and hidden places.", sceneName = "AdventureWorld", systemID = "AdventureWorld", requirement = new SL_Requirement() });
                modes.Add(new SL_ModeDefinition { modeName = "Clash / CS", modeID = "clash_cs", mode = SL_Mode.ClashCS, description = "Practice & Sharpen. Use what you learned in Adventure World.", sceneName = "ClashCS", systemID = "ClashCS", requirement = new SL_Requirement { type = SL_RequirementType.Level, requiredLevel = 10, lockedMessage = "Reach Lv10 to unlock." } });
                modes.Add(new SL_ModeDefinition { modeName = "Battle Royale", modeID = "battle_royale", mode = SL_Mode.BattleRoyale, description = "Adapt & Survive. Compete and survive with your learned skills.", sceneName = "BattleRoyale", systemID = "BattleRoyale", requirement = new SL_Requirement { type = SL_RequirementType.Level, requiredLevel = 10, lockedMessage = "Reach Lv10 to unlock." } });
                modes.Add(new SL_ModeDefinition { modeName = "Siege", modeID = "siege", mode = SL_Mode.Siege, description = "Coordinate & Execute. Work with your team to conquer objectives.", sceneName = "Siege", systemID = "Siege", requirement = new SL_Requirement { type = SL_RequirementType.Level, requiredLevel = 10, lockedMessage = "Reach Lv10 to unlock." } });
            }
            if (loadingScreens.Count == 0) loadingScreens.Add(new SL_LoadingDefinition());
            if (environmentProfiles.Count == 0) environmentProfiles.Add(new SL_EnvironmentProfile());
        }

        private IEnumerator BootSequence()
        {
            ShowOnly(null);
            ShowLoading("Initializing Soulbound Legends...", 0.05f);
            yield return new WaitForSecondsRealtime(initializationDelay);
            ShowLoading("Checking account and saved data...", 0.25f);
            yield return null;
            if (checkConnection && serverBridge == null)
            {
                onConnectionFailed?.Invoke();
                ShowLoading("Connection unavailable. Local fallback is active.", 0.35f);
                yield return new WaitForSecondsRealtime(0.5f);
            }
            ShowLoading("Loading player data...", 0.5f);
            yield return null;
            ShowLoading("Loading lobby systems...", 0.75f);
            yield return null;
            ShowLoading("Ready", 1f);
            yield return new WaitForSecondsRealtime(0.5f);
            onStartupComplete?.Invoke();
            bool first = PlayerPrefs.GetInt("SL_IntroComplete", 0) == 0;
            bool hasName = !string.IsNullOrEmpty(PlayerPrefs.GetString("SL_PlayerName", ""));
            if (firstTimeIntroductionEnabled && first) { ShowIntroduction(); yield break; }
            ShowAccount();
        }

        private void BuildUI()
        {
            if (canvas != null) return;
            var canvasGO = new GameObject("SL_LobbyCanvas");
            canvasGO.transform.SetParent(transform, false);
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            canvasGO.GetComponent<CanvasScaler>().screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasGO.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.5f;
            canvasGO.AddComponent<GraphicRaycaster>();
            if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            BuildEntryPanels();
            BuildLobby();
            BuildSettings();
            BuildEmotes();
        }

        private GameObject Panel(string name, bool dark = true)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(SL_PanelFade));
            go.transform.SetParent(canvas.transform, false);
            var image = go.GetComponent<Image>();
            image.sprite = visualLibrary.defaultPanel;
            image.color = dark ? new Color(0.025f, 0.045f, 0.09f, 0.94f) : Color.white;
            var rt = go.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            return go;
        }

        private Text Label(Transform parent, string value, int size, TextAnchor align = TextAnchor.MiddleCenter)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var t = go.GetComponent<Text>(); t.font = font; t.text = value; t.fontSize = size; t.alignment = align; t.color = Color.white; t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Truncate;
            var rt = go.GetComponent<RectTransform>(); rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one; rt.offsetMin = new Vector2(10, 5); rt.offsetMax = new Vector2(-10, -5);
            return t;
        }

        private Button Button(Transform parent, string title, Vector2 anchorMin, Vector2 anchorMax, UnityAction action, bool selected = false)
        {
            var go = new GameObject(title, typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(parent, false);
            var rt = go.GetComponent<RectTransform>(); rt.anchorMin = anchorMin; rt.anchorMax = anchorMax; rt.offsetMin = new Vector2(4, 4); rt.offsetMax = new Vector2(-4, -4);
            var img = go.GetComponent<Image>(); img.sprite = visualLibrary.defaultButton; img.color = selected ? new Color(0.18f, 0.42f, 0.95f, 0.98f) : new Color(0.03f, 0.07f, 0.14f, 0.94f); img.type = Image.Type.Sliced;
            var b = go.GetComponent<Button>(); b.targetGraphic = img; b.onClick.AddListener(action); b.transition = Selectable.Transition.ColorTint;
            var colors = b.colors; colors.normalColor = img.color; colors.highlightedColor = new Color(0.25f, 0.5f, 1f, 1f); colors.pressedColor = new Color(0.45f, 0.75f, 1f, 1f); colors.selectedColor = colors.highlightedColor; colors.disabledColor = new Color(0.15f, 0.15f, 0.18f, 0.75f); b.colors = colors;
            var feedback = go.AddComponent<SL_ButtonFeedback>();
            feedback.clickSound = Resources.Load<AudioClip>("SampleUI");
            feedback.pressedScale = 0.96f;
            Label(go.transform, title, 26, TextAnchor.MiddleLeft);
            return b;
        }

        private void BuildEntryPanels()
        {
            loadingPanel = Panel("LoadingScreen");
            var ltitle = Label(loadingPanel.transform, "SOULBOUND LEGENDS", 42); ltitle.rectTransform.anchorMin = new Vector2(.25f,.55f); ltitle.rectTransform.anchorMax = new Vector2(.75f,.72f);
            loadingText = Label(loadingPanel.transform, "Loading...", 24); loadingText.rectTransform.anchorMin = new Vector2(.25f,.38f); loadingText.rectTransform.anchorMax = new Vector2(.75f,.46f);
            var bar = new GameObject("ProgressBar", typeof(RectTransform), typeof(Slider)); bar.transform.SetParent(loadingPanel.transform, false); var br = bar.GetComponent<RectTransform>(); br.anchorMin = new Vector2(.25f,.3f); br.anchorMax = new Vector2(.75f,.34f); br.offsetMin=br.offsetMax=Vector2.zero;
            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image)); fill.transform.SetParent(bar.transform, false); var fr=fill.GetComponent<RectTransform>(); fr.anchorMin=Vector2.zero; fr.anchorMax=Vector2.one; fr.offsetMin=fr.offsetMax=Vector2.zero; fill.GetComponent<Image>().color=new Color(0.1f,0.55f,1f); var slider=bar.GetComponent<Slider>(); slider.minValue=0; slider.maxValue=1; slider.value=0; slider.fillRect=fr; slider.interactable=false; loadingFill=fill.GetComponent<Image>();

            introPanel = Panel("OneTimeIntroduction"); introImage = introPanel.AddComponent<Image>(); introImage.sprite = introductionBackground; introImage.color = new Color(1,1,1,0.95f);
            introVideoPlayer = introPanel.AddComponent<VideoPlayer>(); introVideoPlayer.playOnAwake=false; introVideoPlayer.renderMode=VideoRenderMode.APIOnly; introVideoPlayer.isLooping=false;
            var ititle = Label(introPanel.transform, "THE STORY BEGINS", 46); ititle.rectTransform.anchorMin=new Vector2(.2f,.65f); ititle.rectTransform.anchorMax=new Vector2(.8f,.8f);
            Button(introPanel.transform,"CONTINUE",new Vector2(.55f,.08f),new Vector2(.78f,.18f),()=>CompleteIntroduction(false));
            Button(introPanel.transform,"SKIP",new Vector2(.22f,.08f),new Vector2(.45f,.18f),()=>CompleteIntroduction(true));

            accountPanel=Panel("Account"); Label(accountPanel.transform,"ACCOUNT",42).rectTransform.anchorMin=new Vector2(.35f,.72f); Label(accountPanel.transform,"ACCOUNT",42).rectTransform.anchorMax=new Vector2(.65f,.82f);
            if(googleEnabled) Button(accountPanel.transform,"CONTINUE WITH GOOGLE",new Vector2(.3f,.52f),new Vector2(.7f,.61f),AcceptAccount);
            if(appleEnabled) Button(accountPanel.transform,"CONTINUE WITH APPLE",new Vector2(.3f,.39f),new Vector2(.7f,.48f),AcceptAccount);
            if(emailEnabled) Button(accountPanel.transform,"EMAIL / REGISTER",new Vector2(.3f,.26f),new Vector2(.7f,.35f),AcceptAccount);

            namePanel=Panel("InGameName"); Label(namePanel.transform,"CREATE YOUR IN-GAME NAME",38).rectTransform.anchorMin=new Vector2(.25f,.7f); Label(namePanel.transform,"CREATE YOUR IN-GAME NAME",38).rectTransform.anchorMax=new Vector2(.75f,.8f);
            var inputGO=new GameObject("NameInput",typeof(RectTransform),typeof(Image),typeof(InputField)); inputGO.transform.SetParent(namePanel.transform,false); var ir=inputGO.GetComponent<RectTransform>(); ir.anchorMin=new Vector2(.28f,.48f); ir.anchorMax=new Vector2(.72f,.58f); ir.offsetMin=ir.offsetMax=Vector2.zero; inputGO.GetComponent<Image>().color=new Color(.08f,.12f,.2f,.98f); nameInput=inputGO.GetComponent<InputField>(); nameInput.text=playerName; nameInput.characterLimit=nameValidator.maximumNameLength; var placeholder=Label(inputGO.transform,"Enter your name",24,TextAnchor.MiddleLeft); placeholder.color=new Color(1,1,1,.45f); var text=Label(inputGO.transform,"",24,TextAnchor.MiddleLeft); nameInput.textComponent=text; nameInput.placeholder=placeholder; nameInput.onValueChanged.AddListener(OnNameInputChanged);
            nameErrorText=Label(namePanel.transform,"",22); nameErrorText.color=new Color(1,.3f,.3f); nameErrorText.rectTransform.anchorMin=new Vector2(.25f,.38f); nameErrorText.rectTransform.anchorMax=new Vector2(.75f,.45f);
            Button(namePanel.transform,"CONFIRM NAME",new Vector2(.45f,.2f),new Vector2(.72f,.3f),ConfirmName);
            Button(namePanel.transform,"RESET",new Vector2(.28f,.2f),new Vector2(.43f,.3f),()=>nameInput.text="");

            characterPanel=Panel("CharacterCreation"); Label(characterPanel.transform,"CHARACTER CREATION",40).rectTransform.anchorMin=new Vector2(.3f,.75f); Label(characterPanel.transform,"CHARACTER CREATION",40).rectTransform.anchorMax=new Vector2(.7f,.84f);
            Label(characterPanel.transform,"Male / Female • Face • Head • Hair • Body Size",24).rectTransform.anchorMin=new Vector2(.2f,.6f); Label(characterPanel.transform,"Male / Female • Face • Head • Hair • Body Size",24).rectTransform.anchorMax=new Vector2(.8f,.67f);
            Button(characterPanel.transform,"RANDOMIZE",new Vector2(.25f,.2f),new Vector2(.45f,.3f),()=>ShowMessage("Random character selected."));
            Button(characterPanel.transform,"SAVE CHARACTER",new Vector2(.55f,.2f),new Vector2(.75f,.3f),SaveCharacter);
        }

        private void BuildLobby()
        {
            lobbyPanel=Panel("Main3DLobby",false); lobbyBackdrop=lobbyPanel.GetComponent<Image>(); lobbyBackdrop.color=new Color(1,1,1,0.02f); lobbyBackdrop.raycastTarget=false;
            var top=PanelChild(lobbyPanel.transform,"TopBar",new Vector2(0,.9f),new Vector2(1,1),new Color(.02f,.04f,.08f,.9f));
            playerNameText=Label(top.transform,playerName,24,TextAnchor.MiddleLeft); playerNameText.rectTransform.anchorMin=new Vector2(.02f,.15f); playerNameText.rectTransform.anchorMax=new Vector2(.22f,.85f); levelText=Label(top.transform,"Lv. "+playerLevel,22); levelText.rectTransform.anchorMin=new Vector2(.22f,.15f); levelText.rectTransform.anchorMax=new Vector2(.30f,.85f);
            Button(top.transform,"FRIENDS",new Vector2(.75f,.15f),new Vector2(.82f,.85f),()=>ShowMessage("Friends panel connected to Social system.")); Button(top.transform,"MAIL",new Vector2(.83f,.15f),new Vector2(.89f,.85f),()=>ShowMessage("Mail panel connected.")); Button(top.transform,"SETTINGS",new Vector2(.90f,.15f),new Vector2(.98f,.85f),()=>OpenSettings());
            var left=PanelChild(lobbyPanel.transform,"LeftMenu",new Vector2(.015f,.18f),new Vector2(.18f,.88f),new Color(.02f,.04f,.08f,.80f));
            string[] leftItems={"STORE","LUCK ROYALE","CHARACTER","INVENTORY","SKILLS","EQUIPMENT","QUESTS","EVENTS"};
            for(int i=0;i<leftItems.Length;i++){int index=i; Button(left.transform,leftItems[i],new Vector2(.04f,1f-(i+1)*.11f),new Vector2(.96f,1f-i*.11f),()=>HandleLeftMenu(index));}
            var right=PanelChild(lobbyPanel.transform,"Friends",new Vector2(.82f,.45f),new Vector2(.985f,.88f),new Color(.02f,.04f,.08f,.86f)); Label(right.transform,"ONLINE (5/32)",23,TextAnchor.MiddleLeft).rectTransform.anchorMin=new Vector2(.08f,.9f); Label(right.transform,"ONLINE (5/32)",23,TextAnchor.MiddleLeft).rectTransform.anchorMax=new Vector2(.92f,.98f); string[] friends={"Zoro","Luna","Natsu","Rin","Skyler"}; for(int i=0;i<friends.Length;i++) Button(right.transform,"+  "+friends[i]+"   ONLINE",new Vector2(.05f,.78f-i*.14f),new Vector2(.95f,.89f-i*.14f),()=>ShowMessage("Party invite sent."));
            var inherit=PanelChild(lobbyPanel.transform,"Inheritance",new Vector2(.78f,.29f),new Vector2(.985f,.43f),new Color(.02f,.10f,.22f,.92f)); Label(inherit.transform,"YOUR INHERITANCE",20,TextAnchor.MiddleLeft).rectTransform.anchorMin=new Vector2(.06f,.58f); Label(inherit.transform,"Not chosen",25,TextAnchor.MiddleLeft).rectTransform.anchorMin=new Vector2(.06f,.12f); Label(inherit.transform,"Not chosen",25,TextAnchor.MiddleLeft).rectTransform.anchorMax=new Vector2(.8f,.55f);
            var bottom=PanelChild(lobbyPanel.transform,"BottomMenu",new Vector2(.015f,.02f),new Vector2(.58f,.16f),new Color(.02f,.04f,.08f,.88f)); string[] bottomItems={"VAULT","PET","COLLECTION","RANK","EMOTES"}; for(int i=0;i<bottomItems.Length;i++){int index=i; Button(bottom.transform,bottomItems[i],new Vector2(i*.2f,.08f),(new Vector2((i+1)*.2f,.92f)),()=>HandleBottomMenu(index));}
            var mode=PanelChild(lobbyPanel.transform,"ModeAndStart",new Vector2(.72f,.02f),new Vector2(.985f,.27f),new Color(.02f,.04f,.08f,.9f)); modeTitle=Label(mode.transform,"ADVENTURE WORLD",25,TextAnchor.MiddleLeft); modeTitle.rectTransform.anchorMin=new Vector2(.04f,.62f); modeTitle.rectTransform.anchorMax=new Vector2(.94f,.9f); modeDescription=Label(mode.transform,"Learn & Discover",18,TextAnchor.MiddleLeft); modeDescription.rectTransform.anchorMin=new Vector2(.04f,.42f); modeDescription.rectTransform.anchorMax=new Vector2(.94f,.64f); Button(mode.transform,"SELECT MODE",new Vector2(.04f,.08f),new Vector2(.38f,.35f),OpenModes); Button(mode.transform,"START",new Vector2(.42f,.08f),new Vector2(.96f,.35f),StartSelectedMode,true);
            Button(lobbyPanel.transform,"ENTRANCE EMOTES",new Vector2(.43f,.02f),new Vector2(.60f,.10f),OpenEmotes,true);
        }

        private GameObject PanelChild(Transform parent,string name,Vector2 min,Vector2 max,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image)); go.transform.SetParent(parent,false); var rt=go.GetComponent<RectTransform>(); rt.anchorMin=min; rt.anchorMax=max; rt.offsetMin=rt.offsetMax=Vector2.zero; go.GetComponent<Image>().color=color; go.GetComponent<Image>().sprite=visualLibrary.defaultPanel; return go;
        }

        private void BuildSettings()
        {
            settingsPanel=Panel("Settings"); Label(settingsPanel.transform,"SETTINGS",42).rectTransform.anchorMin=new Vector2(.3f,.82f); Label(settingsPanel.transform,"SETTINGS",42).rectTransform.anchorMax=new Vector2(.7f,.92f);
            string[] cats={"GRAPHICS","AUDIO","CONTROLS","HUD","GAMEPLAY","DISPLAY","ACCOUNT"}; for(int i=0;i<cats.Length;i++){string cat=cats[i]; Button(settingsPanel.transform,cat,new Vector2(.22f,.72f-i*.08f),new Vector2(.78f,.79f-i*.08f),()=>ShowMessage(cat+" settings opened."));}
            Button(settingsPanel.transform,"CLOSE",new Vector2(.42f,.08f),new Vector2(.58f,.16f),()=>ShowOnly(lobbyPanel));
        }

        private void BuildEmotes()
        {
            emotePanel=Panel("Emotes"); Label(emotePanel.transform,"EMOTES",42).rectTransform.anchorMin=new Vector2(.35f,.82f); Label(emotePanel.transform,"EMOTES",42).rectTransform.anchorMax=new Vector2(.65f,.92f);
            for(int i=0;i<6;i++){int idx=i; Button(emotePanel.transform,"EMOTE "+(i+1),new Vector2(.18f+(i%3)*.22f,.55f-(i/3)*.2f),new Vector2(.38f+(i%3)*.22f,.70f-(i/3)*.2f),()=>PlayEmote(idx));}
            Button(emotePanel.transform,"CLOSE",new Vector2(.42f,.08f),new Vector2(.58f,.16f),()=>ShowOnly(lobbyPanel));
        }

        private void ShowLoading(string message,float progress)
        {
            if(loadingPanel==null)return; ShowOnly(loadingPanel); loadingText.text=message; loadingFill.fillAmount=progress;
        }

        private void ShowIntroduction()
        {
            ShowOnly(introPanel); introStarted=true; introStartedAt=Time.unscaledTime; onIntroductionStarted?.Invoke();
            if(introductionVideo!=null){introVideoPlayer.clip=introductionVideo; introVideoPlayer.Play();}
            else StartCoroutine(IntroMotion());
        }

        private IEnumerator IntroMotion(){float t=0;while(introStarted&&t<4f){t+=Time.unscaledDeltaTime; if(introImage!=null)introImage.color=new Color(1,1,1,0.75f+Mathf.Sin(t*1.5f)*0.15f);yield return null;} }

        private void CompleteIntroduction(bool skipped)
        {
            if(!introStarted)return; introStarted=false; if(introVideoPlayer!=null)introVideoPlayer.Stop(); if(playIntroductionOnce)PlayerPrefs.SetInt("SL_IntroComplete",1); PlayerPrefs.Save(); if(skipped)onIntroductionSkipped?.Invoke(); else onIntroductionCompleted?.Invoke(); ShowAccount();
        }

        private void ShowAccount(){ShowOnly(accountPanel);}
        private void AcceptAccount(){StartCoroutine(AcceptAccountRoutine());}
        private IEnumerator AcceptAccountRoutine(){ShowLoading("Authenticating account...",.35f);bool ok=true;if(serverAuthenticationRequired&&serverBridge!=null)yield return serverBridge.ValidateAccount(v=>ok=v);if(!ok){onAccountRejected?.Invoke();ShowAccount();yield break;}onAccountAccepted?.Invoke();ShowName();}
        private void ShowName(){ShowOnly(namePanel);nameInput.text=PlayerPrefs.GetString("SL_PlayerName",playerName);nameErrorText.text="";}
        private void OnNameInputChanged(string value){if(!checkNameOnInput)return;string msg;if(!nameValidator.ValidateLocal(value,out msg))nameErrorText.text=msg;else nameErrorText.text="";}
        private void ConfirmName(){StartCoroutine(ConfirmNameRoutine());}
        private IEnumerator ConfirmNameRoutine(){string value=nameInput.text.Trim();string msg;if(checkNameOnConfirm&&!nameValidator.ValidateLocal(value,out msg)){nameErrorText.text=msg;onNameRejected?.Invoke();yield break;}nameErrorText.text="Checking name...";SL_NameValidationResult result=new SL_NameValidationResult{accepted=true};if(nameServerValidationRequired&&serverBridge!=null)yield return serverBridge.ValidateName(value,r=>result=r);if(!result.accepted){nameErrorText.text=string.IsNullOrEmpty(result.message)?"Name rejected.":result.message;onNameRejected?.Invoke();yield break;}playerName=value;PlayerPrefs.SetString("SL_PlayerName",value);PlayerPrefs.Save();nameErrorText.text="Name accepted.";onNameAccepted?.Invoke();yield return new WaitForSecondsRealtime(.25f);StartCharacterCreation();}
        private void StartCharacterCreation(){ShowOnly(characterPanel);}
        private void SaveCharacter(){StartCoroutine(SaveCharacterRoutine());}
        private IEnumerator SaveCharacterRoutine(){ShowLoading("Saving character...",.65f);yield return new WaitForSecondsRealtime(.2f);if(characterServerSaveRequired&&serverBridge!=null)yield return serverBridge.SavePlayerIdentity(playerName,ok=>{});PlayerPrefs.SetInt("SL_CharacterCreated",1);PlayerPrefs.Save();onCharacterSaved?.Invoke();ShowLobby();}

        private void ShowLobby(){ShowOnly(lobbyPanel);playerNameText.text=playerName;levelText.text="Lv. "+playerLevel; if(environmentGenerator!=null)environmentGenerator.Generate(); BuildCharacterPresentation(); SelectMode(modes[0]);}
        private void OpenModes(){modePanel=modePanel??CreateModePanel();ShowOnly(modePanel);}
        private GameObject CreateModePanel(){var p=Panel("ModeSelection");Label(p.transform,"MODE SELECTION",40).rectTransform.anchorMin=new Vector2(.3f,.84f);Label(p.transform,"MODE SELECTION",40).rectTransform.anchorMax=new Vector2(.7f,.94f);for(int i=0;i<modes.Count;i++){int idx=i;var b=Button(p.transform,modes[i].modeName,new Vector2(.2f,.72f-i*.13f),new Vector2(.8f,.82f-i*.13f),()=>SelectModeFromPanel(idx)); if(!IsUnlocked(modes[i]))b.interactable=false;}Button(p.transform,"BACK",new Vector2(.42f,.06f),new Vector2(.58f,.14f),()=>ShowOnly(lobbyPanel));return p;}
        private void SelectModeFromPanel(int index){SelectMode(modes[index]);ShowOnly(lobbyPanel);}
        private void SelectMode(SL_ModeDefinition mode){selectedMode=mode;modeTitle.text=mode.modeName;modeDescription.text=IsUnlocked(mode)?mode.description:mode.requirement.lockedMessage;}
        private bool IsUnlocked(SL_ModeDefinition mode){if(mode.requirement==null||mode.requirement.type==SL_RequirementType.None)return true;if(mode.requirement.type==SL_RequirementType.Level)return playerLevel>=mode.requirement.requiredLevel;if(mode.requirement.type==SL_RequirementType.Pass)return kingdomPassOwned;return true;}
        private void StartSelectedMode(){if(selectedMode==null)return;if(!IsUnlocked(selectedMode)){ShowMessage(selectedMode.requirement.lockedMessage);return;}StartCoroutine(StartModeRoutine(selectedMode));}
        private IEnumerator StartModeRoutine(SL_ModeDefinition mode){if(mode.serverVerification&&serverBridge!=null){bool ok=false;yield return serverBridge.ValidateUnlock(mode.systemID,playerLevel,v=>ok=v);if(!ok){ShowMessage("Server verification failed.");yield break;}}ShowLoading("Entering "+mode.modeName+"...",.3f);yield return new WaitForSecondsRealtime(.2f);if(sceneLoader!=null&&!string.IsNullOrEmpty(mode.sceneName))yield return sceneLoader.Load(mode.sceneName,p=>ShowLoading("Loading "+mode.modeName+"...",p),()=>{});}

        private void HandleLeftMenu(int index)
        {
            switch(index){
                case 0: if(hidePremiumBeforeLevel10&&playerLevel<10){ShowMessage("Store unlocks at Lv10.");return;} OpenFunctionalPanel("STORE","Store / Premium");break;
                case 1: if(playerLevel<10){ShowMessage("Draw/Luck Royale unlocks at Lv10.");return;} OpenFunctionalPanel("LUCK ROYALE","Luck Royale / Draws");break;
                case 2: OpenFunctionalPanel("CHARACTER","Character");break;
                case 3: OpenFunctionalPanel("INVENTORY","Inventory");break;
                case 4: OpenFunctionalPanel("SKILLS","Skills");break;
                case 5: OpenFunctionalPanel("EQUIPMENT","Armory / Equipment");break;
                case 6: OpenFunctionalPanel("QUESTS","Quests");break;
                case 7: OpenFunctionalPanel("EVENTS","Events");break;
            }
        }
        private void HandleBottomMenu(int index){switch(index){case 0:OpenFunctionalPanel("VAULT","Vault • 3D Item Preview");break;case 1:OpenFunctionalPanel("PET","Pet");break;case 2:OpenFunctionalPanel("COLLECTION","Collection");break;case 3:OpenFunctionalPanel("RANK","Rank");break;case 4:OpenEmotes();break;}}
        private void BuildCharacterPresentation()
        {
            if (presentationCharacter != null) Destroy(presentationCharacter);
            if (characterDisplayPoint == null)
            {
                var point = new GameObject("Character Display Point");
                point.transform.SetParent(transform, false);
                point.transform.position = Vector3.zero;
                characterDisplayPoint = point.transform;
            }
            if (characterPreviewPrefab != null)
            {
                presentationCharacter = Instantiate(characterPreviewPrefab, characterDisplayPoint.position, characterDisplayPoint.rotation, characterDisplayPoint);
            }
            else
            {
                var root = new GameObject("Character System Preview Placeholder");
                root.transform.SetParent(characterDisplayPoint, false);
                var body = GameObject.CreatePrimitive(PrimitiveType.Capsule); body.transform.SetParent(root.transform, false); body.transform.localScale = new Vector3(0.7f,1.2f,0.7f); body.GetComponent<Renderer>().material.color = new Color(0.08f,0.16f,0.28f);
                var head = GameObject.CreatePrimitive(PrimitiveType.Sphere); head.transform.SetParent(root.transform, false); head.transform.localPosition = new Vector3(0,1.8f,0); head.transform.localScale = Vector3.one*0.45f; head.GetComponent<Renderer>().material.color = new Color(0.75f,0.55f,0.42f);
                presentationCharacter = root;
            }
        }

        private void OpenFunctionalPanel(string id,string title)
        {
            GameObject p;
            if(!panelLookup.TryGetValue(id,out p) || p==null)
            {
                p=Panel("SystemPanel_"+id);
                panelLookup[id]=p;
                var titleText=Label(p.transform,title,38); titleText.rectTransform.anchorMin=new Vector2(.25f,.80f); titleText.rectTransform.anchorMax=new Vector2(.75f,.91f);
                var info=Label(p.transform,"Connected system: "+id+"\n\nThis panel is ready for the corresponding Soulbound Legends system to provide live data.\n\n3D preview and Inspector-configurable graphics are supported where applicable.",23,TextAnchor.MiddleCenter); info.rectTransform.anchorMin=new Vector2(.20f,.35f); info.rectTransform.anchorMax=new Vector2(.80f,.72f);
                if(id=="VAULT"||id=="EQUIPMENT"||id=="CHARACTER"||id=="PET")
                {
                    var preview=GameObject.CreatePrimitive(PrimitiveType.Cube); preview.name="3D Preview Display Point"; preview.transform.SetParent(p.transform,false); preview.transform.localPosition=new Vector3(0,0,0); preview.transform.localScale=new Vector3(1.2f,1.2f,1.2f); preview.GetComponent<Renderer>().material.color=new Color(.15f,.45f,.9f); Destroy(preview.GetComponent<Collider>());
                }
                Button(p.transform,"CLOSE",new Vector2(.42f,.08f),new Vector2(.58f,.16f),()=>ShowOnly(lobbyPanel));
            }
            ShowOnly(p);
        }

        private void OpenSettings(){ShowOnly(settingsPanel);}
        private void OpenEmotes(){ShowOnly(emotePanel);}
        private void PlayEmote(int index){if(index<entranceEmotes.Count&&entranceEmotes[index]!=null){var e=entranceEmotes[index];if(e.requirement!=null&&!IsRequirementMet(e.requirement)){ShowMessage(e.requirement.lockedMessage);return;}if(e.audio!=null)AudioSource.PlayClipAtPoint(e.audio,Camera.main?Camera.main.transform.position:Vector3.zero);ShowMessage("Playing "+e.name);return;}ShowMessage("Emote preview "+(index+1));}
        private bool IsRequirementMet(SL_Requirement req){if(req==null||req.type==SL_RequirementType.None)return true;if(req.type==SL_RequirementType.Level)return playerLevel>=req.requiredLevel;if(req.type==SL_RequirementType.Pass)return kingdomPassOwned;return true;}

        private void ShowMessage(string message){if(notificationText==null){var n=PanelChild(lobbyPanel.transform,"Notification",new Vector2(.30f,.86f),new Vector2(.70f,.96f),new Color(.08f,.12f,.22f,.96f));notificationText=Label(n.transform,message,22);StartCoroutine(HideNotification(n));}else notificationText.text=message;}
        private IEnumerator HideNotification(GameObject n){yield return new WaitForSecondsRealtime(2.5f);if(n!=null)Destroy(n);notificationText=null;}
        private void ShowOnly(GameObject target){var all=new[]{loadingPanel,introPanel,accountPanel,namePanel,characterPanel,lobbyPanel,modePanel,settingsPanel,emotePanel};foreach(var p in all)if(p!=null)p.SetActive(p==target);}

        private void LoadSavedPreferences(){playerName=PlayerPrefs.GetString("SL_PlayerName",playerName);playerLevel=PlayerPrefs.GetInt("SL_PlayerLevel",playerLevel);kingdomPassOwned=PlayerPrefs.GetInt("SL_KingdomPass",kingdomPassOwned?1:0)==1;}
        public void SavePreferences(){PlayerPrefs.SetString("SL_PlayerName",playerName);PlayerPrefs.SetInt("SL_PlayerLevel",playerLevel);PlayerPrefs.SetInt("SL_KingdomPass",kingdomPassOwned?1:0);PlayerPrefs.Save();}
        public void ResetLocalProgress(){PlayerPrefs.DeleteKey("SL_IntroComplete");PlayerPrefs.DeleteKey("SL_PlayerName");PlayerPrefs.DeleteKey("SL_CharacterCreated");PlayerPrefs.Save();}
    }
}
