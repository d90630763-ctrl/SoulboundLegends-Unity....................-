using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SoulboundLegends.Runtime
{
    /// <summary>Runtime village/world services generated and attached by the Game Builder.</summary>
    public sealed class SLAdventureWorldDirector : MonoBehaviour
    {
        public int worldSeed = 1001;
        public int resourceNodeCount = 18;
        public bool generateResourceNodes = true;
        public bool enableDayNight = true;
        public bool enableWeather = true;
        public float dayLengthSeconds = 420f;
        public Light sun;
        public string currentWeather = "Clear";
        float weatherTimer;
        float saveTimer;
        readonly List<SLResourceNode> nodes = new List<SLResourceNode>();
        static readonly string[] weatherTypes = { "Clear", "Cloudy", "Rain", "Arcane Mist" };
        public IReadOnlyList<SLResourceNode> ResourceNodes => nodes;

        void Start()
        {
            if (sun == null) { var s = GameObject.Find("SL_Sunlight"); if (s) sun = s.GetComponent<Light>(); }
            if (generateResourceNodes) GenerateNodes();
            ApplyWeather("Clear");
        }
        void Update()
        {
            if (enableDayNight && sun != null)
            {
                float angle = (Time.time / Mathf.Max(60f, dayLengthSeconds)) * 360f - 90f;
                sun.transform.rotation = Quaternion.Euler(angle, -32f, 0f);
                float daylight = Mathf.Clamp01(Mathf.Sin(angle * Mathf.Deg2Rad) * .8f + .25f);
                sun.intensity = Mathf.Lerp(.12f, 1.25f, daylight);
                RenderSettings.ambientIntensity = Mathf.Lerp(.35f, 1f, daylight);
            }
            if (enableWeather)
            {
                weatherTimer += Time.deltaTime;
                if (weatherTimer > 150f) { weatherTimer = 0; ApplyWeather(weatherTypes[UnityEngine.Random.Range(0, weatherTypes.Length)]); }
            }
            saveTimer += Time.unscaledDeltaTime;
            if (saveTimer > 20f) { saveTimer = 0; SLPlayerSave.SaveCurrent(); }
        }
        void GenerateNodes()
        {
            var random = new System.Random(worldSeed);
            for (int i = 0; i < resourceNodeCount; i++)
            {
                float x = (float)(random.NextDouble() * 44.0 - 22.0);
                float z = (float)(random.NextDouble() * 30.0 + 2.0);
                string item = i % 3 == 0 ? "Herb" : i % 3 == 1 ? "Wood" : "Iron Ore";
                PrimitiveType shape = item == "Herb" ? PrimitiveType.Sphere : item == "Wood" ? PrimitiveType.Cylinder : PrimitiveType.Cube;
                var go = GameObject.CreatePrimitive(shape);
                go.name = "GatherNode_" + item.Replace(" ", "") + "_" + i;
                go.transform.SetParent(transform, true);
                go.transform.position = new Vector3(x, item == "Wood" ? .42f : .28f, z);
                go.transform.localScale = item == "Herb" ? new Vector3(.38f,.45f,.38f) : item == "Wood" ? new Vector3(.22f,.48f,.22f) : new Vector3(.55f,.42f,.5f);
                var renderer = go.GetComponent<Renderer>();
                var shader = Shader.Find("Universal Render Pipeline/Lit"); if (!shader) shader = Shader.Find("Standard");
                if (renderer && shader) { var mat = new Material(shader); mat.color = item == "Herb" ? new Color(.2f,.8f,.28f) : item == "Wood" ? new Color(.32f,.18f,.08f) : new Color(.48f,.52f,.58f); renderer.sharedMaterial = mat; }
                var node = go.AddComponent<SLResourceNode>(); node.itemId = item; node.amount = item == "Herb" ? 2 : 1; nodes.Add(node);
            }
        }
        public void ApplyWeather(string weather)
        {
            currentWeather = weather;
            switch (weather)
            {
                case "Cloudy": RenderSettings.fog = true; RenderSettings.fogDensity = .009f; if (sun) sun.intensity = .7f; break;
                case "Rain": RenderSettings.fog = true; RenderSettings.fogDensity = .012f; if (sun) sun.intensity = .55f; break;
                case "Arcane Mist": RenderSettings.fog = true; RenderSettings.fogDensity = .018f; RenderSettings.fogColor = new Color(.22f,.18f,.42f); break;
                default: RenderSettings.fog = false; RenderSettings.fogDensity = .006f; break;
            }
            Publish("Village weather: " + currentWeather + ".");
        }
        public void GatherNearest()
        {
            var player = GameObject.Find("Player_RiggedProcedural");
            if (!player) { Publish("Player actor not found."); return; }
            SLResourceNode nearest = null; float best = 3.5f;
            foreach (var n in nodes) if (n && n.Available) { float d = Vector3.Distance(player.transform.position,n.transform.position); if (d < best) { best=d; nearest=n; } }
            if (nearest) nearest.Gather(); else Publish("No resource node in reach. Move closer to a glowing herb, tree or ore deposit.");
        }
        public static void Publish(string message)
        {
            Debug.Log("Soulbound Legends: " + message);
            var go = GameObject.Find("SL_StatusText"); if (go) { var t=go.GetComponent<Text>(); if(t) t.text=message; }
        }
    }

    public sealed class SLResourceNode : MonoBehaviour
    {
        public string itemId = "Herb";
        public int amount = 1;
        public bool Available { get; private set; } = true;
        public void Gather()
        {
            if (!Available) { SLAdventureWorldDirector.Publish("This resource has already been gathered."); return; }
            var inv=SLVillageGameplaySystems.Instance; if(!inv) { SLAdventureWorldDirector.Publish("Inventory system is not active."); return; }
            inv.Seed(itemId,amount); Available=false; gameObject.SetActive(false);
            SLQuestProgression.NotifyGather(itemId, amount);
            SLAdventureWorldDirector.Publish("Gathered " + amount + " " + itemId + ".");
        }
    }

    /// <summary>Simple local AI for generated beasts: patrol, pursue, attack and grant a local reward on defeat.</summary>
    public sealed class SLMonsterBrain : MonoBehaviour
    {
        public float detectionRange = 12f;
        public float attackRange = 1.8f;
        public float moveSpeed = 2.1f;
        public float attackDamage = 35f;
        public float attackCooldown = 1.4f;
        public int xpReward = 20;
        Transform target;
        float nextAttack;
        Vector3 home;
        SLRuntimeCombatant combatant;
        bool rewardGranted;
        void Start() { home=transform.position; combatant=GetComponent<SLRuntimeCombatant>(); var p=GameObject.Find("Player_RiggedProcedural"); if(p)target=p.transform; }
        void Update()
        {
            if (!target) { var p=GameObject.Find("Player_RiggedProcedural"); if(p)target=p.transform; else return; }
            if (combatant && combatant.IsDead) return;
            float d=Vector3.Distance(transform.position,target.position);
            if(d<=detectionRange)
            {
                Vector3 flat=target.position-transform.position; flat.y=0;
                if(flat.sqrMagnitude>.01f) { transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(flat),Time.deltaTime*5f); }
                if(d>attackRange) transform.position=Vector3.MoveTowards(transform.position,target.position,moveSpeed*Time.deltaTime);
                else if(Time.time>=nextAttack) { nextAttack=Time.time+attackCooldown; var hp=target.GetComponent<SLRuntimeCombatant>(); if(hp)hp.ApplyDamage(attackDamage); }
            }
            else if(Vector3.Distance(transform.position,home)>1.5f) transform.position=Vector3.MoveTowards(transform.position,home,moveSpeed*.35f*Time.deltaTime);
        }
        public void OnDefeated()
        {
            if(rewardGranted)return; rewardGranted=true;
            var p=GameObject.Find("Player_RiggedProcedural"); if(p){var xp=p.GetComponent<SLGeneratedProgression>();if(xp)xp.AddExperience(xpReward);}
        }
    }

    [Serializable] public class SLQuestSaveData { public int questIndex; public int gathered; public int defeated; public bool claimed; }
    /// <summary>Small real local quest chain with objective checks, save persistence and reward gating.</summary>
    public sealed class SLQuestProgression : MonoBehaviour
    {
        public static SLQuestProgression Instance { get; private set; }
        public SLQuestSaveData data = new SLQuestSaveData();
        static readonly string[] titles={"First Steps","Gather Supplies","Protect the Trail","Guild Initiate"};
        void Awake(){Instance=this; string json=PlayerPrefs.GetString("SL_QuestData",""); if(!string.IsNullOrEmpty(json))try{data=JsonUtility.FromJson<SLQuestSaveData>(json);}catch{} }
        void OnDestroy(){if(Instance==this)Instance=null;}
        float uiRefresh;
        void Update(){if(Time.unscaledTime<uiRefresh)return;uiRefresh=Time.unscaledTime+.5f;var hud=GameObject.Find("QuestHUD");if(hud){var t=hud.GetComponent<Text>();if(t)t.text="GUILD • "+CurrentObjective;}var panel=GameObject.Find("QuestPanelText");if(panel){var t=panel.GetComponent<Text>();if(t)t.text="QUEST LOG\n\nCurrent: "+CurrentObjective+"\n\nGathered resources: "+data.gathered+"\nBeasts defeated: "+data.defeated+"\n\nReturn to the Adventure Guild when the objective is complete.";}}
        public string CurrentObjective
        {
            get { if(data.questIndex==0)return "Speak to the Adventure Guild Guide"; if(data.questIndex==1)return "Gather 3 resources ("+Mathf.Min(3,data.gathered)+"/3)"; if(data.questIndex==2)return "Defeat 2 beasts ("+Mathf.Min(2,data.defeated)+"/2)"; if(data.questIndex>=3)return "Return to the Guild to claim your reward"; return "Explore Village 001"; }
        }
        public static void NotifyGather(string item,int amount){if(Instance){Instance.data.gathered+=amount;Instance.RefreshObjectives();}}
        public static void NotifyMonsterDefeated(){if(Instance){Instance.data.defeated++;Instance.RefreshObjectives();}}
        public void AdvanceFromGuild(){if(data.questIndex==0)data.questIndex=1; Save(); SLAdventureWorldDirector.Publish("Adventure Guild — "+titles[Mathf.Clamp(data.questIndex,0,titles.Length-1)]+": "+CurrentObjective);}
        public bool CanClaim => data.questIndex>=3&&!data.claimed;
        public void Claim()
        {
            if(!CanClaim){SLAdventureWorldDirector.Publish("Quest incomplete: "+CurrentObjective);return;}
            data.claimed=true; var inv=SLVillageGameplaySystems.Instance;if(inv){inv.coins+=10;inv.Seed("Guild Token",1);}var p=GameObject.Find("Player_RiggedProcedural");if(p){var xp=p.GetComponent<SLGeneratedProgression>();if(xp)xp.AddExperience(30);}Save();SLAdventureWorldDirector.Publish("Guild quest complete! +30 XP, +10 coins and 1 Guild Token.");
        }
        public void RefreshObjectives()
        {
            if(data.questIndex==1&&data.gathered>=3)data.questIndex=2;
            if(data.questIndex==2&&data.defeated>=2)data.questIndex=3;
            Save();
        }
        void Save(){PlayerPrefs.SetString("SL_QuestData",JsonUtility.ToJson(data));PlayerPrefs.Save();}
    }

    [Serializable] public class SLPlayerSaveData { public int level=1; public int xp; public int coins=25; public string inventoryJson=""; public string element="Water"; }
    public static class SLPlayerSave
    {
        const string Key="SL_PlayerSave_v1";
        public static void SaveCurrent()
        {
            var d=new SLPlayerSaveData();var p=GameObject.Find("Player_RiggedProcedural");if(p){var xp=p.GetComponent<SLGeneratedProgression>();if(xp){d.level=xp.level;d.xp=xp.experience;}d.element=PlayerPrefs.GetString("SL_CharacterElementName","Water");}
            var inv=SLVillageGameplaySystems.Instance;if(inv){d.coins=inv.coins;d.inventoryJson=JsonUtility.ToJson(new InventoryWrapper(inv.inventory));}
            PlayerPrefs.SetString(Key,JsonUtility.ToJson(d));PlayerPrefs.Save();
        }
        public static SLPlayerSaveData Load(){var json=PlayerPrefs.GetString(Key,"");return string.IsNullOrEmpty(json)?new SLPlayerSaveData():JsonUtility.FromJson<SLPlayerSaveData>(json);}
        public static void LoadInto(SLVillageGameplaySystems systems)
        {
            var d=Load(); systems.coins=d.coins;
            if(!string.IsNullOrEmpty(d.inventoryJson))try{var wrapper=JsonUtility.FromJson<InventoryWrapper>(d.inventoryJson);if(wrapper!=null&&wrapper.items!=null&&wrapper.items.Count>0)systems.inventory=wrapper.items;}catch{}
        }
        [Serializable] class InventoryWrapper { public List<SLVillageGameplaySystems.Stack> items; public InventoryWrapper(List<SLVillageGameplaySystems.Stack> i){items=i;} }
    }
}
