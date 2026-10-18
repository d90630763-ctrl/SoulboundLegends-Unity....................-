using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftSystem : MonoBehaviour
    {
        [SerializeField] private RiftConfiguration configuration;
        [SerializeField] private RiftSystemConnectionHub connectionHub;

        private RiftAuthorityGuard authority;
        private RiftStateMachine stateMachine;
        private RiftPersistenceService persistence;
        private RiftEventDispatcher events;
        private readonly Dictionary<string, RiftStateSnapshot> active = new();
        private readonly Dictionary<string, long> nextSpawnAt = new();
        private readonly Dictionary<string, int> activeSpawnCounts = new();

        public RiftSystemConnectionHub Connections => connectionHub;
        public IReadOnlyDictionary<string, RiftStateSnapshot> ActiveRifts => active;

        private void Awake()
        {
            if (configuration == null)
                throw new InvalidOperationException("RiftSystem requires a RiftConfiguration.");
            if (connectionHub == null)
                throw new InvalidOperationException("RiftSystem requires a RiftSystemConnectionHub.");

            connectionHub.Rebuild();
            authority = new RiftAuthorityGuard(connectionHub, configuration);
            stateMachine = new RiftStateMachine();
            persistence = new RiftPersistenceService(connectionHub);
            events = new RiftEventDispatcher(connectionHub, authority);
        }

        private void Start()
        {
            if (!connectionHub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider server) ||
                !server.IsAuthoritativeServer)
                return;

            foreach (var snapshot in connectionHub.GetAll<IRiftPersistenceProvider>(RiftCapability.Persistence)
                         .SelectMany(x => x.LoadActiveRifts()))
                active[snapshot.RiftId] = snapshot;
        }

        private void Update()
        {
            if (authority == null || !connectionHub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider server) ||
                !server.IsAuthoritativeServer)
                return;

            long now = authority.ServerNow();

            foreach (var snapshot in active.Values.ToArray())
                Tick(snapshot, now);

            TryScheduleWorldSpawn(now);
        }

        private void Tick(RiftStateSnapshot rift, long now)
        {
            switch (rift.State)
            {
                case RiftState.Active:
                case RiftState.InProgress:
                case RiftState.BossActive:
                    TickRiftContent(rift, now);
                    if (now >= rift.ExpirationUnixMilliseconds)
                    {
                        if (stateMachine.TryTransition(rift, RiftState.TimeExpired, now, out _))
                        {
                            events.Emit(events.Create("RiftTimeExpired", rift));
                            Persist(rift);
                            BeginMerging(rift);
                        }
                        return;
                    }
                    break;

                case RiftState.Merging:
                    if (stateMachine.TryTransition(rift, RiftState.Engulfed, now, out _))
                        Engulf(rift, now);
                    break;

                case RiftState.Engulfed:
                    TickInvasion(rift, now);
                    break;
            }
        }

        private void TryScheduleWorldSpawn(long now)
        {
            if (active.Values.Any(x => x.State != RiftState.Cleared && x.State != RiftState.CityDestroyed && x.State != RiftState.Failed))
                return;

            if (!nextSpawnAt.TryGetValue("global", out long next))
            {
                nextSpawnAt["global"] = now + RandomSeconds();
                return;
            }

            if (now < next)
                return;

            if (configuration.WorldConditions.Any(c =>
                !connectionHub.TryGet(RiftCapability.AdventureWorld, out IRiftAdventureWorldProvider world) ||
                !world.IsWorldConditionSatisfied(c)))
            {
                nextSpawnAt["global"] = now + 30000;
                return;
            }

            if (TryCreateRift(null, null, out _))
                nextSpawnAt["global"] = now + RandomSeconds();
            else
                nextSpawnAt["global"] = now + 60000;
        }

        private int RandomSeconds() =>
            UnityEngine.Random.Range(configuration.MinimumSecondsBetweenRifts,
                configuration.MaximumSecondsBetweenRifts + 1);

        public bool TryCreateRift(string forcedCityId, string forcedTypeId, out string riftId)
        {
            riftId = null;

            if (!connectionHub.TryGet(RiftCapability.AdventureWorld, out IRiftAdventureWorldProvider world))
                return false;

            if (!world.TryGetValidCity(forcedCityId, out RiftCityContext city) || !city.IsValid)
                return false;

            RiftTypeDefinition type = SelectType(forcedTypeId);
            if (type == null)
                return false;

            if (!connectionHub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider server) ||
                !server.IsAuthoritativeServer)
                return false;

            long now = server.ServerUnixMilliseconds;
            riftId = Guid.NewGuid().ToString("N");

            var snapshot = new RiftStateSnapshot
            {
                SchemaVersion = configuration.StateSchemaVersion,
                RiftId = riftId,
                RiftTypeId = type.Id,
                RiftDifficultyId = SelectDifficulty()?.Id ?? "unknown",
                KingdomId = city.KingdomId,
                CityId = city.CityId,
                State = RiftState.Normal,
                AttemptLimit = Mathf.Max(1, type.AttemptLimit > 0 ? type.AttemptLimit : configuration.DefaultAttemptLimit),
                SpawnUnixMilliseconds = now,
                StateChangedUnixMilliseconds = now,
                BossDefinitionId = SelectBoss(type),
                WorldConsequence = RiftWorldConsequence.None,
                CurrentWave = 0,
                NextWaveUnixMilliseconds = now
            };

            if (!stateMachine.TryTransition(snapshot, RiftState.Appearing, now, out _))
                return false;

            active.Add(riftId, snapshot);
            events.Emit(events.Create("RiftSpawned", snapshot));
            PublishDiscovery(snapshot, RiftDiscoveryType.Appearance, "rift.appeared", null, null);
            PublishQuest(snapshot);
            Persist(snapshot);

            if (!stateMachine.TryTransition(snapshot, RiftState.Active, now, out _))
                return false;

            snapshot.ExpirationUnixMilliseconds = now + (long)Mathf.Max(1, type.DurationSeconds > 0 ? type.DurationSeconds : configuration.DefaultDurationSeconds) * 1000L;
            events.Emit(events.Create("RiftDiscovered", snapshot));
            Persist(snapshot);
            return true;
        }

        public RiftRequestResult EnterRift(string playerId, string riftId, string nonce)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return RiftRequestResult.NotFound;

            if (!authority.RequireServer("EnterRift", playerId, riftId, rift.CityId, nonce, out var authResult))
                return authResult;

            if (rift.State != RiftState.Active && rift.State != RiftState.InProgress && rift.State != RiftState.BossActive)
                return RiftRequestResult.InvalidState;

            if (rift.AttemptsConsumed >= rift.AttemptLimit)
                return RiftRequestResult.AttemptsUnavailable;

            if (rift.Participants.Any(x => x.PlayerId == playerId && x.Status == RiftParticipantStatus.Inside))
                return RiftRequestResult.AlreadyParticipant;

            if (!connectionHub.TryGet(RiftCapability.DynamicInstance, out IRiftInstanceProvider instances))
                return RiftRequestResult.InfrastructureUnavailable;

            if (rift.CurrentInstanceId == null)
            {
                var type = GetType(rift.RiftTypeId);
                if (!instances.TryCreateInstance(new RiftInstanceRequest
                    {
                        RiftId = rift.RiftId,
                        CityId = rift.CityId,
                        Capacity = type?.InstanceCapacity ?? configuration.DefaultInstanceCapacity,
                        PartyAllowed = type?.AllowParties ?? configuration.AllowParties
                    }, out var result) || !result.Created)
                    return RiftRequestResult.InfrastructureUnavailable;

                rift.CurrentInstanceId = result.InstanceId;
                events.Emit(events.Create("RiftInstanceCreated", rift, playerId));
            }

            if (instances.TryGetInstancePlayerCount(rift.CurrentInstanceId, out int count))
            {
                var type = GetType(rift.RiftTypeId);
                int capacity = type?.InstanceCapacity ?? configuration.DefaultInstanceCapacity;
                if (count >= capacity)
                    return RiftRequestResult.CapacityReached;
            }

            if (!instances.TryMovePlayerToInstance(playerId, rift.CurrentInstanceId))
                return RiftRequestResult.Rejected;

            var participant = rift.Participants.FirstOrDefault(x => x.PlayerId == playerId);
            if (participant == null)
            {
                participant = new RiftParticipantRecord { PlayerId = playerId };
                rift.Participants.Add(participant);
            }

            participant.Status = RiftParticipantStatus.Inside;
            participant.EnteredUnixMilliseconds = authority.ServerNow();
            participant.LastSeenUnixMilliseconds = participant.EnteredUnixMilliseconds;
            rift.AttemptsConsumed++;

            if (rift.State == RiftState.Active)
                stateMachine.TryTransition(rift, RiftState.InProgress, authority.ServerNow(), out _);

            events.Emit(events.Create("RiftPlayerEntered", rift, playerId));
            events.Emit(events.Create("RiftAttemptConsumed", rift, playerId));
            Persist(rift);
            return RiftRequestResult.Accepted;
        }

        public RiftRequestResult ExitRift(string playerId, string riftId, string nonce)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return RiftRequestResult.NotFound;

            if (!authority.RequireServer("ExitRift", playerId, riftId, rift.CityId, nonce, out var authResult))
                return authResult;

            var participant = rift.Participants.FirstOrDefault(x => x.PlayerId == playerId);
            if (participant == null || participant.Status != RiftParticipantStatus.Inside)
                return RiftRequestResult.NotParticipant;

            if (rift.State == RiftState.Engulfed)
            {
                authority.Report(playerId, riftId, "ImpossibleRiftExit", "Player attempted to leave an engulfed Rift.", 5);
                return RiftRequestResult.Rejected;
            }

            if (rift.State != RiftState.Cleared)
                return RiftRequestResult.InvalidState;

            if (connectionHub.TryGet(RiftCapability.DynamicInstance, out IRiftInstanceProvider instances) &&
                rift.CurrentInstanceId != null)
                instances.TryRemovePlayerFromInstance(playerId, rift.CurrentInstanceId);

            participant.Status = RiftParticipantStatus.Completed;
            participant.LastSeenUnixMilliseconds = authority.ServerNow();
            events.Emit(events.Create("RiftPlayerLeft", rift, playerId));
            Persist(rift);
            return RiftRequestResult.Accepted;
        }

        public RiftRequestResult RequestBossDefeat(string playerId, string riftId, string bossSpawnId, string nonce)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return RiftRequestResult.NotFound;

            if (!authority.RequireServer("BossDefeat", playerId, riftId, rift.CityId, nonce, out var authResult))
                return authResult;

            if (rift.State != RiftState.BossActive || rift.BossDefeated)
                return RiftRequestResult.InvalidState;

            if (!connectionHub.TryGet(RiftCapability.Combat, out IRiftCombatProvider combat) ||
                !combat.ValidateBossDefeat(riftId, bossSpawnId, playerId, authority.ServerNow()))
                return RiftRequestResult.Rejected;

            rift.BossDefeated = true;
            var boss = rift.Monsters.FirstOrDefault(x => x.SpawnId == bossSpawnId);
            if (boss != null) boss.IsAlive = false;
            events.Emit(events.Create("RiftBossDefeated", rift, playerId, payload: bossSpawnId));
            Complete(rift);
            return RiftRequestResult.Accepted;
        }

        private void Complete(RiftStateSnapshot rift)
        {
            long now = authority.ServerNow();
            if (!stateMachine.TryTransition(rift, RiftState.Cleared, now, out _))
                return;

            rift.CompletionRecorded = true;
            rift.FailureReason = RiftFailureReason.None;
            rift.WorldConsequence = RiftWorldConsequence.None;

            if (connectionHub.TryGet(RiftCapability.WorldKingdom, out IRiftKingdomProvider kingdom))
                kingdom.TryClearCityEngulfment(rift.CityId, rift.RiftId);

            if (connectionHub.TryGet(RiftCapability.Navigation, out IRiftNavigationProvider navigation))
                navigation.SetAreaRestricted(rift.CityId, false, "Rift cleared.");

            if (connectionHub.TryGet(RiftCapability.Npc, out IRiftNpcProvider npc))
                npc.SetAreaAccessRestricted(rift.CityId, false, "Rift cleared.");

            GrantRewards(rift);
            if (connectionHub.TryGet(RiftCapability.QuestGuild, out IRiftQuestProvider quest))
                quest.CompleteRiftQuest(rift.RiftId);

            events.Emit(events.Create("RiftCleared", rift));
            PublishDiscovery(rift, RiftDiscoveryType.Completion, "rift.cleared", null, null);
            Persist(rift);
            CleanupInstance(rift);
        }

        private void BeginMerging(RiftStateSnapshot rift)
        {
            long now = authority.ServerNow();
            RiftState next = rift.FailureReason == RiftFailureReason.AttemptsExhausted
                ? RiftState.Merging
                : RiftState.Merging;

            if (!stateMachine.TryTransition(rift, next, now, out _))
                return;

            Persist(rift);
        }

        private void Engulf(RiftStateSnapshot rift, long now)
        {
            rift.WorldConsequence = RiftWorldConsequence.Engulfed;

            if (connectionHub.TryGet(RiftCapability.WorldKingdom, out IRiftKingdomProvider kingdom))
                kingdom.TrySetCityEngulfed(rift.CityId, rift.RiftId, now);

            if (connectionHub.TryGet(RiftCapability.Navigation, out IRiftNavigationProvider navigation))
                navigation.SetAreaRestricted(rift.CityId, true, "Rift engulfment.");

            if (connectionHub.TryGet(RiftCapability.Npc, out IRiftNpcProvider npc))
            {
                npc.SetAreaAccessRestricted(rift.CityId, true, "Rift engulfment.");
                npc.NotifyCityEngulfed(rift.CityId, rift.RiftId);
            }

            LockParticipants(rift, "Rift engulfment.");
            rift.CurrentWave = 0;
            rift.SpawnedWaveNumbers.Clear();
            rift.NextWaveUnixMilliseconds = now;
            events.Emit(events.Create("RiftMerged", rift));
            events.Emit(events.Create("CityEngulfed", rift));
            Persist(rift);
        }

        private void LockParticipants(RiftStateSnapshot rift, string reason)
        {
            if (!connectionHub.TryGet(RiftCapability.Character, out IRiftCharacterProvider characters))
                return;

            foreach (var p in rift.Participants.Where(x => x.Status == RiftParticipantStatus.Inside))
            {
                p.WasInsideAtEngulfment = true;
                characters.TrySetPlayerLocked(p.PlayerId, true, reason);
            }
        }


        private void TickInvasion(RiftStateSnapshot rift, long now)
        {
            if (!connectionHub.TryGet(RiftCapability.MonsterGeneration, out IRiftMonsterProvider monsters))
                return;

            var type = GetType(rift.RiftTypeId);
            if (type == null)
                return;

            TickInvasionWaves(rift, type, monsters, now);

            if (!connectionHub.TryGet(RiftCapability.Destruction, out IRiftDestructionProvider destruction))
                return;

            var affected = rift.Participants
                .Where(x => x.WasInsideAtEngulfment)
                .Select(x => x.PlayerId)
                .ToArray();

            var request = new RiftDestructionRequest
            {
                RiftId = rift.RiftId,
                CityId = rift.CityId,
                KingdomId = rift.KingdomId,
                Timestamp = now,
                Cause = "Rift invasion",
                AffectedPlayerIds = affected
            };

            // The connected destruction system owns the actual city-health/destruction
            // condition. Querying it must not itself cause destruction.
            if (destruction.EvaluateWorldDestruction(request))
                CommitCityDestroyed(rift, affected, now);
        }

        private void TickInvasionWaves(
            RiftStateSnapshot rift,
            RiftTypeDefinition type,
            IRiftMonsterProvider monsters,
            long now)
        {
            if (now < rift.NextWaveUnixMilliseconds)
                return;

            var next = type.Waves
                .Where(x => !x.IsBossWave && !rift.SpawnedWaveNumbers.Contains(x.Wave))
                .OrderBy(x => x.Wave)
                .FirstOrDefault();

            if (next == null)
                return;

            int existing = rift.Monsters.Count(x => x.IsAlive);
            int allowed = Mathf.Max(0, configuration.MaxWorldInvasionMonsters - existing);
            int count = Mathf.Min(Mathf.Max(0, next.Population), allowed);

            var difficulty = configuration.Difficulties.FirstOrDefault(x => x.Id == rift.RiftDifficultyId);
            int level = Mathf.Max(1, next.MonsterBaseLevel + (difficulty?.MonsterLevelOffset ?? 0));

            for (int i = 0; i < count; i++)
            {
                if (next.MonsterPool.Count == 0 || next.SpawnPointIds.Count == 0)
                    break;

                string definition = next.MonsterPool[i % next.MonsterPool.Count];
                string spawnPoint = next.SpawnPointIds[i % next.SpawnPointIds.Count];

                if (monsters.SpawnRiftMonster(new RiftMonsterSpawnRequest
                {
                    RiftId = rift.RiftId,
                    SpawnPointId = spawnPoint,
                    MonsterDefinitionId = definition,
                    Level = level,
                    IsElite = true,
                    IsBoss = false,
                    Wave = next.Wave
                }, out string spawnId))
                {
                    rift.Monsters.Add(new RiftMonsterRecord
                    {
                        SpawnId = spawnId,
                        MonsterDefinitionId = definition,
                        Level = level,
                        IsElite = true,
                        IsBoss = false,
                        IsAlive = true,
                        SpawnWave = next.Wave
                    });
                }
            }

            rift.CurrentWave = next.Wave;
            rift.SpawnedWaveNumbers.Add(next.Wave);
            rift.NextWaveUnixMilliseconds = now + Mathf.Max(1, next.DelaySeconds) * 1000L;
            events.Emit(events.Create("RiftInvasionWaveSpawned", rift,
                payload: $"{{\"wave\":{next.Wave},\"count\":{count}}}"));
            Persist(rift);
        }

        private void CommitCityDestroyed(RiftStateSnapshot rift, IReadOnlyList<string> affectedPlayers, long now)
        {
            if (rift.DestructionRecorded)
                return;

            if (!stateMachine.TryTransition(rift, RiftState.CityDestroyed, now, out _))
                return;

            rift.WorldConsequence = RiftWorldConsequence.Destroyed;
            rift.DestructionRecorded = true;

            foreach (var p in rift.Participants.Where(x => x.WasInsideAtEngulfment))
            {
                p.WasInsideAtDestruction = true;
                p.Status = RiftParticipantStatus.Punished;

                if (connectionHub.TryGet(RiftCapability.Progression, out IRiftProgressionProvider progression))
                    progression.HandleRiftFailurePunishment(new RiftPunishmentContext
                    {
                        PlayerId = p.PlayerId,
                        RiftId = rift.RiftId,
                        CityId = rift.CityId,
                        FailureReason = RiftFailureReason.CityDestroyed.ToString(),
                        DestructionTimestamp = now
                    });
            }

            if (connectionHub.TryGet(RiftCapability.WorldKingdom, out IRiftKingdomProvider kingdom))
                kingdom.TrySetCityDestroyed(rift.CityId, rift.RiftId, now, "Rift invasion");

            if (connectionHub.TryGet(RiftCapability.Npc, out IRiftNpcProvider npc))
                npc.NotifyCityDestroyed(rift.CityId, rift.RiftId);

            events.Emit(events.Create("CityDestroyed", rift,
                payload: $"{{\"affectedPlayerCount\":{affectedPlayers.Count}}}"));
            PublishDiscovery(rift, RiftDiscoveryType.Failure, "rift.city_destroyed", null, null);
            Persist(rift);
            CleanupInstance(rift);
        }

        public void ConsumeAttemptFailure(string riftId)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return;

            if (rift.AttemptsConsumed < rift.AttemptLimit)
                return;

            long now = authority.ServerNow();
            if (rift.State != RiftState.Active && rift.State != RiftState.InProgress && rift.State != RiftState.BossActive)
                return;

            rift.FailureReason = RiftFailureReason.AttemptsExhausted;
            if (stateMachine.TryTransition(rift, RiftState.AttemptsExhausted, now, out _))
            {
                events.Emit(events.Create("RiftAttemptsExhausted", rift, failure: rift.FailureReason.ToString()));
                Persist(rift);
                BeginMerging(rift);
            }
        }

        public void RegisterBossSpawn(string riftId, string bossSpawnId)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return;

            if (rift.State != RiftState.InProgress && rift.State != RiftState.Active)
                return;

            rift.BossSpawned = true;
            rift.CurrentWave++;
            stateMachine.TryTransition(rift, RiftState.BossActive, authority.ServerNow(), out _);
            events.Emit(events.Create("RiftBossSpawned", rift));
            Persist(rift);
        }

        public bool TryRestorePlayer(string playerId, string riftId)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return false;

            var p = rift.Participants.FirstOrDefault(x => x.PlayerId == playerId);
            if (p == null)
                return false;

            p.LastSeenUnixMilliseconds = authority.ServerNow();

            if (rift.State == RiftState.Engulfed)
            {
                if (connectionHub.TryGet(RiftCapability.Character, out IRiftCharacterProvider characters))
                    characters.TrySetPlayerLocked(playerId, true, "Restored inside engulfed Rift.");
            }

            if (rift.CurrentInstanceId != null &&
                connectionHub.TryGet(RiftCapability.DynamicInstance, out IRiftInstanceProvider instances))
                instances.TryMovePlayerToInstance(playerId, rift.CurrentInstanceId);

            events.Emit(events.Create("RiftReconnect", rift, playerId));
            Persist(rift);
            return true;
        }


        public bool ValidateBoundaryEntry(string playerId, string riftId, RiftVector3 serverPosition)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return false;

            if (!connectionHub.TryGet(RiftCapability.ServerAuthority, out IRiftServerAuthorityProvider server) ||
                !server.IsAuthoritativeServer)
                return false;

            if (rift.State != RiftState.Engulfed)
                return true;

            if (!connectionHub.TryGet(RiftCapability.WorldKingdom, out IRiftKingdomProvider kingdom) ||
                !kingdom.TryGetCity(rift.CityId, out var city))
                return false;

            float dx = serverPosition.X - city.Center.X;
            float dz = serverPosition.Z - city.Center.Z;
            float radius = city.Radius + configuration.PositionValidationToleranceMeters;
            bool inside = dx * dx + dz * dz <= radius * radius;

            if (!inside)
                authority.Report(playerId, riftId, "ImpossibleRiftBoundaryState",
                    "Player position is outside an engulfed Rift area while server state says trapped.", 5);

            return inside;
        }

        public bool ValidateExitBoundary(string playerId, string riftId, RiftVector3 serverPosition)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return false;

            if (rift.State != RiftState.Engulfed)
                return true;

            if (!connectionHub.TryGet(RiftCapability.Character, out IRiftCharacterProvider character))
                return false;

            if (!character.TryGetPlayerPosition(playerId, out var current))
                return false;

            if (Mathf.Abs(current.X - serverPosition.X) > configuration.PositionValidationToleranceMeters ||
                Mathf.Abs(current.Y - serverPosition.Y) > configuration.PositionValidationToleranceMeters ||
                Mathf.Abs(current.Z - serverPosition.Z) > configuration.PositionValidationToleranceMeters)
            {
                authority.Report(playerId, riftId, "ClientPositionMismatch",
                    "Boundary request position differs from server position.", 5);
                return false;
            }

            return false;
        }

        public bool ValidatePortalUse(string playerId, string riftId, string portalId, string destinationId)
        {
            if (!active.TryGetValue(riftId, out var rift))
                return true;

            if (rift.State != RiftState.Engulfed)
                return true;

            if (connectionHub.TryGet(RiftCapability.Portal, out IRiftPortalProvider portals))
            {
                if (!portals.CanUsePortal(playerId, portalId, destinationId))
                {
                    authority.Report(playerId, riftId, "BlockedRiftPortalBypass",
                        "Portal provider rejected an attempt to cross the engulfed boundary.", 4);
                    return false;
                }
            }

            // An engulfed Rift cannot be exited through a portal by default.
            authority.Report(playerId, riftId, "ImpossibleRiftPortalExit",
                "Player attempted portal exit from an engulfed Rift.", 5);
            return false;
        }

        public bool ForceSetDuration(string administratorId, string riftId, int seconds, string reason)
        {
            if (!AuthorizeAdmin(administratorId, riftId, "ForceSetDuration", reason) ||
                seconds < 1 || !active.TryGetValue(riftId, out var rift))
                return false;

            rift.ExpirationUnixMilliseconds = authority.ServerNow() + seconds * 1000L;
            Persist(rift);
            return true;
        }

        public bool ForceSetAttempts(string administratorId, string riftId, int attempts, string reason)
        {
            if (!AuthorizeAdmin(administratorId, riftId, "ForceSetAttempts", reason) ||
                attempts < 1 || !active.TryGetValue(riftId, out var rift))
                return false;

            rift.AttemptLimit = attempts;
            if (rift.AttemptsConsumed >= attempts)
                ConsumeAttemptFailure(riftId);
            Persist(rift);
            return true;
        }

        public bool ForceEscalate(string administratorId, string riftId, string reason)
        {
            if (!AuthorizeAdmin(administratorId, riftId, "ForceEscalate", reason) ||
                !active.TryGetValue(riftId, out var rift))
                return false;

            if (rift.State == RiftState.Active || rift.State == RiftState.InProgress || rift.State == RiftState.BossActive)
            {
                rift.FailureReason = RiftFailureReason.Administrative;
                if (stateMachine.TryTransition(rift, RiftState.TimeExpired, authority.ServerNow(), out _))
                {
                    Persist(rift);
                    BeginMerging(rift);
                    return true;
                }
            }
            return false;
        }

        public bool ForceEngulf(string administratorId, string riftId, string reason)
        {
            if (!AuthorizeAdmin(administratorId, riftId, "ForceEngulf", reason) ||
                !active.TryGetValue(riftId, out var rift))
                return false;

            if (rift.State == RiftState.Merging)
            {
                if (stateMachine.TryTransition(rift, RiftState.Engulfed, authority.ServerNow(), out _))
                {
                    Engulf(rift, authority.ServerNow());
                    return true;
                }
            }

            if (rift.State == RiftState.Active || rift.State == RiftState.InProgress || rift.State == RiftState.BossActive)
            {
                rift.FailureReason = RiftFailureReason.Administrative;
                if (stateMachine.TryTransition(rift, RiftState.TimeExpired, authority.ServerNow(), out _) &&
                    stateMachine.TryTransition(rift, RiftState.Merging, authority.ServerNow(), out _) &&
                    stateMachine.TryTransition(rift, RiftState.Engulfed, authority.ServerNow(), out _))
                {
                    Engulf(rift, authority.ServerNow());
                    return true;
                }
            }
            return false;
        }

        public bool ForceClear(string administratorId, string riftId, string reason)
        {
            if (!AuthorizeAdmin(administratorId, riftId, "ForceClear", reason) ||
                !active.TryGetValue(riftId, out var rift))
                return false;

            if (rift.State != RiftState.Active && rift.State != RiftState.InProgress &&
                rift.State != RiftState.BossActive && rift.State != RiftState.Engulfed)
                return false;

            Complete(rift);
            return true;
        }

        private bool AuthorizeAdmin(string administratorId, string riftId, string action, string reason)
        {
            if (string.IsNullOrWhiteSpace(administratorId))
                return false;

            if (connectionHub.TryGet(RiftCapability.LiveOps, out IRiftLiveOpsProvider ops))
            {
                return ops.AuthorizeAdminCommand(new RiftAdminCommand
                {
                    CommandId = Guid.NewGuid().ToString("N"),
                    AdministratorId = administratorId,
                    RiftId = riftId,
                    Reason = reason,
                    IssuedUnixMilliseconds = authority.ServerNow()
                });
            }

            return false;
        }

        public RiftStateSnapshot GetSnapshot(string riftId)
        {
            active.TryGetValue(riftId, out var snapshot);
            return snapshot;
        }

        public void PublishServerSnapshot(string riftId)
        {
            if (!active.TryGetValue(riftId, out var snapshot))
                return;

            if (connectionHub.TryGet(RiftCapability.Ui, out IRiftUiProvider ui))
                ui.PushSnapshot(snapshot);

            if (connectionHub.TryGet(RiftCapability.AdventureWorld, out IRiftAdventureWorldProvider world))
                world.NotifyRiftWorldStateChanged(snapshot);
        }

        private void GrantRewards(RiftStateSnapshot rift)
        {
            if (rift.RewardsRecorded || !connectionHub.TryGet(RiftCapability.Rewards, out IRiftRewardProvider rewards))
                return;

            var type = GetType(rift.RiftTypeId);
            if (type == null)
                return;

            foreach (var p in rift.Participants.Where(x => x.Status == RiftParticipantStatus.Inside ||
                                                             x.Status == RiftParticipantStatus.Completed))
            {
                var records = type.Rewards
                    .Where(x => UnityEngine.Random.Range(0f, 100f) <= x.ChancePercent)
                    .Select(x => new RiftRewardRecord
                    {
                        RewardId = x.RewardId,
                        RewardType = x.RewardType,
                        ItemId = x.ItemId,
                        Amount = x.Amount
                    }).ToList();

                if (rewards.TryGrantRewards(new RiftRewardGrantRequest
                {
                    RiftId = rift.RiftId,
                    PlayerId = p.PlayerId,
                    Rewards = records,
                    IdempotencyKey = $"rift:{rift.RiftId}:reward:{p.PlayerId}"
                }))
                {
                    foreach (var record in records)
                        record.Granted = true;
                }
            }

            rift.RewardsRecorded = true;
            events.Emit(events.Create("RiftRewardsGranted", rift));
        }

        private void PublishQuest(RiftStateSnapshot snapshot)
        {
            if (connectionHub.TryGet(RiftCapability.QuestGuild, out IRiftQuestProvider quest))
                quest.CreateOrUpdateRiftQuest(snapshot);
        }

        private void PublishDiscovery(RiftStateSnapshot snapshot, RiftDiscoveryType type, string key, string playerId, string evidence)
        {
            if (connectionHub.TryGet(RiftCapability.LoreDiscovery, out IRiftLoreProvider lore))
                lore.RecordDiscovery(new RiftDiscoveryEvent
                {
                    RiftId = snapshot.RiftId,
                    DiscoveryType = type,
                    DiscoveryKey = key,
                    PlayerId = playerId,
                    EvidenceId = evidence,
                    TimestampUnixMilliseconds = authority.ServerNow()
                });
        }

        private void CleanupInstance(RiftStateSnapshot rift)
        {
            if (rift.CurrentInstanceId == null)
                return;

            if (connectionHub.TryGet(RiftCapability.DynamicInstance, out IRiftInstanceProvider instances))
            {
                instances.TryDestroyInstance(rift.CurrentInstanceId,
                    rift.State == RiftState.CityDestroyed ? "City destroyed." : "Rift resolved.");
            }

            events.Emit(events.Create("RiftInstanceDestroyed", rift));
        }

        private void Persist(RiftStateSnapshot snapshot)
        {
            if (!persistence.Save(snapshot, snapshot.VersionToken, out string newVersion))
            {
                events.Emit(events.Create("RiftPersistenceFailure", snapshot,
                    failure: RiftFailureReason.InfrastructureFailure.ToString()));
                return;
            }

            snapshot.VersionToken = newVersion;
            PublishServerSnapshot(snapshot.RiftId);
        }

        private RiftTypeDefinition GetType(string id) =>
            configuration.RiftTypes.FirstOrDefault(x => string.Equals(x.Id, id, StringComparison.Ordinal));

        private RiftTypeDefinition SelectType(string forcedId)
        {
            if (!string.IsNullOrWhiteSpace(forcedId))
                return GetType(forcedId);

            var candidates = configuration.RiftTypes.Where(x => x.Weight > 0).ToList();
            if (candidates.Count == 0)
                return null;

            int total = candidates.Sum(x => x.Weight);
            int roll = UnityEngine.Random.Range(0, total);
            foreach (var candidate in candidates)
            {
                roll -= candidate.Weight;
                if (roll < 0)
                    return candidate;
            }
            return candidates[^1];
        }

        private RiftDifficultyDefinition SelectDifficulty()
        {
            var candidates = configuration.Difficulties.Where(x => x.Weight > 0).ToList();
            if (candidates.Count == 0)
                return null;

            int total = candidates.Sum(x => x.Weight);
            int roll = UnityEngine.Random.Range(0, total);
            foreach (var candidate in candidates)
            {
                roll -= candidate.Weight;
                if (roll < 0)
                    return candidate;
            }
            return candidates[^1];
        }

        private string SelectBoss(RiftTypeDefinition type)
        {
            if (type.BossDefinitionIds.Count == 0)
                return string.Empty;
            return type.BossDefinitionIds[UnityEngine.Random.Range(0, type.BossDefinitionIds.Count)];
        }
    }
}
