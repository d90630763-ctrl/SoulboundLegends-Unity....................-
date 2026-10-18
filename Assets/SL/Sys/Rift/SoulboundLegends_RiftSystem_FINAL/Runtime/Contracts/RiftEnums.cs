using System;

namespace SoulboundLegends.RiftSystem
{
    public enum RiftState
    {
        Normal = 0,
        Appearing = 1,
        Active = 2,
        InProgress = 3,
        BossActive = 4,
        Cleared = 5,
        TimeExpired = 6,
        AttemptsExhausted = 7,
        Merging = 8,
        Engulfed = 9,
        CityDestroyed = 10,
        Failed = 11
    }

    public enum RiftFailureReason
    {
        None = 0,
        TimerExpired = 1,
        AttemptsExhausted = 2,
        CityDestroyed = 3,
        Administrative = 4,
        InfrastructureFailure = 5
    }

    public enum RiftDiscoveryType
    {
        Appearance = 0,
        Encounter = 1,
        Monster = 2,
        Boss = 3,
        Completion = 4,
        Failure = 5,
        WorldProgression = 6
    }

    public enum RiftObjectiveType
    {
        KillFinalBoss = 0
    }

    public enum RiftParticipantStatus
    {
        Outside = 0,
        Inside = 1,
        Dead = 2,
        Disconnected = 3,
        Completed = 4,
        Punished = 5
    }

    public enum RiftWorldConsequence
    {
        None = 0,
        Engulfed = 1,
        Destroyed = 2
    }

    public enum RiftRequestResult
    {
        Accepted = 0,
        Rejected = 1,
        NotFound = 2,
        InvalidState = 3,
        Unauthorized = 4,
        InvalidDistance = 5,
        CapacityReached = 6,
        AttemptsUnavailable = 7,
        Cooldown = 8,
        RateLimited = 9,
        InfrastructureUnavailable = 10,
        AlreadyParticipant = 11,
        NotParticipant = 12,
        AlreadyCompleted = 13,
        InvalidNonce = 14,
        InvalidRequest = 15
    }
}
