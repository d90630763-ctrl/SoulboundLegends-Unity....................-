using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSScoringSystem : MonoBehaviour
    {
        public int teamARoundWins { get; private set; }
        public int teamBRoundWins { get; private set; }

        public void ResetMatch()
        {
            teamARoundWins = 0;
            teamBRoundWins = 0;
        }

        public void RecordRoundWin(CSTeam team)
        {
            if (team == CSTeam.TeamA) teamARoundWins++;
            if (team == CSTeam.TeamB) teamBRoundWins++;
        }

        public bool HasMatchWinner(int roundsToWin, out CSTeam winner)
        {
            if (teamARoundWins >= roundsToWin) { winner = CSTeam.TeamA; return true; }
            if (teamBRoundWins >= roundsToWin) { winner = CSTeam.TeamB; return true; }
            winner = CSTeam.None;
            return false;
        }

        public void RecordKill(CSPlayerSlot killer, CSPlayerSlot victim)
        {
            if (killer != null) killer.kills++;
            if (victim != null) { victim.deaths++; victim.alive = false; }
        }

        public void RecordAssist(CSPlayerSlot assister)
        {
            if (assister != null) assister.assists++;
        }
    }
}
