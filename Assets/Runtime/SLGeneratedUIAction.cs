using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SoulboundLegends.Runtime
{
    /// <summary>Serialized action target used by Builder-generated UI buttons.</summary>
    public sealed class SLGeneratedUIAction : MonoBehaviour
    {
        [Tooltip("Optional scene to load when this button is pressed.")]
        public string targetSceneName;
        [Tooltip("Optional panel to toggle when this button is pressed.")]
        public GameObject targetPanel;
        public string targetPanelName;
        [Tooltip("Optional message to log for diagnostics.")]
        public string diagnosticMessage;
        [Tooltip("Optional gameplay command: Attack, ElementalSkill, or GrantXP.")]
        public string gameplayCommand;

        public void Execute()
        {
            if (!string.IsNullOrWhiteSpace(gameplayCommand))
            {
                ExecuteGameplayCommand();
                return;
            }
            if (!string.IsNullOrWhiteSpace(targetSceneName))
            {
                if (Application.CanStreamedLevelBeLoaded(targetSceneName))
                    SceneManager.LoadScene(targetSceneName);
                else
                    Debug.LogWarning("Soulbound Legends: target scene is not in Build Settings: " + targetSceneName);
                return;
            }
            if (targetPanel == null && !string.IsNullOrWhiteSpace(targetPanelName))
            {
                var canvas = GetComponentInParent<Canvas>();
                if (canvas != null) targetPanel = canvas.transform.Find(targetPanelName)?.gameObject;
            }
            if (targetPanel != null)
            {
                targetPanel.SetActive(!targetPanel.activeSelf);
                return;
            }
            if (!string.IsNullOrWhiteSpace(diagnosticMessage))
            {
                Debug.Log(diagnosticMessage);
                var status = GameObject.Find("LoginStatus");
                if (status != null)
                {
                    var text = status.GetComponent<Text>();
                    if (text != null) text.text = diagnosticMessage;
                }
            }
        }

        static void PublishStatus(string message)
        {
            Debug.Log("Soulbound Legends: " + message);
            var statusObject = GameObject.Find("SL_StatusText");
            if (statusObject != null)
            {
                var statusText = statusObject.GetComponent<Text>();
                if (statusText != null) statusText.text = message;
            }
        }

        void ExecuteGameplayCommand()
        {
            var player = GameObject.Find("Player_RiggedProcedural");
            if (player == null) { Debug.LogWarning("Soulbound Legends: gameplay command needs the player actor in this scene."); return; }
            switch (gameplayCommand)
            {
                case "Attack":
                {
                    var combat = player.GetComponent<SLRuntimeCombatInput>();
                    if (combat != null) combat.AttackNearest();
                    else Debug.Log("This scene has no local combat input component.");
                    break;
                }
                case "HeavyAttack": { var combat=player.GetComponent<SLRuntimeCombatInput>(); if(combat!=null)combat.HeavyAttack(); break; }
                case "Block": { var combat=player.GetComponent<SLRuntimeCombatInput>(); if(combat!=null)combat.Block(); break; }
                case "Dodge": { var combat=player.GetComponent<SLRuntimeCombatInput>(); if(combat!=null)combat.Dodge(); break; }
                case "Jump": { var combat=player.GetComponent<SLRuntimeCombatInput>(); if(combat!=null)combat.Jump(); break; }
                case "ElementalSkill":
                {
                    foreach (var enemy in FindObjectsByType<SLRuntimeCombatant>(FindObjectsSortMode.None))
                    {
                        if (enemy == null || enemy.gameObject == player || enemy.IsDead) continue;
                        if (Vector3.Distance(player.transform.position, enemy.transform.position) <= 6f) enemy.ApplyDamage(100f);
                    }
                    var burst = new GameObject("SL_ElementalSkillBurst");
                    burst.transform.position = player.transform.position + Vector3.up;
                    var particles = burst.AddComponent<ParticleSystem>();
                    var main = particles.main; main.startColor = new Color(.2f,.75f,1f,.9f); main.startSize = .35f; main.startLifetime = 1.1f; main.duration = .7f; main.loop = false;
                    var emission = particles.emission; emission.rateOverTime = 0; emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 42) });
                    var shape = particles.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 2.4f;
                    Destroy(burst, 2f);
                    break;
                }
                case "GrantXP":
                {
                    var progression = player.GetComponent<SLGeneratedProgression>();
                    if (progression != null) progression.GrantQuestReward();
                    break;
                }
                case "ChangeElement":
                {
                    int element = (PlayerPrefs.GetInt("SL_CharacterElement", 0) + 1) % 10;
                    PlayerPrefs.SetInt("SL_CharacterElement", element);
                    Color[] elements = { new Color(.1f,.7f,1f), new Color(1f,.22f,.08f), new Color(.4f,.7f,.22f), new Color(.55f,.2f,.95f), new Color(1f,.8f,.12f), new Color(.15f,.9f,.65f), Color.white, new Color(.5f,.75f,1f), new Color(.55f,.85f,1f), new Color(.85f,.25f,.95f) };
                    foreach (var renderer in player.GetComponentsInChildren<Renderer>(true))
                    {
                        if (renderer.name.Contains("BackCrystal") || renderer.name.Contains("CrownCore") || renderer.name.Contains("ChestSigil") || renderer.name.Contains("ShoulderRune")) renderer.material.color = elements[element];
                    }
                    Debug.Log("Selected element index: " + element);
                    break;
                }
                case "GatherMaterials":
                {
                    var director=FindFirstObjectByType<SLAdventureWorldDirector>();
                    if(director!=null) director.GatherNearest();
                    else {var systems=SLVillageGameplaySystems.Instance;if(systems!=null)systems.Gather();PublishStatus(systems!=null?systems.status:"Gathering system is not present.");}
                    break;
                }
                case "InteractNearest":
                {
                    var p=GameObject.Find("Player_RiggedProcedural"); if(!p){PublishStatus("Player not found.");break;}
                    SLVillageNPC nearest=null;float best=4f;
                    foreach(var npc in FindObjectsByType<SLVillageNPC>(FindObjectsSortMode.None)){float d=Vector3.Distance(p.transform.position,npc.transform.position);if(d<best){best=d;nearest=npc;}}
                    if(nearest!=null)
                    {
                        PublishStatus(nearest.Interact());
                        if(nearest.npcName.Contains("Alchemist")){var c=GameObject.Find("Panel_Alchemy");if(c)c.SetActive(true);}
                        if(nearest.npcName.Contains("Blacksmith")){var c=GameObject.Find("Panel_Blacksmith");if(c)c.SetActive(true);}
                        if(nearest.npcName.Contains("Guild")){var c=GameObject.Find("Panel_Quests");if(c)c.SetActive(true);}
                    }
                    else {var director=FindFirstObjectByType<SLAdventureWorldDirector>();if(director!=null)director.GatherNearest();else PublishStatus("Move closer to a villager or resource.");}
                    break;
                }
                case "UsePotion":
                {
                    var inv=SLVillageGameplaySystems.Instance;var hp=player.GetComponent<SLRuntimeCombatant>();
                    if(inv==null||hp==null){PublishStatus("Potion or player health system is unavailable.");break;}
                    if(inv.Take("Minor Healing Potion",1)){hp.health=Mathf.Min(hp.maxHealth,hp.health+250f);PublishStatus("Used Minor Healing Potion. Restored up to 250 HP.");}
                    else PublishStatus("No Minor Healing Potion in inventory.");break;
                }
                case "SaveProgress": SLPlayerSave.SaveCurrent(); PublishStatus("Local progress saved on this device."); break;
                case "CraftPotion": { var systems = SLVillageGameplaySystems.Instance; if (systems != null) systems.CraftPotion(); PublishStatus(systems != null ? systems.status : "Alchemy system is not present."); break; }
                case "ForgeSword": { var systems = SLVillageGameplaySystems.Instance; if (systems != null) systems.ForgeSword(); PublishStatus(systems != null ? systems.status : "Blacksmith system is not present."); break; }
                case "ForgeStaff": { var systems = SLVillageGameplaySystems.Instance; if (systems != null) systems.ForgeStaff(); PublishStatus(systems != null ? systems.status : "Blacksmith system is not present."); break; }
                case "ForgeBow": { var systems = SLVillageGameplaySystems.Instance; if (systems != null) systems.ForgeBow(); PublishStatus(systems != null ? systems.status : "Blacksmith system is not present."); break; }
                case "GuildQuest": { var q=SLQuestProgression.Instance; if(q!=null)q.AdvanceFromGuild(); else {var systems=SLVillageGameplaySystems.Instance;if(systems!=null)systems.TalkToGuild();PublishStatus(systems!=null?systems.status:"Quest system is not present.");} break; }
                case "ClaimGuildQuest": { var q=SLQuestProgression.Instance;if(q!=null)q.Claim();else {var systems=SLVillageGameplaySystems.Instance;if(systems!=null)systems.ClaimQuest();PublishStatus(systems!=null?systems.status:"Quest system is not present.");} break; }
                case "EquipSword": { var systems=SLVillageGameplaySystems.Instance; if(systems!=null)systems.Equip("Starter_IronSword"); PublishStatus(systems!=null?systems.status:"Inventory system is not present."); break; }
                case "EquipStaff": { var systems=SLVillageGameplaySystems.Instance; if(systems!=null)systems.Equip("Starter_ApprenticeStaff"); PublishStatus(systems!=null?systems.status:"Inventory system is not present."); break; }
                case "EquipBow": { var systems=SLVillageGameplaySystems.Instance; if(systems!=null)systems.Equip("Starter_HunterBow"); PublishStatus(systems!=null?systems.status:"Inventory system is not present."); break; }
                case "ShowInventory": { var systems = SLVillageGameplaySystems.Instance; Debug.Log(systems != null ? systems.InventoryText() : "Inventory system is not present."); var label = GameObject.Find("InventoryPanelText"); if(label != null){var t=label.GetComponent<Text>(); if(t != null && systems != null)t.text=systems.InventoryText();} PublishStatus(systems != null ? "Inventory refreshed." : "Inventory system is not present."); break; }
                case "RandomizeLook":
                {
                    float scale = UnityEngine.Random.Range(.92f, 1.09f);
                    player.transform.localScale = new Vector3(scale, 1f, scale);
                    foreach (var renderer in player.GetComponentsInChildren<Renderer>(true))
                    {
                        if (renderer.name.Contains("ChestJacket") || renderer.name.Contains("Tunic") || renderer.name.Contains("HairCap")) renderer.material.color = UnityEngine.Random.ColorHSV(.48f,.68f,.45f,.85f,.45f,.95f);
                    }
                    break;
                }
            }
        }
    }

    /// <summary>Close button helper for Builder-generated panels.</summary>
    public sealed class SLGeneratedPanelCloser : MonoBehaviour
    {
        public GameObject panel;
        public void Close() { if (panel != null) panel.SetActive(false); }
    }

    /// <summary>Close-range third-person camera with touch/mouse orbit, zoom limits and collision handling.</summary>
    public sealed class SLBuilderThirdPersonCamera : MonoBehaviour
    {
        public Transform target;
        public float distance = 5.2f;
        public float height = 1.7f;
        public float shoulderOffset = 0.45f;
        public float sensitivity = 0.16f;
        public float smoothing = 12f;
        public float minimumDistance = 2.8f;
        public float maximumDistance = 8f;
        public float pitch = 12f;
        float yaw;
        float desiredDistance;
        Vector2 lastPointer;
        bool dragging;

        void Awake()
        {
            desiredDistance = distance;
            if (target != null) yaw = target.eulerAngles.y;
        }

        void LateUpdate()
        {
            if (target == null) return;
            ReadOrbitInput();
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 focus = target.position + Vector3.up * height;
            Vector3 shoulder = target.right * shoulderOffset;
            Vector3 backwards = rotation * Vector3.back;
            Vector3 desired = focus + shoulder + backwards * desiredDistance;
            Vector3 ray = desired - focus;
            if (Physics.SphereCast(focus, 0.18f, ray.normalized, out RaycastHit hit, ray.magnitude, ~0, QueryTriggerInteraction.Ignore))
                desired = focus + ray.normalized * Mathf.Max(0.6f, hit.distance - 0.12f);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smoothing * Time.deltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(focus + target.forward * 1.2f - transform.position, Vector3.up), 1f - Mathf.Exp(-smoothing * Time.deltaTime));
        }

        void ReadOrbitInput()
        {
            if (Input.GetMouseButtonDown(1)) { dragging = true; lastPointer = Input.mousePosition; }
            if (Input.GetMouseButtonUp(1)) dragging = false;
            if (dragging && Input.GetMouseButton(1))
            {
                Vector2 p = Input.mousePosition; Vector2 d = p - lastPointer; lastPointer = p;
                yaw += d.x * sensitivity; pitch = Mathf.Clamp(pitch - d.y * sensitivity, -5f, 55f);
            }
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                if (t.phase == TouchPhase.Moved && t.position.x > Screen.width * 0.35f)
                {
                    yaw += t.deltaPosition.x * sensitivity * 0.65f;
                    pitch = Mathf.Clamp(pitch - t.deltaPosition.y * sensitivity * 0.65f, -5f, 55f);
                }
                if (Input.touchCount > 1)
                {
                    Touch t2 = Input.GetTouch(1);
                    if (t.phase == TouchPhase.Moved && t2.phase == TouchPhase.Moved)
                        desiredDistance = Mathf.Clamp(desiredDistance - (t.deltaPosition.magnitude - t2.deltaPosition.magnitude) * 0.004f, minimumDistance, maximumDistance);
                }
            }
            float scroll = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scroll) > 0.01f) desiredDistance = Mathf.Clamp(desiredDistance - scroll * 0.35f, minimumDistance, maximumDistance);
        }
    }
}
