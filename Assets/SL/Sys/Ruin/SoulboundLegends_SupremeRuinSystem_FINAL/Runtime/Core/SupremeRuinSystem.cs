using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulboundLegends.SupremeRuin
{
    public sealed class SupremeRuinSystem : MonoBehaviour
    {
        [SerializeField] private SupremeRuinConfiguration configuration;
        [SerializeField] private SupremeRuinConnectionHub connections;

        private readonly Dictionary<string, SupremeRuinStateSnapshot> active = new();
        private readonly SupremeRuinStateMachine stateMachine = new();
        private readonly Dictionary<string, long> requestWindowStart = new();
        private readonly Dictionary<string, int> requestCounts = new();

        public SupremeRuinConnectionHub Connections => connections;
        public SupremeRuinConfiguration Configuration => configuration;

        private void Awake()
        {
            if (connections != null) connections.Rebuild();
            RestorePersistedRuinStates();
        }

        private void Update()
        {
            if (!TryServer(out var server)) return;
            long now = server.ServerNowUnixMilliseconds;

            foreach (var state in active.Values.ToArray())
                Tick(state, now);
        }

        public RuinAccessDecision RequestEntry(RuinAccessRequest request)
        {
            var denied = new RuinAccessDecision { Allowed=false };

            if (request == null || configuration == null || connections == null)
                return Denied(denied, "INVALID_REQUEST");

            if (!TryServer(out var server) || !server.IsAuthoritativeServer)
                return Denied(denied, "SERVER_AUTHORITY_REQUIRED");

            if (!server.ValidateRequestNonce(request.PlayerId, request.Nonce, request.RequestId))
                return Denied(denied, "INVALID_NONCE");

            if (!RateLimit(request.PlayerId, server.ServerNowUnixMilliseconds))
                return Denied(denied, "RATE_LIMITED");

            if (!connections.TryGet(SupremeRuinCapability.ServerAuthorization, out ISupremeRuinAuthorization auth) ||
                !auth.Authorize(request.PlayerId, "SupremeRuinEnter", request.RuinId))
                return Denied(denied, "UNAUTHORIZED");

            var ruin = configuration.FindRuin(request.RuinId);
            if (ruin == null) return Denied(denied, "RUIN_NOT_FOUND");
            if (!ValidateGodDefinition(ruin)) return Denied(denied, "INVALID_RUIN_CONFIGURATION");

            if (active.TryGetValue(ruin.Id, out var existing) &&
                existing.RuinState != RuinState.Available &&
                existing.RuinState != RuinState.Completed &&
                existing.RuinState != RuinState.Failed)
                return Denied(denied, "RUIN_CURRENTLY_CHALLENGED");

            if (ruin.MaxPlayers != 1 || ruin.AllowParty)
                return Denied(denied, "ONE_PLAYER_TRIAL_REQUIRED");

            if (!TryGetWorld(out var world) || !world.IsRuinAreaAvailable(ruin.KingdomId, ruin.EntranceDefinitionId))
                return Denied(denied, "WORLD_AREA_UNAVAILABLE");

            if (!TryGetCharacter(out var character) || !character.IsCharacterEligibleForTrial(request.PlayerId))
                return Denied(denied, "CHARACTER_STATE_INVALID");

            if (!TryGetInheritance(out var inheritance))
                return Denied(denied, "INHERITANCE_PROVIDER_REQUIRED");

            if (inheritance != null && inheritance.HasInheritance(request.PlayerId))
                return Denied(denied, "INHERITANCE_ALREADY_OWNED");

            if (!TryGetRequirements(out _) ||
                !CheckRequirements(request.PlayerId, ruin, out string requirementFailure))
                return Denied(denied, requirementFailure ?? "REQUIREMENTS_PROVIDER_REQUIRED");

            if (world.IsRuinOccupied(ruin.Id))
                return Denied(denied, "RUIN_CURRENTLY_CHALLENGED");

            string trialId = Guid.NewGuid().ToString("N");
            if (!world.TryMarkRuinOccupied(ruin.Id, trialId))
                return Denied(denied, "RUIN_OCCUPIED_RACE");

            if (!TryGetInstance(out var instances) ||
                !instances.CreateTrialInstance(request.RequestId, ruin.Id, request.PlayerId,
                    ruin.TrialEnvironmentDefinitionId, out string instanceId))
            {
                world.TryReleaseRuin(ruin.Id, trialId);
                return Denied(denied, "INSTANCE_CREATION_FAILED");
            }

            if (!instances.AddPlayer(instanceId, request.PlayerId))
            {
                instances.DestroyInstance(instanceId, "PLAYER_ADD_FAILED");
                world.TryReleaseRuin(ruin.Id, trialId);
                return Denied(denied, "INSTANCE_PLAYER_ADD_FAILED");
            }

            if (!TryGetRequirements(out var requirements) ||
                !ConsumeRequirementsAtomically(request.PlayerId, ruin, requirements))
            {
                instances.RemovePlayer(instanceId, request.PlayerId);
                instances.DestroyInstance(instanceId, "REQUIREMENT_TRANSACTION_FAILED");
                world.TryReleaseRuin(ruin.Id, trialId);
                return Denied(denied, "REQUIREMENT_TRANSACTION_FAILED");
            }

            long now = server.ServerNowUnixMilliseconds;
            var state = new SupremeRuinStateSnapshot
            {
                TrialId = trialId,
                RuinId = ruin.Id,
                GodId = ruin.GodId,
                KingdomId = ruin.KingdomId,
                EntranceId = ruin.EntranceDefinitionId,
                InstanceId = instanceId,
                RuinState = RuinState.Occupied,
                TrialState = TrialState.Starting,
                ActiveChallengerPlayerId = request.PlayerId,
                StartedUnixMilliseconds = now,
                LastUpdatedUnixMilliseconds = now,
                TrialDeadlineUnixMilliseconds = now + ruin.TrialDurationSeconds * 1000L,
                RequiredCreatureCount = 5,
                InheritanceAwardState = InheritanceAwardState.None,
                Version = 1
            };
            state.Participants.Add(new RuinParticipant
            {
                PlayerId=request.PlayerId, CharacterId=request.CharacterId,
                JoinedUnixMilliseconds=now, IsInside=true, CanAssist=false
            });

            active[ruin.Id] = state;

            if (!stateMachine.TryTransition(state, RuinState.TrialStarting) ||
                !BuildInfrastructure(ruin, instanceId) ||
                !StartQuest(state, request.PlayerId) ||
                !SpawnCreatures(state, ruin))
            {
                FailInternal(state, TrialFailureReason.ServerTermination, "TRIAL_START_FAILED");
                return Denied(denied, "TRIAL_START_FAILED");
            }

            stateMachine.TryTransition(state, RuinState.TrialActive);
            state.TrialState = TrialState.Active;
            Persist(state);
            Track("SupremeRuinTrialStarted", state, request.PlayerId, null);

            return new RuinAccessDecision
            {
                Allowed=true, ReasonCode="OK", ReasonMessage="Divine trial started.",
                TrialId=state.TrialId, InstanceId=state.InstanceId
            };
        }


        public bool CanEnterOrAssist(string playerId, string ruinId)
        {
            if (!TryServer(out var server) || !server.IsAuthoritativeServer)
                return false;

            var state = active.Values.FirstOrDefault(x => x.RuinId == ruinId &&
                (x.RuinState == RuinState.Occupied || x.RuinState == RuinState.TrialStarting ||
                 x.RuinState == RuinState.TrialActive));

            if (state == null)
                return true;

            // A Supreme Ruin has exactly one active challenger. No party member,
            // second client, reconnecting duplicate, portal, or other participant
            // may become an additional challenger.
            bool isOwner = state.ActiveChallengerPlayerId == playerId;
            if (!isOwner)
            {
                server.ReportSecurity(playerId, ruinId, "SUPREME_RUIN_ASSIST_BLOCKED",
                    "A second player attempted to enter or assist an active Divine Trial.", 4);
                if (connections.TryGet(SupremeRuinCapability.AntiCheat, out ISupremeRuinAntiCheat anti))
                    anti.Report(playerId, ruinId, "SUPREME_RUIN_ASSIST_BLOCKED",
                        "Second-player entry/assist attempt.", 4);
                return false;
            }
            return true;
        }

        public bool SubmitCreatureDeath(CreatureDeathValidationRequest request)
        {
            if (request == null || !TryServer(out var server) || !server.IsAuthoritativeServer)
                return false;

            var state = active.Values.FirstOrDefault(x => x.TrialId == request.TrialId);
            if (state == null || state.TrialState != TrialState.Active)
                return false;

            var creature = state.Creatures.FirstOrDefault(x => x.SpawnId == request.SpawnId);
            if (creature == null || creature.Defeated || !creature.Spawned)
                return false;

            if (!TryGetCharacter(out var character) || !character.IsPlayerInInstance(request.KillerPlayerId, state.InstanceId))
                return Reject(server, request.KillerPlayerId, state.RuinId, "KILLER_NOT_IN_INSTANCE");

            if (!TryGetCombat(out var combat) ||
                !combat.ValidateCreatureDeath(request, out var proof) ||
                proof == null || !proof.Legitimate ||
                proof.SpawnId != request.SpawnId)
                return Reject(server, request.KillerPlayerId, state.RuinId, "INVALID_CREATURE_DEATH");

            creature.Defeated = true;
            creature.DefeatedByPlayerId = request.KillerPlayerId;
            creature.DefeatedAtUnixMilliseconds = server.ServerNowUnixMilliseconds;
            state.DefeatedCreatureCount++;
            state.TrialState = TrialState.CreatureProgress;

            if (connections.TryGet(SupremeRuinCapability.Quest, out ISupremeRuinQuest quest))
                if (!quest.UpdateCreatureDefeat(state.TrialId, creature.RequiredIndex))
                    return Reject(server, request.KillerPlayerId, state.RuinId, "QUEST_UPDATE_FAILED");

            Track("SupremeRuinCreatureDefeated", state, request.KillerPlayerId, creature.CreatureId);
            Persist(state);

            if (state.DefeatedCreatureCount == 5)
                return CompleteTrial(state);

            return true;
        }

        public bool FailTrial(string ruinId, TrialFailureReason reason, string detail)
        {
            if (!active.TryGetValue(ruinId, out var state)) return false;
            return FailInternal(state, reason, detail);
        }

        public SupremeRuinStateSnapshot GetState(string ruinId)
        {
            if (active.TryGetValue(ruinId, out var state))
                return CloneState(state);
            return null;
        }

        private bool CompleteTrial(SupremeRuinStateSnapshot state)
        {
            if (state.DefeatedCreatureCount != 5 || state.Creatures.Count != 5 ||
                state.Creatures.Any(x => !x.Defeated))
                return false;

            if (!TryServer(out var server)) return false;
            var ruin = configuration.FindRuin(state.RuinId);
            var god = configuration.FindGod(state.GodId);
            if (ruin == null || god == null) return FailInternal(state, TrialFailureReason.InvalidState, "CONFIGURATION_INVALID");

            if (!TryGetInheritance(out var inheritance) || inheritance.HasInheritance(state.ActiveChallengerPlayerId))
                return FailInternal(state, TrialFailureReason.InvalidState, "INHERITANCE_CONFLICT");

            // The Quest provider is part of the authoritative completion gate.
            // Never award an inheritance and then roll the trial back because a
            // downstream optional reward/UI operation failed.
            if (connections.TryGet(SupremeRuinCapability.Quest, out ISupremeRuinQuest quest) &&
                !quest.CompleteTrialQuest(state.TrialId))
                return FailInternal(state, TrialFailureReason.InvalidState, "QUEST_COMPLETION_FAILED");

            var record = new InheritanceRecord
            {
                PlayerId=state.ActiveChallengerPlayerId,
                CharacterId=state.Participants.First(x=>x.PlayerId==state.ActiveChallengerPlayerId).CharacterId,
                GodId=god.Id,
                InheritanceId=god.InheritanceId,
                TrialId=state.TrialId,
                RuinId=state.RuinId,
                AcquisitionUnixMilliseconds=server.ServerNowUnixMilliseconds,
                Version=1
            };

            if (!inheritance.AwardInheritanceAtomically(record))
                return FailInternal(state, TrialFailureReason.InvalidState, "INHERITANCE_AWARD_FAILED");

            state.InheritanceAwardState = InheritanceAwardState.Awarded;
            state.AwardedInheritanceId = record.InheritanceId;

            // Additional rewards are independent from the core inheritance grant.
            // Their provider must make its own grant idempotent and retryable.
            if (connections.TryGet(SupremeRuinCapability.Rewards, out ISupremeRuinRewards rewards))
                rewards.GrantConfiguredRewards(new RuinRewardRequest
                {
                    TrialId=state.TrialId, PlayerId=state.ActiveChallengerPlayerId,
                    InheritanceId=record.InheritanceId, ServerTimestamp=server.ServerNowUnixMilliseconds
                });

            state.TrialState = TrialState.Completed;
            state.RuinState = RuinState.Completed;
            Persist(state);
            Track("SupremeRuinCompleted", state, state.ActiveChallengerPlayerId, record.InheritanceId);
            Cleanup(state, true);
            return true;
        }

        private bool FailInternal(SupremeRuinStateSnapshot state, TrialFailureReason reason, string detail)
        {
            state.FailureReason = reason;
            state.FailureDetail = detail;
            state.TrialState = TrialState.Failed;
            state.RuinState = RuinState.Failed;

            if (connections.TryGet(SupremeRuinCapability.Quest, out ISupremeRuinQuest quest))
                quest.FailTrialQuest(state.TrialId, reason);

            Persist(state);
            Track("SupremeRuinFailed", state, state.ActiveChallengerPlayerId, detail);
            Cleanup(state, false);
            return false;
        }

        private void Tick(SupremeRuinStateSnapshot state, long now)
        {
            if (state.TrialState != TrialState.Active && state.TrialState != TrialState.CreatureProgress)
                return;

            if (now >= state.TrialDeadlineUnixMilliseconds)
            {
                FailInternal(state, TrialFailureReason.InvalidState, "TRIAL_TIME_EXPIRED");
                return;
            }

            if (TryGetCharacter(out var character) && !character.IsPlayerAlive(state.ActiveChallengerPlayerId))
            {
                var ruin = configuration.FindRuin(state.RuinId);
                if (ruin != null && ruin.Failure.FailOnPlayerDeath)
                    FailInternal(state, TrialFailureReason.PlayerDeath, "PLAYER_DIED");
            }
        }

        private bool SpawnCreatures(SupremeRuinStateSnapshot state, SupremeRuinDefinition ruin)
        {
            if (!connections.TryGet(SupremeRuinCapability.MonsterGeneration, out ISupremeRuinMonster monsters))
                return false;

            if (ruin.RequiredCreatures.Count != 5) return false;
            var god = configuration.FindGod(ruin.GodId);
            if (god == null) return false;

            var ordered = ruin.RequiredCreatures.OrderBy(x=>x.RequiredIndex).ToList();
            for (int i=0;i<ordered.Count;i++)
            {
                var slot = ordered[i];
                var creature = configuration.FindCreature(slot.CreatureDefinitionId);
                if (creature == null || creature.God.ToString() != god.God.ToString())
                    return false;
                if (!ValidateElement(god, creature))
                    return false;

                var profile = ScaleStats(creature.Stats);
                var request = new DivineCreatureSpawnRequest
                {
                    TrialId=state.TrialId, RuinId=state.RuinId,
                    DefinitionId=creature.Id, GodId=god.Id,
                    Element=creature.Element.ToString(), RequiredIndex=slot.RequiredIndex,
                    Level=creature.BaseLevel, Stats=profile,
                    ArenaRole=creature.Role.ToString(),
                    SpawnPointId=slot.SpawnPointId, InstanceId=state.InstanceId
                };

                // Sequential trials spawn all definitions through the monster provider,
                // while the provider/configured trial mode determines activation timing.
                if (!monsters.SpawnDivineCreature(request, out string spawnId))
                    return false;

                state.Creatures.Add(new CreatureProgressRecord
                {
                    CreatureId=creature.Id, SpawnId=spawnId,
                    DefinitionId=creature.Id, RequiredIndex=slot.RequiredIndex,
                    Spawned=true, Defeated=false
                });
            }
            return true;
        }

        private DivineCreatureStatProfile ScaleStats(DivineCreatureStatProfile source)
        {
            return new DivineCreatureStatProfile
            {
                Hp=source.Hp, Armor=source.Armor, Attack=source.Attack,
                AttackSpeed=source.AttackSpeed, MovementSpeed=source.MovementSpeed,
                Resistance=source.Resistance, ElementalPower=source.ElementalPower,
                CriticalPower=source.CriticalPower, SkillDamageMultiplier=source.SkillDamageMultiplier,
                HealingPerSecond=source.HealingPerSecond, Regeneration=source.Regeneration,
                StatusResistance=source.StatusResistance,
                CrowdControlResistance=source.CrowdControlResistance,
                AiReactionSeconds=source.AiReactionSeconds,
                PhaseMultiplier=source.PhaseMultiplier
            };
        }

        private bool ValidateElement(GodDefinition god, DivineCreatureDefinition creature)
        {
            if (god.God == SupremeGodId.Omnigod)
                return god.OmnigodAllowedElements.Contains(creature.Element) || creature.Element == DivineElement.MultiElement;
            if (god.God == SupremeGodId.Shadow)
                return creature.Element == DivineElement.Shadow;
            return creature.Element.ToString().Equals(god.Element.ToString(), StringComparison.Ordinal);
        }

        private bool ValidateGodDefinition(SupremeRuinDefinition ruin)
        {
            var god = configuration.FindGod(ruin.GodId);
            return god != null && (god.God != SupremeGodId.Shadow || configuration.HiddenLore.HideShadowBetrayalEarly);
        }

        private bool CheckRequirements(string playerId, SupremeRuinDefinition ruin, out string failure)
        {
            failure = "REQUIREMENT_FAILED";
            if (!TryGetRequirements(out var req)) return false;

            foreach (var item in ruin.Requirements)
            {
                bool ok = item.Type switch
                {
                    RequirementType.Item => req.CheckItem(playerId, item.Id, item.Quantity),
                    RequirementType.Level => req.CheckLevel(playerId, item.MinimumLevel),
                    RequirementType.Quest => req.CheckQuest(playerId, item.Id),
                    RequirementType.Story => req.CheckStory(playerId, item.ConditionValue),
                    RequirementType.World => req.CheckWorld(item.ConditionValue),
                    RequirementType.GodCondition => req.CheckGodCondition(playerId, item.ConditionValue),
                    RequirementType.Permission => req.CheckPermission(playerId, item.Id),
                    RequirementType.Discovery => req.CheckDiscovery(playerId, item.Id),
                    RequirementType.Custom => req.CheckWorld(item.ConditionValue),
                    _ => false
                };
                if (!ok) { failure = $"REQUIREMENT_FAILED:{item.Type}:{item.Id}"; return false; }
            }
            return true;
        }

        private bool ConsumeRequirementsAtomically(string playerId, SupremeRuinDefinition ruin, ISupremeRuinRequirements req)
        {
            var items = ruin.Requirements.Where(x=>x.Type==RequirementType.Item && x.ConsumeOnSuccessfulStart)
                .Select(x=>new ItemRequirementData{ItemId=x.Id,Quantity=x.Quantity}).ToList();
            return items.Count == 0 || req.ConsumeItemsAtomically(playerId, items);
        }

        private bool BuildInfrastructure(SupremeRuinDefinition ruin, string instanceId)
        {
            return !connections.TryGet(SupremeRuinCapability.Infrastructure, out ISupremeRuinInfrastructure infra) ||
                   infra.BuildRuin(ruin.Id, ruin.InfrastructureDefinitionId, instanceId);
        }

        private bool StartQuest(SupremeRuinStateSnapshot state, string playerId)
        {
            return !connections.TryGet(SupremeRuinCapability.Quest, out ISupremeRuinQuest quest) ||
                   quest.CreateDivineTrialQuest(playerId, state.TrialId, state.RuinId, state.GodId, 5);
        }

        private void Cleanup(SupremeRuinStateSnapshot state, bool success)
        {
            if (connections.TryGet(SupremeRuinCapability.DynamicInstance, out ISupremeRuinDynamicInstance instances))
                instances.DestroyInstance(state.InstanceId, success ? "TRIAL_COMPLETED" : "TRIAL_FAILED");

            if (connections.TryGet(SupremeRuinCapability.Infrastructure, out ISupremeRuinInfrastructure infra))
                infra.DestroyRuin(state.RuinId, state.InstanceId);

            if (connections.TryGet(SupremeRuinCapability.WorldState, out ISupremeRuinWorld world))
                world.TryReleaseRuin(state.RuinId, state.TrialId);

            Track("SupremeRuinInstanceCleaned", state, state.ActiveChallengerPlayerId, success ? "SUCCESS" : "FAILURE");
        }

        private void Persist(SupremeRuinStateSnapshot state)
        {
            if (!TryServer(out var server)) return;
            state.LastUpdatedUnixMilliseconds = server.ServerNowUnixMilliseconds;
            state.Version++;
            if (connections.TryGet(SupremeRuinCapability.Persistence, out ISupremeRuinPersistence persistence))
                if (!persistence.Save(state))
                    throw new InvalidOperationException($"Failed to persist Supreme Ruin state {state.RuinId}.");
            if (connections.TryGet(SupremeRuinCapability.Ui, out ISupremeRuinUi ui))
                ui.PublishState(CloneState(state));
        }

        private void RestorePersistedRuinStates()
        {
            if (!connections.TryGet(SupremeRuinCapability.Persistence, out ISupremeRuinPersistence persistence))
                return;

            foreach (var ruin in configuration.Ruins)
            {
                if (persistence.Load(ruin.Id, out var state) &&
                    state != null &&
                    (state.RuinState == RuinState.Occupied || state.RuinState == RuinState.TrialStarting ||
                     state.RuinState == RuinState.TrialActive))
                    active[ruin.Id] = state;
            }
        }

        private bool RateLimit(string playerId, long now)
        {
            if (!requestWindowStart.TryGetValue(playerId, out long start) || now-start >= 60000)
            {
                requestWindowStart[playerId]=now; requestCounts[playerId]=0;
            }
            requestCounts[playerId]++;
            return requestCounts[playerId] <= configuration.GlobalRules.RequestRateLimitPerMinute;
        }

        private bool Reject(ISupremeRuinServerAuthority server, string player, string ruin, string code)
        {
            server.ReportSecurity(player, ruin, code, "Supreme Ruin request rejected.", 5);
            if (connections.TryGet(SupremeRuinCapability.AntiCheat, out ISupremeRuinAntiCheat anti))
                anti.Report(player, ruin, code, "Authoritative Supreme Ruin validation rejected an action.", 5);
            return false;
        }

        private void Track(string evt, SupremeRuinStateSnapshot state, string player, string payload)
        {
            if (connections.TryGet(SupremeRuinCapability.Analytics, out ISupremeRuinAnalytics analytics))
                analytics.Track(evt, state.RuinId, state.TrialId, player, payload);
        }

        private static RuinAccessDecision Denied(RuinAccessDecision d, string code)
        {
            d.ReasonCode=code; d.ReasonMessage=code; return d;
        }

        private bool TryServer(out ISupremeRuinServerAuthority p) =>
            connections.TryGet(SupremeRuinCapability.ServerAuthority, out p);

        private bool TryGetRequirements(out ISupremeRuinRequirements p) =>
            connections.TryGet(SupremeRuinCapability.Requirements, out p);

        private bool TryGetWorld(out ISupremeRuinWorld p) =>
            connections.TryGet(SupremeRuinCapability.WorldState, out p);

        private bool TryGetInstance(out ISupremeRuinDynamicInstance p) =>
            connections.TryGet(SupremeRuinCapability.DynamicInstance, out p);

        private bool TryGetCharacter(out ISupremeRuinCharacter p) =>
            connections.TryGet(SupremeRuinCapability.Character, out p);

        private bool TryGetCombat(out ISupremeRuinCombat p) =>
            connections.TryGet(SupremeRuinCapability.Combat, out p);

        private bool TryGetInheritance(out ISupremeRuinInheritance p) =>
            connections.TryGet(SupremeRuinCapability.Inheritance, out p);

        private SupremeRuinStateSnapshot CloneState(SupremeRuinStateSnapshot x)
        {
            return JsonUtility.FromJson<SupremeRuinStateSnapshot>(JsonUtility.ToJson(x));
        }
    }
}
