namespace SoulboundLegends.SupremeRuin
{
    public enum SupremeGodId { Water, Fire, Earth, Void, Thunder, Nature, Light, Air, Ice, Omnigod, Shadow }
    public enum DivineElement { Water, Fire, Earth, Void, Thunder, Nature, Light, Air, Ice, MultiElement, Shadow }
    public enum RuinState { Dormant, Available, Occupied, TrialStarting, TrialActive, Completed, Failed, Terminating, Corrupted }
    public enum TrialState { None, Starting, Active, CreatureProgress, Completed, Failed, Terminated }
    public enum CreatureSpawnMode { Sequential, Simultaneous, PhaseBased }
    public enum TrialFailureReason { None, PlayerDeath, Abandoned, DisconnectTimeout, ServerTermination, InvalidState, Administrative }
    public enum InheritanceAwardState { None, Pending, Awarded, Rejected, Failed }
    public enum RequirementType { Item, Level, Quest, Story, World, GodCondition, Permission, Discovery, Custom }
    public enum LoreVisibility { Public, HiddenUntilDiscovery, HiddenUntilStory, ServerOnly }
    public enum DivineRole { Aggressor, Defender, Ranged, Controller, UltimateThreat, Custom }
}
