using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Runtime
{
    /// <summary>Local gameplay foundation for gathering, inventory, starter weapons, alchemy, blacksmithing and NPC quests.
    /// Online saves, trading and authoritative multiplayer crafting must be connected to server services before release.</summary>
    public sealed class SLVillageGameplaySystems : MonoBehaviour
    {
        public static SLVillageGameplaySystems Instance { get; private set; }
        [Serializable] public class Stack { public string itemId; public int amount; public Stack(string id,int n){itemId=id;amount=n;} }
        public List<Stack> inventory = new List<Stack>();
        public int coins = 25;
        public int questProgress;
        public bool starterSword, starterStaff, starterBow;
        public string status = "Welcome to Village 001. Speak to villagers and gather materials.";
        void Awake(){ if(Instance==null) Instance=this; Seed("Herb",5); Seed("Iron Ore",4); Seed("Wood",5); Seed("Monster Core",1); Seed("Water Flask",3); SLPlayerSave.LoadInto(this); }
        void OnDestroy(){if(Instance==this)Instance=null;}
        public void Seed(string id,int amount){var s=inventory.Find(x=>x.itemId==id);if(s==null)inventory.Add(new Stack(id,amount));else s.amount+=amount;}
        public int Count(string id){var s=inventory.Find(x=>x.itemId==id);return s==null?0:s.amount;}
        public bool Take(string id,int amount){var s=inventory.Find(x=>x.itemId==id);if(s==null||s.amount<amount)return false;s.amount-=amount;if(s.amount<=0)inventory.Remove(s);return true;}
        public bool CraftPotion(){if(Count("Herb")<2||Count("Water Flask")<1){status="Alchemy failed: need 2 Herbs and 1 Water Flask.";return false;}Take("Herb",2);Take("Water Flask",1);Seed("Minor Healing Potion",1);status="Alchemy complete: Minor Healing Potion added to inventory.";return true;}
        public bool ForgeSword(){if(Count("Iron Ore")<3||Count("Wood")<1){status="Blacksmith failed: need 3 Iron Ore and 1 Wood.";return false;}Take("Iron Ore",3);Take("Wood",1);starterSword=true;Seed("Iron Sword",1);status="Forged an Iron Sword. Equip it from Inventory.";return true;}
        public bool ForgeStaff(){if(Count("Wood")<3||Count("Monster Core")<1){status="Blacksmith failed: need 3 Wood and 1 Monster Core.";return false;}Take("Wood",3);Take("Monster Core",1);starterStaff=true;Seed("Apprentice Staff",1);status="Forged an Apprentice Staff.";return true;}
        public bool ForgeBow(){if(Count("Wood")<3||Count("Herb")<1){status="Blacksmith failed: need 3 Wood and 1 Herb for binding.";return false;}Take("Wood",3);Take("Herb",1);starterBow=true;Seed("Hunter Bow",1);status="Crafted a Hunter Bow.";return true;}
        public void Gather(){Seed("Herb",1);Seed("Wood",1);Seed("Iron Ore",1);status="Gathered 1 Herb, 1 Wood and 1 Iron Ore.";}
        public void TalkToGuild(){status="Adventure Guild: First Steps — gather materials, craft a potion, then defeat a nearby beast. Reward: 30 XP and 10 coins.";}
        public void ClaimQuest(){if(questProgress>0){status="Quest reward already claimed. New guild quests will be added through content updates.";return;}questProgress=1;coins+=10;Seed("Guild Token",1);var p=GameObject.Find("Player_RiggedProcedural");if(p){var xp=p.GetComponent<SLGeneratedProgression>();if(xp)xp.GrantQuestReward();}status="Quest completed! +30 XP (if progression is connected), +10 coins and 1 Guild Token.";}
        public void Equip(string weapon){var p=GameObject.Find("Player_RiggedProcedural");if(!p){status="No player found in this scene.";return;}var gen=p.GetComponent<SLStarterWeaponGenerator>();if(!gen){status="Starter weapon generator is not attached to player.";return;}foreach(Transform child in gen.weaponSocket)if(child.name.StartsWith("Starter_"))child.gameObject.SetActive(child.name==weapon);status="Equipped: "+weapon.Replace("Starter_","")+".";}
        public string InventoryText(){var lines="INVENTORY  |  Coins: "+coins+"\n";foreach(var s in inventory)lines+=s.itemId+"  x"+s.amount+"\n";return lines;}
    }

    /// <summary>Creates low-poly starter weapon meshes from Unity primitives and attaches them to a player hand socket.
    /// Replace these blockout models with authored art for final visual quality.</summary>
    public sealed class SLStarterWeaponGenerator : MonoBehaviour
    {
        public bool generateSword=true, generateStaff=true, generateBow=true;
        public Transform weaponSocket;
        void Start(){if(!weaponSocket){var socket=new GameObject("RightHand_WeaponSocket").transform;socket.SetParent(transform,false);socket.localPosition=new Vector3(.42f,1.05f,.18f);weaponSocket=socket;}BuildWeapon("Starter_IronSword",new Vector3(.055f,.62f,.055f),new Color(.72f,.8f,.9f),new Vector3(0,.35f,0),generateSword);BuildWeapon("Starter_ApprenticeStaff",new Vector3(.045f,1.05f,.045f),new Color(.35f,.2f,.65f),new Vector3(.1f,.1f,0),generateStaff);BuildWeapon("Starter_HunterBow",new Vector3(.06f,.62f,.06f),new Color(.42f,.22f,.09f),new Vector3(-.12f,.2f,0),generateBow);}
        void BuildWeapon(string name,Vector3 size,Color color,Vector3 offset,bool enabled){if(!enabled)return;var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name=name;g.transform.SetParent(weaponSocket,false);g.transform.localPosition=offset;g.transform.localScale=size;var r=g.GetComponent<Renderer>();var m=new Material(Shader.Find("Standard"));m.color=color;r.sharedMaterial=m;var c=g.GetComponent<Collider>();if(c)Destroy(c);g.SetActive(false);}
    }

    public sealed class SLVillageNPC : MonoBehaviour
    {
        public string npcName="Village Resident";
        [TextArea] public string greeting="Welcome, traveller. The Adventure Guild can guide your first steps.";
        public float interactionDistance=3f;
        public string Interact(){var player=GameObject.Find("Player_RiggedProcedural");if(!player||Vector3.Distance(player.transform.position,transform.position)>interactionDistance)return npcName+": Come closer so we can speak.";if(npcName.Contains("Guild")){var q=SLQuestProgression.Instance;if(q!=null){q.AdvanceFromGuild();return npcName+": " + q.CurrentObjective;}}return npcName+": "+greeting;}
    }
}
