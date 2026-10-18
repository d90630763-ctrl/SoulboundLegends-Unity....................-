using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.AdventureWorld
{
    public class AdventureWorldPlayerState : MonoBehaviour
    {
        public PlayerWorldState state = new();
        static readonly Dictionary<string, PlayerWorldState> Memory = new();

        public void LoadOrCreate(string playerId)
        {
            if (string.IsNullOrWhiteSpace(playerId)) playerId = "LOCAL_PLAYER";
            if (!Memory.TryGetValue(playerId, out var saved))
            {
                saved = new PlayerWorldState { playerId = playerId, level = 1, xp = 0, skillPoints = 0 };
                Memory[playerId] = saved;
            }
            state = saved;
        }

        public void Save()
        {
            if (state == null || string.IsNullOrEmpty(state.playerId)) return;
            Memory[state.playerId] = state;
        }

        public void AddMonsterXP(long amount)
        {
            state.xp += Mathf.Max(0, (int)Mathf.Min(int.MaxValue, amount));
            int newLevel = Mathf.Max(1, 1 + (int)(state.xp / 1000L));
            if (newLevel > state.level) state.skillPoints += newLevel - state.level;
            state.level = newLevel;
            Save();
        }
    }
}
