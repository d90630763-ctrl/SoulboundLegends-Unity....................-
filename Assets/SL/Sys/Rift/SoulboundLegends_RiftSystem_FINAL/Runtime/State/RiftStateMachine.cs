using System;
using System.Collections.Generic;

namespace SoulboundLegends.RiftSystem
{
    public sealed class RiftStateMachine
    {
        private static readonly Dictionary<RiftState, HashSet<RiftState>> Allowed = new()
        {
            [RiftState.Normal] = new() { RiftState.Appearing },
            [RiftState.Appearing] = new() { RiftState.Active, RiftState.Failed },
            [RiftState.Active] = new() { RiftState.InProgress, RiftState.TimeExpired, RiftState.AttemptsExhausted, RiftState.Cleared },
            [RiftState.InProgress] = new() { RiftState.BossActive, RiftState.TimeExpired, RiftState.AttemptsExhausted, RiftState.Cleared },
            [RiftState.BossActive] = new() { RiftState.Cleared, RiftState.TimeExpired, RiftState.AttemptsExhausted },
            [RiftState.TimeExpired] = new() { RiftState.Merging },
            [RiftState.AttemptsExhausted] = new() { RiftState.Merging },
            [RiftState.Merging] = new() { RiftState.Engulfed },
            [RiftState.Engulfed] = new() { RiftState.Cleared, RiftState.CityDestroyed },
            [RiftState.Cleared] = new(),
            [RiftState.CityDestroyed] = new(),
            [RiftState.Failed] = new()
        };

        public bool CanTransition(RiftState from, RiftState to) =>
            Allowed.TryGetValue(from, out var next) && next.Contains(to);

        public bool TryTransition(RiftStateSnapshot snapshot, RiftState target, long timestamp, out string error)
        {
            if (!CanTransition(snapshot.State, target))
            {
                error = $"Illegal Rift state transition: {snapshot.State} -> {target}.";
                return false;
            }

            snapshot.State = target;
            snapshot.StateChangedUnixMilliseconds = timestamp;
            error = null;
            return true;
        }
    }
}
