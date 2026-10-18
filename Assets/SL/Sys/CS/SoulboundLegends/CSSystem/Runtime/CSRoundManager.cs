using UnityEngine;

namespace SoulboundLegends.CS
{
    public class CSRoundManager : MonoBehaviour
    {
        public float timeRemaining;
        public bool suddenDeath;
        public CSTeam roundWinner = CSTeam.None;

        public void BeginRound(float duration)
        {
            timeRemaining = duration;
            suddenDeath = false;
            roundWinner = CSTeam.None;
        }

        public void Tick(float serverDeltaTime)
        {
            if (timeRemaining <= 0) return;
            timeRemaining -= Mathf.Max(0, serverDeltaTime);
            if (timeRemaining <= 0) EnterSuddenDeath();
        }

        public void EnterSuddenDeath()
        {
            suddenDeath = true;
            timeRemaining = 0;
        }

        public CSTeam ResolveByElimination(int aliveA, int aliveB)
        {
            if (aliveA <= 0 && aliveB <= 0) return CSTeam.None;
            if (aliveA <= 0) return CSTeam.TeamB;
            if (aliveB <= 0) return CSTeam.TeamA;
            return CSTeam.None;
        }
    }
}
