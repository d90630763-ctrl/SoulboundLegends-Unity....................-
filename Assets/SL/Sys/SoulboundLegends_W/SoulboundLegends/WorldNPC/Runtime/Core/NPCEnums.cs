using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.WorldNPC
{
    public enum NPCGenerationType { Primary, Secondary }
    public enum NPCGender { Male, Female }
    public enum NPCImportance { Background, Normal, Important, StoryCritical, Hidden }
    public enum NPCServiceType { Talk, Quest, Shop, Craft, Alchemy, Repair, Upgrade, Trade, Train, RankTest, Bounty, GuildRegistration, Information, Heal, Party, Teleport, RoyalAudience, ReportCrime, Prison, SpecialEvent, HiddenInteraction, Custom }
    public enum NPCRoutineAction { Wake, GoToWork, Work, Eat, Rest, Talk, Wander, Patrol, Travel, Sleep, Emergency, Combat, ReturnHome, Custom }
    public enum NPCSystemCapability { Voice, Dialogue, Appearance, Navigation, Quest, Crafting, Alchemy, Shop, Interaction, UI, ServerAuthorization, Inventory, Equipment, Reputation, Guild, Bounty, Party, Teleport, Combat, Custom }

    [Serializable]
    public class NPCStringEntry { public string key; [TextArea] public string value; }

    [Serializable]
    public class NPCFloatRange { public float min = 0f; public float max = 1f; public float RandomValue() => UnityEngine.Random.Range(min, max); }
}
