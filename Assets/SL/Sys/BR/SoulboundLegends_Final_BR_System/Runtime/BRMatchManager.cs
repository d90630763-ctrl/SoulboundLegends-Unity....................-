using UnityEngine;

namespace SoulboundLegends.BR
{
    public enum BRMatchState { Waiting, Teleporting, Active, FinalZone, Completed, ReturningToLobby }

    public sealed class BRMatchManager : MonoBehaviour
    {
        public BRMatchState state = BRMatchState.Waiting;
        public int alivePlayers;
        public int placement;
        public bool victory;

        public void BeginTeleportation()
        {
            state = BRMatchState.Teleporting;
        }

        public void BeginMatch(int playerCount)
        {
            alivePlayers = playerCount;
            state = BRMatchState.Active;
        }

        public void EnterFinalZone()
        {
            state = BRMatchState.FinalZone;
        }

        public void CompleteMatch(bool playerWon)
        {
            victory = playerWon;
            state = BRMatchState.Completed;
        }

        public void ReturnToLobby()
        {
            state = BRMatchState.ReturningToLobby;
        }
    }
}
