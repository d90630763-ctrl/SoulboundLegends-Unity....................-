using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.InstanceCoordinator
{
    [CreateAssetMenu(menuName="Soulbound Legends/Instance Coordinator/Configuration", fileName="InstanceCoordinatorConfig")]
    public sealed class InstanceCoordinatorConfig : ScriptableObject
    {
        [Min(1)] public int DefaultMaximumCapacity = 100;
        [Min(0)] public int DefaultMinimumCapacity = 1;
        [Min(1)] public int ReservationTimeoutSeconds = 20;
        [Min(5)] public int HeartbeatTimeoutSeconds = 15;
        [Min(1)] public int IdleTimeoutSeconds = 300;
        [Min(1)] public int CleanupTimeoutSeconds = 60;
        [Min(1)] public int MaximumInstances = 10000;
        [Min(0.1f)] public float RegistryRefreshSeconds = 5f;
        public bool PreferPartyAffinity = true;
        public bool RejectUnhealthyServers = true;
        public List<RegionSetting> Regions = new();
        public List<InstanceTypeProfile> InstanceTypeProfiles = new();

        [Serializable] public sealed class RegionSetting { public string Region = "global"; public int Weight = 100; public bool Enabled = true; }
        [Serializable] public sealed class InstanceTypeProfile
        {
            public string Type = "generic";
            public int DefaultCapacity = 100;
            public int MaximumCapacity = 100;
            public int IdleTimeoutSeconds = 300;
            public bool PersistentByDefault;
            public bool PartyMustRemainTogether = true;
        }
    }
}
