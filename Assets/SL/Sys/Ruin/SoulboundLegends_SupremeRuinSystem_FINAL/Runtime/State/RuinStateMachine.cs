using System.Collections.Generic;

namespace SoulboundLegends.SupremeRuin
{
    public sealed class SupremeRuinStateMachine
    {
        private static readonly Dictionary<RuinState, HashSet<RuinState>> Allowed = new()
        {
            { RuinState.Dormant, new HashSet<RuinState>{RuinState.Available} },
            { RuinState.Available, new HashSet<RuinState>{RuinState.Occupied} },
            { RuinState.Occupied, new HashSet<RuinState>{RuinState.TrialStarting, RuinState.Terminating, RuinState.Corrupted} },
            { RuinState.TrialStarting, new HashSet<RuinState>{RuinState.TrialActive, RuinState.Failed, RuinState.Terminating} },
            { RuinState.TrialActive, new HashSet<RuinState>{RuinState.Completed, RuinState.Failed, RuinState.Terminating, RuinState.Corrupted} },
            { RuinState.Completed, new HashSet<RuinState>{RuinState.Available, RuinState.Terminating} },
            { RuinState.Failed, new HashSet<RuinState>{RuinState.Available, RuinState.Terminating} },
            { RuinState.Terminating, new HashSet<RuinState>{RuinState.Available, RuinState.Corrupted} },
            { RuinState.Corrupted, new HashSet<RuinState>() }
        };

        public bool TryTransition(SupremeRuinStateSnapshot state, RuinState next)
        {
            if (state == null || !Allowed.TryGetValue(state.RuinState, out var set) || !set.Contains(next))
                return false;
            state.RuinState = next;
            return true;
        }
    }
}
