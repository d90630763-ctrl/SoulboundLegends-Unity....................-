using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSMatchManager : MonoBehaviour
    {
        public CSMatchState State { get; private set; } = CSMatchState.None;
        public int currentRound { get; private set; }
        public CSResult result { get; private set; } = CSResult.None;
        public CSCombatSiteDefinition activeSite { get; private set; }

        public void StartMatch(CSCombatSiteDefinition site)
        {
            activeSite = site;
            result = CSResult.None;
            currentRound = 0;
            State = CSMatchState.Preparing;
        }

        public void StartRound()
        {
            if (State == CSMatchState.Finished) return;
            currentRound++;
            State = CSMatchState.RoundActive;
        }

        public void EndRound()
        {
            if (State != CSMatchState.RoundActive) return;
            State = CSMatchState.RoundEnding;
        }

        public void EndMatch(CSResult finalResult)
        {
            result = finalResult;
            State = CSMatchState.MatchEnding;
            State = CSMatchState.Finished;
        }

        public void CancelMatch()
        {
            result = CSResult.Cancelled;
            State = CSMatchState.Finished;
        }
    }
}
