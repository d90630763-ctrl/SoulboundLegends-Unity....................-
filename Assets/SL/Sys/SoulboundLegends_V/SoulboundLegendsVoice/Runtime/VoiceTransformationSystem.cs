using System.Collections;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace SoulboundLegends.Voice
{
    public class VoiceTransformationSystem : MonoBehaviour
    {
        [Header("IN-GAME SETUP")]
        [SerializeField] private bool createUIOnStart = true;
        [SerializeField] private bool showUI = true;
        [SerializeField] private string uiCanvasName = "Soulbound Voice Settings";

        [Header("VOICE TRANSFORMATION")]
        [SerializeField] private VoiceTransformationSettings settings = new VoiceTransformationSettings();

        [Header("RUNTIME AUDIO")]
        [SerializeField] private AudioSource previewAudioSource;
        [SerializeField] private AudioSource microphoneAudioSource;
        [SerializeField] private AudioSource targetOutputAudioSource;

        [Header("MICROPHONE")]
        [SerializeField] private string microphoneDevice = "";
        [SerializeField] private int microphoneLengthSeconds = 10;
        [SerializeField] private int microphoneSampleRate = 44100;

        [Header("STARTUP")]
        [SerializeField] private bool previewOnStart = false;
        [SerializeField] private bool saveSettingsAutomatically = true;
        [SerializeField] private string playerPrefsKey = "SL_VoiceTransformation";

        [Header("PITCH RANGE")]
        [SerializeField] private float minimumPitch = 0.65f;
        [SerializeField] private float maximumPitch = 1.35f;

        private Canvas canvas;
        private GameObject panel;
        private Slider genderSlider;
        private Slider pitchSlider;
        private Slider deepnessSlider;
        private Slider brightnessSlider;
        private Slider volumeSlider;
        private Text statusText;
        private Coroutine micRoutine;

        public VoiceTransformationSettings Settings => settings;

        private void Awake()
        {
            LoadSettings();
            EnsureAudioSources();

            if (createUIOnStart)
                BuildUI();

            ApplySettings();
        }

        private void Start()
        {
            if (previewOnStart)
                StartMicrophonePreview();
        }

        private void EnsureAudioSources()
        {
            if (previewAudioSource == null)
            {
                GameObject go = new GameObject("Voice Preview Audio");
                go.transform.SetParent(transform);
                previewAudioSource = go.AddComponent<AudioSource>();
                previewAudioSource.playOnAwake = false;
                previewAudioSource.loop = true;
            }

            if (microphoneAudioSource == null)
            {
                GameObject go = new GameObject("Voice Microphone Audio");
                go.transform.SetParent(transform);
                microphoneAudioSource = go.AddComponent<AudioSource>();
                microphoneAudioSource.playOnAwake = false;
                microphoneAudioSource.loop = true;
            }

            if (targetOutputAudioSource == null)
                targetOutputAudioSource = microphoneAudioSource;
        }

        public void BuildUI()
        {
            if (canvas != null) return;

            GameObject canvasObject = new GameObject(uiCanvasName);
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            panel = CreatePanel(canvas.transform);
            CreateTitle(panel.transform, "VOICE SETTINGS");

            CreateLabel(panel.transform, "Voice Type", 0.80f);
            genderSlider = CreateSlider(panel.transform, 0.70f);
            genderSlider.minValue = 0f;
            genderSlider.maxValue = 1f;
            genderSlider.value = settings.gender;
            genderSlider.onValueChanged.AddListener(v => { settings.gender = v; ApplySettings(); });

            CreateLabel(panel.transform, "Pitch", 0.59f);
            pitchSlider = CreateSlider(panel.transform, 0.49f);
            pitchSlider.minValue = minimumPitch;
            pitchSlider.maxValue = maximumPitch;
            pitchSlider.value = settings.pitch;
            pitchSlider.onValueChanged.AddListener(v => { settings.pitch = v; ApplySettings(); });

            CreateLabel(panel.transform, "Deepness", 0.38f);
            deepnessSlider = CreateSlider(panel.transform, 0.28f);
            deepnessSlider.value = settings.deepness;
            deepnessSlider.onValueChanged.AddListener(v => { settings.deepness = v; ApplySettings(); });

            CreateLabel(panel.transform, "Brightness", 0.17f);
            brightnessSlider = CreateSlider(panel.transform, 0.07f);
            brightnessSlider.value = settings.brightness;
            brightnessSlider.onValueChanged.AddListener(v => { settings.brightness = v; ApplySettings(); });

            CreateButton(panel.transform, "FEMININE", 0.0f, -55f, () => ApplyPreset(0.0f));
            CreateButton(panel.transform, "NEUTRAL", 0.34f, -55f, () => ApplyPreset(0.5f));
            CreateButton(panel.transform, "MASCULINE", 0.68f, -55f, () => ApplyPreset(1.0f));

            CreateButton(panel.transform, "START VOICE", 0.0f, -105f, StartMicrophonePreview);
            CreateButton(panel.transform, "STOP", 0.34f, -105f, StopMicrophonePreview);
            CreateButton(panel.transform, "SAVE", 0.68f, -105f, SaveSettings);

            statusText = CreateStatus(panel.transform);
            if (!showUI) panel.SetActive(false);
        }

        private GameObject CreatePanel(Transform parent)
        {
            GameObject p = new GameObject("VoiceSettingsPanel");
            p.transform.SetParent(parent, false);
            RectTransform r = p.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.08f, 0.10f);
            r.anchorMax = new Vector2(0.92f, 0.90f);
            r.offsetMin = r.offsetMax = Vector2.zero;

            Image img = p.AddComponent<Image>();
            img.color = new Color(0.035f, 0.035f, 0.05f, 0.94f);
            return p;
        }

        private void CreateTitle(Transform parent, string text)
        {
            Text t = CreateText(parent, text, 28);
            RectTransform r = t.rectTransform;
            r.anchorMin = new Vector2(0.05f, 0.86f);
            r.anchorMax = new Vector2(0.95f, 0.98f);
            r.offsetMin = r.offsetMax = Vector2.zero;
            t.alignment = TextAnchor.MiddleCenter;
        }

        private void CreateLabel(Transform parent, string text, float y)
        {
            Text t = CreateText(parent, text, 18);
            RectTransform r = t.rectTransform;
            r.anchorMin = new Vector2(0.08f, y);
            r.anchorMax = new Vector2(0.92f, y + 0.07f);
            r.offsetMin = r.offsetMax = Vector2.zero;
            t.alignment = TextAnchor.MiddleLeft;
        }

        private Slider CreateSlider(Transform parent, float y)
        {
            GameObject go = new GameObject("Slider");
            go.transform.SetParent(parent, false);
            RectTransform r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.08f, y);
            r.anchorMax = new Vector2(0.92f, y + 0.06f);
            r.offsetMin = r.offsetMax = Vector2.zero;

            Slider s = go.AddComponent<Slider>();
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(go.transform, false);
            Image bgi = bg.AddComponent<Image>();
            bgi.color = Color.gray;
            RectTransform br = bg.GetComponent<RectTransform>();
            br.anchorMin = new Vector2(0, .25f);
            br.anchorMax = new Vector2(1, .75f);
            br.offsetMin = br.offsetMax = Vector2.zero;

            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(go.transform, false);
            Image hi = handle.AddComponent<Image>();
            hi.color = Color.white;
            RectTransform hr = handle.GetComponent<RectTransform>();
            hr.sizeDelta = new Vector2(28, 28);

            s.targetGraphic = hi;
            s.fillRect = br;
            s.handleRect = hr;
            return s;
        }

        private void CreateButton(Transform parent, string label, float x, float y, UnityEngine.Events.UnityAction action)
        {
            GameObject go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            RectTransform r = go.AddComponent<RectTransform>();
            r.anchorMin = new Vector2(0.04f + x, 0.02f);
            r.anchorMax = new Vector2(0.32f + x, 0.11f);
            r.anchoredPosition = new Vector2(0, y);
            r.offsetMin += new Vector2(0, y);
            r.offsetMax += new Vector2(0, y);

            Image img = go.AddComponent<Image>();
            img.color = new Color(.16f, .16f, .22f, 1f);
            Button b = go.AddComponent<Button>();
            b.onClick.AddListener(action);

            Text t = CreateText(go.transform, label, 13);
            t.alignment = TextAnchor.MiddleCenter;
            t.rectTransform.anchorMin = Vector2.zero;
            t.rectTransform.anchorMax = Vector2.one;
            t.rectTransform.offsetMin = t.rectTransform.offsetMax = Vector2.zero;
        }

        private Text CreateStatus(Transform parent)
        {
            Text t = CreateText(parent, "Voice ready", 14);
            RectTransform r = t.rectTransform;
            r.anchorMin = new Vector2(.08f, .0f);
            r.anchorMax = new Vector2(.92f, .04f);
            r.offsetMin = r.offsetMax = Vector2.zero;
            t.alignment = TextAnchor.MiddleCenter;
            return t;
        }

        private Text CreateText(Transform parent, string value, int size)
        {
            GameObject go = new GameObject("Text");
            go.transform.SetParent(parent, false);
            Text t = go.AddComponent<Text>();
            t.text = value;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.color = Color.white;
            return t;
        }

        public void ApplyPreset(float gender)
        {
            settings.gender = gender;

            if (gender <= 0.01f)
            {
                settings.pitch = 1.16f;
                settings.deepness = 0.25f;
                settings.brightness = 0.72f;
            }
            else if (gender >= 0.99f)
            {
                settings.pitch = 0.84f;
                settings.deepness = 0.78f;
                settings.brightness = 0.38f;
            }
            else
            {
                settings.pitch = 1f;
                settings.deepness = .5f;
                settings.brightness = .5f;
            }

            SyncUI();
            ApplySettings();
        }

        public void ApplySettings()
        {
            if (previewAudioSource != null)
            {
                previewAudioSource.pitch = settings.pitch;
                previewAudioSource.volume = settings.volume;
            }

            if (microphoneAudioSource != null)
            {
                microphoneAudioSource.pitch = settings.pitch;
                microphoneAudioSource.volume = settings.volume;
            }

            if (targetOutputAudioSource != null)
            {
                targetOutputAudioSource.pitch = settings.pitch;
                targetOutputAudioSource.volume = settings.volume;
            }

            ApplyFilters(microphoneAudioSource);
            ApplyFilters(previewAudioSource);
            ApplyFilters(targetOutputAudioSource);

            if (saveSettingsAutomatically)
                SaveSettings();
        }

        private void ApplyFilters(AudioSource source)
        {
            if (source == null) return;

            AudioLowPassFilter low = source.GetComponent<AudioLowPassFilter>();
            if (low == null) low = source.gameObject.AddComponent<AudioLowPassFilter>();

            AudioHighPassFilter high = source.GetComponent<AudioHighPassFilter>();
            if (high == null) high = source.gameObject.AddComponent<AudioHighPassFilter>();

            float deepCut = Mathf.Lerp(5000f, 1800f, settings.deepness);
            float brightCut = Mathf.Lerp(1500f, 7000f, settings.brightness);

            low.cutoffFrequency = Mathf.Clamp(deepCut + brightCut * .35f, 500f, 22000f);
            high.cutoffFrequency = Mathf.Clamp(80f + settings.brightness * 350f, 10f, 2000f);
        }

        public void StartMicrophonePreview()
        {
            if (micRoutine != null) StopCoroutine(micRoutine);
            micRoutine = StartCoroutine(StartMicRoutine());
        }

        private IEnumerator StartMicRoutine()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
                Permission.RequestUserPermission(Permission.Microphone);
#endif
            yield return new WaitForSeconds(.25f);

            if (Microphone.devices == null || Microphone.devices.Length == 0)
            {
                SetStatus("No microphone found");
                yield break;
            }

            string device = microphoneDevice;
            if (string.IsNullOrEmpty(device))
                device = Microphone.devices[0];

            AudioClip clip = Microphone.Start(device, true, microphoneLengthSeconds, microphoneSampleRate);
            while (Microphone.GetPosition(device) <= 0)
                yield return null;

            microphoneAudioSource.clip = clip;
            microphoneAudioSource.loop = true;
            microphoneAudioSource.Play();
            SetStatus("Microphone preview ON");
        }

        public void StopMicrophonePreview()
        {
            if (!string.IsNullOrEmpty(microphoneDevice) && Microphone.IsRecording(microphoneDevice))
                Microphone.End(microphoneDevice);
            else if (Microphone.devices != null && Microphone.devices.Length > 0 && Microphone.IsRecording(Microphone.devices[0]))
                Microphone.End(Microphone.devices[0]);

            if (microphoneAudioSource != null)
                microphoneAudioSource.Stop();

            SetStatus("Microphone preview OFF");
        }

        private void SyncUI()
        {
            if (genderSlider != null) genderSlider.value = settings.gender;
            if (pitchSlider != null) pitchSlider.value = settings.pitch;
            if (deepnessSlider != null) deepnessSlider.value = settings.deepness;
            if (brightnessSlider != null) brightnessSlider.value = settings.brightness;
            if (volumeSlider != null) volumeSlider.value = settings.volume;
        }

        public void SetUIVisible(bool visible)
        {
            showUI = visible;
            if (panel != null) panel.SetActive(visible);
        }

        public void SaveSettings()
        {
            string data = JsonUtility.ToJson(settings);
            PlayerPrefs.SetString(playerPrefsKey, data);
            PlayerPrefs.Save();
            SetStatus("Voice settings saved");
        }

        public void LoadSettings()
        {
            if (!PlayerPrefs.HasKey(playerPrefsKey)) return;

            string data = PlayerPrefs.GetString(playerPrefsKey);
            if (!string.IsNullOrEmpty(data))
                settings = JsonUtility.FromJson<VoiceTransformationSettings>(data);
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
                statusText.text = message;
        }
    }
}
