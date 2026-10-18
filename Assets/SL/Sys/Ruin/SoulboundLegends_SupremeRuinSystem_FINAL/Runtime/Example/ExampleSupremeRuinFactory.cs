using UnityEngine;

namespace SoulboundLegends.SupremeRuin
{
    [CreateAssetMenu(menuName="Soulbound Legends/Supreme Ruin/Create Example Configuration")]
    public sealed class ExampleSupremeRuinFactory : ScriptableObject
    {
        public SupremeRuinConfiguration Target;
        public void ApplyExampleDefaults()
        {
            if (Target == null) return;
            Target.Gods.Clear();
            Target.DivineCreatures.Clear();
            Target.Ruins.Clear();

            string[] ids = { "Water","Fire","Earth","Void","Thunder","Nature","Light","Air","Ice","Omnigod" };
            DivineElement[] elements = { DivineElement.Water,DivineElement.Fire,DivineElement.Earth,DivineElement.Void,
                DivineElement.Thunder,DivineElement.Nature,DivineElement.Light,DivineElement.Air,DivineElement.Ice,DivineElement.MultiElement };

            for (int i=0;i<ids.Length;i++)
            {
                Target.Gods.Add(new GodDefinition {
                    Id=ids[i], God=(SupremeGodId)i, DisplayName=ids[i]+" God",
                    Element=elements[i], InheritanceId=ids[i]+"Inheritance",
                    PlayerFacingEarlyLoreAllowed=true
                });
            }

            // Example configuration is executable data, not gameplay hard-coding.
            for (int g=0;g<ids.Length;g++)
            {
                var ruin = new SupremeRuinDefinition {
                    Id=ids[g]+"SupremeRuin", DisplayName=ids[g]+" Supreme Ruin",
                    GodId=ids[g], KingdomId="CONFIGURE_KINGDOM",
                    EntranceDefinitionId="CONFIGURE_ENTRANCE",
                    InfrastructureDefinitionId="CONFIGURE_INFRASTRUCTURE",
                    TrialEnvironmentDefinitionId="CONFIGURE_TRIAL_ENVIRONMENT",
                    TrialDurationSeconds=3600, MaxPlayers=1, AllowParty=false
                };

                for (int n=1;n<=5;n++)
                {
                    string cid = ids[g]+"DivineCreature"+n;
                    Target.DivineCreatures.Add(new DivineCreatureDefinition {
                        Id=cid, SpeciesTemplateId="CONFIGURE_SPECIES_TEMPLATE",
                        DisplayName=ids[g]+" Divine Creature "+n,
                        God=(SupremeGodId)g, Element=elements[g],
                        Role=(DivineRole)Mathf.Min(n-1,4), BaseLevel=100
                    });
                    ruin.RequiredCreatures.Add(new DivineCreatureTrialSlot {
                        CreatureDefinitionId=cid, RequiredIndex=n,
                        SpawnPointId="CONFIGURE_SPAWN_POINT_"+n
                    });
                }
                Target.Ruins.Add(ruin);
            }
        }
    }
}
