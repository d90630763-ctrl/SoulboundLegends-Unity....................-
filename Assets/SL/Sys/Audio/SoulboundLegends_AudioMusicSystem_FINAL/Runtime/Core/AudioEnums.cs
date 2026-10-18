using System;
namespace SoulboundLegends.AudioMusic
{
    public enum AudioCategory { Master, Music, SFX, Voice, UI, Environment, Combat, Ambient, System }
    public enum SoundType {
        None, Footstep, Walk, Run, Sprint, Jump, Land, Fall, Swim, Climb, Slide, Dodge,
        Attack, WeaponSwing, WeaponImpact, Block, Parry, Damage, Death, Heal, Buff, Debuff, Status,
        SkillCast, SkillCharge, SkillActivate, ProjectileLaunch, ProjectileTravel, SkillImpact, Explosion,
        AreaEffect, Shield, Transformation, Ultimate, SkillCancel, Equip, Unequip, Critical, WeaponAbility,
        WeaponBreak, MonsterIdle, MonsterMovement, MonsterAttack, MonsterAbility, MonsterDamage, MonsterEnrage,
        MonsterPhase, BossIntro, BossTransition, BossDefeat, DoorOpen, DoorClose, Portal, Teleport,
        WaterEnter, WaterExit, Splash, Dive, Waterfall, Wave, Underwater, Weather, Rain, HeavyRain, Thunder,
        Wind, Storm, Snow, Destruction, Debris, Collapse, QuestAccepted, QuestComplete, Reward, ItemAcquire,
        ItemEquip, ItemCraft, Matchmaking, PartyInvite, FriendRequest, Mail, Chat, Notification, Warning,
        Error, Confirmation, MenuOpen, MenuClose, ButtonPress, ButtonHover, TabSwitch, LevelUp, RankUp,
        RiftAppear, RiftActivate, RiftWarning, RiftAmbience, RiftExpansion, RiftEngulf, RiftBoss, RiftClear,
        RiftFailure, RuinDiscovery, RuinEntrance, RuinTrialStart, DivineIntro, DivineAbility, DivineAttack,
        DivineDefeat, DivineFinalDefeat, GodInheritance, ServerEvent, Custom
    }
    public enum MovementState { Idle, Walk, Run, Sprint, Crouch, Jump, Fall, Swim, Climb, Slide }
    public enum SurfaceType { Auto, Grass, Dirt, Stone, Rock, Sand, Wood, Metal, Ice, Snow, Water, Mud, Swamp, Fantasy, Custom }
    public enum ElementType { None, Fire, Water, Earth, Void, Thunder, Nature, Light, Air, Ice, Shadow, Omnigod }
    public enum WeaponType { None, Sword, Dagger, Spear, Axe, Bow, Staff, Magic, Ranged, Custom }
    public enum MusicContext {
        OpeningScreen, Login, LoadingScreen, CharacterCreation, Lobby, AdventureWorld, Village, Kingdom, City,
        CS, BattleRoyale, Siege, Dungeon, BossBattle, Rift, SupremeRuin, Victory, Defeat, MainMenu, Settings,
        Shop, SpecialEvent, Custom
    }
    public enum MusicSelectionMode { Sequential, Random, WeightedRandom, ShuffleNoImmediateRepeat }
    public enum MusicTransitionMode { Fade, Crossfade, Instant }
    public enum AudioQuality { Low, Medium, High, VeryHigh }
}
