#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using SoulboundLegends.AdventureWorld;

[CustomEditor(typeof(AdventureWorldSystem))]
public class AdventureWorldSystemEditor : Editor
{
    bool showSkills = true;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var system = (AdventureWorldSystem)target;
        EditorGUILayout.Space(12);
        EditorGUILayout.LabelField("SOULBOUND LEGENDS — FINAL ADVENTURE WORLD", EditorStyles.boldLabel);

        if (GUILayout.Button("AUTO-CREATE ALL CONNECTION SLOTS"))
        {
            string[] names = {"Terrain","Fantasy Infrastructure","General NPC","Monster/Human Generation","General Monster AI","General Monster Skill","Portal","Crafting","Materials/Plants","Server Authorization","Networking","Character","Player Script","Player Character","Character Customization","Character Controller","Camera","General HUD","Combat","Skills","Skill Animation","Equipment","Inventory","Stats/Battle Power","Animation","Housing","Guild","Party/Friends","Auction House","Cloud/Weather","Lighting","Water","Destruction","Quest","Rewards","Rank Advancement","Rank/Season","Game State","Time Sync","Localization","Analytics","Reporting","Scene Loading"};
            foreach (var n in names) if (!system.systemConnections.Exists(x => x.systemName == n)) system.systemConnections.Add(new AdventureWorldSystemSlot { systemName = n, capability = n });
            EnsureSkillSystem(system);
            EditorUtility.SetDirty(system);
        }
        if (GUILayout.Button("VALIDATE ADVENTURE WORLD"))
        {
            List<string> errors = AdventureWorldGenerationValidation.Validate(system);
            if (errors.Count == 0) EditorUtility.DisplayDialog("Adventure World", "VALIDATION PASSED", "OK");
            else EditorUtility.DisplayDialog("Adventure World", string.Join("\n", errors), "OK");
        }
        if (GUILayout.Button("GENERATE SELECTED WORLD"))
        {
            var generator = system.GetComponent<AdventureWorldGenerator>() ?? system.gameObject.AddComponent<AdventureWorldGenerator>();
            generator.worldSystem = system; generator.GenerateSelectedWorld(); EditorUtility.SetDirty(generator);
        }
        if (GUILayout.Button("GENERATE NEW CONTENT ONLY"))
        {
            var generator = system.GetComponent<AdventureWorldGenerator>() ?? system.gameObject.AddComponent<AdventureWorldGenerator>();
            generator.worldSystem = system; generator.GenerateNewContentOnly(); EditorUtility.SetDirty(generator);
        }
        if (GUILayout.Button("REVEAL GENERATED ARTIFACTS")) Selection.activeObject = system.GetComponent<AdventureWorldArtifactSystem>() ?? system.gameObject.AddComponent<AdventureWorldArtifactSystem>();
        if (GUILayout.Button("REVEAL GENERATED MATERIALS")) Selection.activeObject = system.GetComponent<AdventureWorldMaterialSystem>() ?? system.gameObject.AddComponent<AdventureWorldMaterialSystem>();
        if (GUILayout.Button("REVEAL AUCTION / TRANSACTION REGISTRY")) Selection.activeObject = system.GetComponent<AdventureWorldAuctionHouseSystem>() ?? system.gameObject.AddComponent<AdventureWorldAuctionHouseSystem>();

        EditorGUILayout.Space(8);
        var skills = EnsureSkillSystem(system);
        showSkills = EditorGUILayout.Foldout(showSkills, "SKILL PAVILION + WORLD SKILL REGISTRY", true, EditorStyles.foldoutHeader);
        if (!showSkills) return;

        EditorGUILayout.HelpBox("Normal-to-mid grade skills can be sold in the Skill Pavilion. High-grade skills are normally discovered in ruins, events, or from high-rank monsters. Collaboration/season skills may be placed in the Pavilion. Purchases are server-authorized.", MessageType.Info);

        if (GUILayout.Button("+ ADD SKILL"))
        {
            Undo.RecordObject(skills, "Add Adventure Skill");
            skills.AddSkill(new AdventureSkillRecord { skillId = "skill_new_" + System.Guid.NewGuid().ToString("N").Substring(0, 8), skillName = "New Skill", pavilionEnabled = true });
            EditorUtility.SetDirty(skills);
        }
        if (GUILayout.Button("SYNC SKILLS FROM GENERAL SKILL + ANIMATION SYSTEMS"))
        {
            Undo.RecordObject(skills, "Sync Adventure Skills");
            skills.SyncFromProviders();
            EditorUtility.SetDirty(skills);
        }
        if (GUILayout.Button("VALIDATE SKILL REGISTRY"))
        {
            var errors = AdventureWorldGenerationValidation.Validate(system);
            if (errors.Count == 0) EditorUtility.DisplayDialog("Skills", "SKILL REGISTRY VALID", "OK");
            else EditorUtility.DisplayDialog("Skills", string.Join("\n", errors), "OK");
        }

        for (int i = 0; i < skills.skills.Count; i++)
        {
            var skill = skills.skills[i];
            if (skill == null) continue;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(string.IsNullOrEmpty(skill.skillName) ? "Unnamed Skill" : skill.skillName, EditorStyles.boldLabel);
            if (GUILayout.Button("-", GUILayout.Width(24))) { skills.skills.RemoveAt(i); EditorUtility.SetDirty(skills); EditorGUILayout.EndHorizontal(); EditorGUILayout.EndVertical(); break; }
            EditorGUILayout.EndHorizontal();

            skill.skillId = EditorGUILayout.TextField("Skill ID", skill.skillId);
            skill.skillName = EditorGUILayout.TextField("Name", skill.skillName);
            skill.tier = (AdventureSkillTier)EditorGUILayout.EnumPopup("Tier", skill.tier);
            skill.rarity = (AdventureSkillRarity)EditorGUILayout.EnumPopup("Rarity", skill.rarity);
            skill.description = EditorGUILayout.TextArea(skill.description, GUILayout.MinHeight(40));
            skill.generalSkillDefinitionId = EditorGUILayout.TextField("General Skill Definition", skill.generalSkillDefinitionId);
            skill.animationProviderId = EditorGUILayout.TextField("Animation Provider", skill.animationProviderId);

            EditorGUILayout.Space(3);
            EditorGUILayout.LabelField("WHERE SHOULD THIS SKILL APPEAR?", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Toggle(skill.appearsInRuin, "RUINS", "Button")) { skill.appearsInRuin = true; skill.primarySource = AdventureSkillSource.Ruin; }
            if (GUILayout.Toggle(skill.appearsInPavilion, "PAVILION", "Button")) { skill.appearsInPavilion = true; skill.primarySource = AdventureSkillSource.SkillPavilion; }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove from Ruins")) skill.appearsInRuin = false;
            if (GUILayout.Button("Remove from Pavilion")) skill.appearsInPavilion = false;
            EditorGUILayout.EndHorizontal();

            if (skill.appearsInPavilion)
            {
                skill.pavilionPriceAether = EditorGUILayout.LongField("Pavilion Price (Aether)", skill.pavilionPriceAether);
                skill.pavilionPriceSet = true;
                skill.pavilionCategory = EditorGUILayout.TextField("Pavilion Category", skill.pavilionCategory);
                skill.collaborationOrSeasonOnly = EditorGUILayout.Toggle("Collab / Season Only", skill.collaborationOrSeasonOnly);
            }
            if (skill.appearsInRuin)
            {
                skill.regionId = EditorGUILayout.TextField("Ruin Region ID", skill.regionId);
                skill.locationId = EditorGUILayout.TextField("Ruin / Discovery Location", skill.locationId);
            }
            skill.appearsAsHighRankMonsterDrop = EditorGUILayout.Toggle("High-Rank Monster Drop", skill.appearsAsHighRankMonsterDrop);
            if (skill.appearsAsHighRankMonsterDrop) skill.requiredMonsterId = EditorGUILayout.TextField("Required Monster", skill.requiredMonsterId);
            skill.appearsInEvents = EditorGUILayout.Toggle("Event Skill", skill.appearsInEvents);
            skill.generationVersion = EditorGUILayout.TextField("Generation Version", skill.generationVersion);
            skill.discovered = EditorGUILayout.Toggle("Discovered", skill.discovered);
            skill.claimed = EditorGUILayout.Toggle("Claimed", skill.claimed);
            if (skill.claimed) skill.ownerId = EditorGUILayout.TextField("Owner ID", skill.ownerId);
            EditorGUILayout.EndVertical();
        }

        EditorUtility.SetDirty(skills);
    }

    AdventureWorldSkillSystem EnsureSkillSystem(AdventureWorldSystem system)
    {
        var skills = system.GetComponent<AdventureWorldSkillSystem>();
        if (skills == null) skills = system.gameObject.AddComponent<AdventureWorldSkillSystem>();
        skills.enableSkillPavilion = system.enableSkillPavilion;
        return skills;
    }
}
#endif
