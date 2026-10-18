#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using SoulboundLegends.WorldNPC;

public static class NPCStarterDatabaseMenu
{
    [MenuItem("Soulbound Legends/World NPC/Create Starter Databases")]
    public static void Create()
    {
        string root = "Assets/SoulboundLegends/WorldNPC/GeneratedData";
        EnsureFolder("Assets/SoulboundLegends/WorldNPC", "GeneratedData");
        var db = ScriptableObject.CreateInstance<NPCOccupationDatabase>();
        string[] names = {
            "king","queen","crown_prince","crown_princess","prince","princess","royal_advisor","royal_secretary","royal_treasurer","royal_mage","royal_physician","royal_scribe","royal_messenger","royal_diplomat","noble","duke","duchess","marquis","count","baron",
            "royal_guard","knight","knight_captain","guard_captain","city_guard","gate_guard","palace_guard","patrol_guard","watchtower_guard","archer_guard","mage_guard","cavalry","general","soldier","recruit","prison_guard","border_guard",
            "guild_master","guild_receptionist","quest_clerk","rank_examiner","guild_trainer","guild_merchant","guild_blacksmith","guild_alchemist","guild_scout","guild_messenger","bounty_clerk","monster_researcher","adventurer",
            "merchant","weapon_merchant","armor_merchant","potion_merchant","magic_item_merchant","material_merchant","food_merchant","general_storekeeper","jeweler","tailor","carpenter","blacksmith","alchemist","baker","butcher","fisherman","farmer","miner","hunter","mason","builder","cook","innkeeper","bartender","stable_keeper","animal_handler","healer","teacher","scholar","librarian","priest","priestess","craftsman","artisan","servant","maid","messenger","street_vendor","child","elder","traveler"
        };
        foreach (var id in names)
        {
            var o = ScriptableObject.CreateInstance<NPCOccupationDefinition>(); o.occupationId = id; o.displayName = ToDisplay(id); o.description = "Starter editable occupation definition.";
            o.allowedServices.Add(NPCServiceType.Talk);
            if (id.Contains("blacksmith")) { o.allowedServices.Add(NPCServiceType.Craft); o.allowedServices.Add(NPCServiceType.Repair); o.allowedServices.Add(NPCServiceType.Upgrade); o.allowedServices.Add(NPCServiceType.Shop); }
            if (id.Contains("alchemist") || id == "healer") { o.allowedServices.Add(NPCServiceType.Alchemy); o.allowedServices.Add(NPCServiceType.Shop); }
            if (id.Contains("merchant") || id.Contains("storekeeper") || id == "jeweler" || id == "tailor" || id == "baker" || id == "butcher" || id == "innkeeper" || id == "bartender") o.allowedServices.Add(NPCServiceType.Shop);
            if (id.Contains("guard") || id == "knight" || id == "soldier" || id == "general" || id == "cavalry" || id == "recruit") { o.allowedServices.Add(NPCServiceType.Information); o.allowedServices.Add(NPCServiceType.ReportCrime); }
            if (id.Contains("guild") || id == "quest_clerk" || id == "bounty_clerk" || id == "adventurer") o.allowedServices.Add(NPCServiceType.Quest);
            if (id == "rank_examiner") o.allowedServices.Add(NPCServiceType.RankTest);
            if (id == "guild_receptionist") o.allowedServices.Add(NPCServiceType.GuildRegistration);
            if (id == "guild_trainer") o.allowedServices.Add(NPCServiceType.Train);
            if (id == "innkeeper") o.allowedServices.Add(NPCServiceType.Heal);
            if (id == "king" || id == "queen" || id.Contains("prince") || id == "princess" || id == "duke" || id == "duchess" || id == "marquis" || id == "count" || id == "baron" || id == "noble") o.allowedServices.Add(NPCServiceType.RoyalAudience);
            string path = AssetDatabase.GenerateUniqueAssetPath(root + "/Occupation_" + id + ".asset"); AssetDatabase.CreateAsset(o, path); db.occupations.Add(o);
        }
        AssetDatabase.CreateAsset(db, AssetDatabase.GenerateUniqueAssetPath(root + "/WorldNPC_OccupationDatabase.asset"));
        var dialogue = ScriptableObject.CreateInstance<NPCDialogueDatabase>();
        Add(dialogue,"blacksmith","Need something repaired?","The forge is open."); Add(dialogue,"alchemist","Looking for a potion?","I can help with that."); Add(dialogue,"guard","State your business.","Keep moving, traveler."); Add(dialogue,"guild","Welcome to the Adventure Guild.","Looking for a quest?"); Add(dialogue,"merchant","Take a look at my wares.","See anything you like?"); Add(dialogue,"farmer","Good day, traveler.","The fields are busy today."); Add(dialogue,"default","Hello, traveler.","Good day.");
        AssetDatabase.CreateAsset(dialogue, AssetDatabase.GenerateUniqueAssetPath(root + "/WorldNPC_DialogueDatabase.asset"));
        AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); Selection.activeObject = db; EditorUtility.DisplayDialog("World NPC", "Starter occupation and dialogue databases created.", "OK");
    }
    static void Add(NPCDialogueDatabase db,string tag,params string[] lines){var e=new NPCDialogueDatabase.Entry{tag=tag}; e.lines.AddRange(lines); db.entries.Add(e);}
    static string ToDisplay(string s){var a=s.Replace('_',' ').Split(' '); for(int i=0;i<a.Length;i++) if(a[i].Length>0)a[i]=char.ToUpper(a[i][0])+a[i].Substring(1); return string.Join(" ",a);}
    static void EnsureFolder(string parent,string name){if(!AssetDatabase.IsValidFolder(parent+"/"+name))AssetDatabase.CreateFolder(parent,name);}
}
#endif
