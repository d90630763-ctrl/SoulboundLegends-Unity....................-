namespace SoulboundLegends.PlayerDatabaseBackend;
public sealed record PlayerSnapshot(Guid PlayerId,Guid AccountId,Guid CharacterId,string CharacterName,string Gender,string? Face,string? Hair,string? Appearance,int Level,long Xp,string Rank,long Version,DateTimeOffset UpdatedAt,object? Data);
public sealed record MutationRequest(string Operation,string IdempotencyKey,long ExpectedVersion,object? Payload);
public sealed record MutationResult(string IdempotencyKey,long NewVersion,object? Result);
public sealed record EconomyMutation(string Currency,long Delta,string Reason,string IdempotencyKey,long ExpectedVersion);
public enum RiftState{Active,Escalated,Merging,Engulfed,Cleared,Failed}
public enum RuinState{Locked,Available,InChallenge,Claimed,Completed}
