using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    [CreateAssetMenu(menuName="Soulbound Legends/NPC/Occupation", fileName="NPC_Occupation")]
    public class NPCOccupationDefinition : ScriptableObject
    {
        public string occupationId = "occupation";
        public string displayName = "Occupation";
        [TextArea] public string description;
        public List<NPCServiceType> allowedServices = new();
        public List<NPCSystemCapability> requiredCapabilities = new();
        public List<NPCSystemCapability> forbiddenCapabilities = new();
        public List<string> defaultDialogueTags = new();
    }

    [CreateAssetMenu(menuName="Soulbound Legends/NPC/Occupation Database", fileName="NPC_OccupationDatabase")]
    public class NPCOccupationDatabase : ScriptableObject
    {
        public List<NPCOccupationDefinition> occupations = new();
        public NPCOccupationDefinition Find(string id) => occupations.Find(x => x && x.occupationId == id);
    }

    [CreateAssetMenu(menuName="Soulbound Legends/NPC/Voice Profile", fileName="NPC_VoiceProfile")]
    public class NPCVoiceProfile : ScriptableObject
    {
        public string profileId;
        public NPCGender gender;
        [Range(0f,1f)] public float genderVoiceBlend = .5f;
        [Range(.5f,2f)] public float pitch = 1f;
        [Range(.5f,2f)] public float speed = 1f;
        [TextArea] public string notes;
    }

    [CreateAssetMenu(menuName="Soulbound Legends/NPC/Dialogue Database", fileName="NPC_DialogueDatabase")]
    public class NPCDialogueDatabase : ScriptableObject
    {
        [Serializable] public class Entry { public string tag; [TextArea] public List<string> lines = new(); }
        public List<Entry> entries = new();
        public string GetRandom(string tag)
        {
            var e = entries.Find(x => x != null && x.tag == tag && x.lines != null && x.lines.Count > 0);
            if (e == null) return string.Empty;
            return e.lines[UnityEngine.Random.Range(0, e.lines.Count)];
        }
    }

    [CreateAssetMenu(menuName="Soulbound Legends/NPC/Content Database", fileName="NPC_ContentDatabase")]
    public class NPCContentDatabase : ScriptableObject
    {
        [Serializable] public class ContentEntry
        {
            public string id; public string displayName; [TextArea] public string description; public Sprite icon; public GameObject preview;
            public string category; public string rarity; public int levelRequirement; public int goldPrice; public bool craftable; public bool buyable; public bool sellable;
            public List<string> requiredMaterials = new(); public List<int> requiredQuantities = new(); public List<string> availableRegions = new(); public List<string> availableNPCs = new(); public string unlockCondition;
        }
        public List<ContentEntry> potions = new();
        public List<ContentEntry> weapons = new();
        public List<ContentEntry> armor = new();
        public List<ContentEntry> materials = new();
        public List<ContentEntry> recipes = new();
        public List<ContentEntry> shopItems = new();
        public List<NPCOccupationDefinition> npcServices = new();
        public List<string> regions = new();
    }

    [Serializable]
    public class NPCNavigationData
    {
        public Collider regionBoundary;
        public Transform home;
        public Transform workplace;
        public List<Transform> allowedAreas = new();
        public List<Transform> forbiddenAreas = new();
        public List<Transform> destinations = new();
        public List<Transform> patrolRoutes = new();
        public List<Transform> interactionLocations = new();
        public bool canLeaveRegion;
        public bool canTravelOutside;
    }

    [Serializable]
    public class NPCRoutineStep
    {
        public NPCRoutineAction action;
        public float duration = 30f;
        public Transform target;
    }

    [Serializable]
    public class NPCServiceBinding
    {
        public NPCServiceType service;
        public string customServiceId;
        public bool enabled = true;
    }

    [Serializable]
    public class NPCGeneratedIdentity
    {
        public string npcId;
        public string displayName;
        public string title;
        public NPCGender gender;
        public int age;
        public string personality;
        public string description;
        public string faction;
        public NPCImportance importance;
        public string storyRole;
        public string occupationId;
        public int uniqueSeed;
    }

    [Serializable]
    public class NPCDialogueContext
    {
        public string timeOfDay;
        public string regionId;
        public bool firstMeeting;
        public bool hasActiveQuest;
        public bool storyEventActive;
        public string customContext;
    }
}
