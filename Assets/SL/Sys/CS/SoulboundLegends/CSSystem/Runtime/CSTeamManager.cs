using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.CS
{
    public enum CSTeam { None, TeamA, TeamB }

    [Serializable]
    public class CSPlayerSlot
    {
        public string playerId;
        public CSTeam team;
        public bool connected;
        public bool alive;
        public float battlePower;
        public int kills;
        public int deaths;
        public int assists;
    }

    public class CSTeamManager : MonoBehaviour
    {
        public List<CSPlayerSlot> teamA = new List<CSPlayerSlot>();
        public List<CSPlayerSlot> teamB = new List<CSPlayerSlot>();

        public void BuildTeams(List<CSPlayerSlot> players, int playersPerTeam)
        {
            teamA.Clear(); teamB.Clear();
            players.Sort((a,b) => b.battlePower.CompareTo(a.battlePower));
            float aPower = 0, bPower = 0;
            foreach (var p in players)
            {
                if (teamA.Count >= playersPerTeam) { p.team = CSTeam.TeamB; teamB.Add(p); bPower += p.battlePower; }
                else if (teamB.Count >= playersPerTeam) { p.team = CSTeam.TeamA; teamA.Add(p); aPower += p.battlePower; }
                else if (aPower <= bPower) { p.team = CSTeam.TeamA; teamA.Add(p); aPower += p.battlePower; }
                else { p.team = CSTeam.TeamB; teamB.Add(p); bPower += p.battlePower; }
            }
        }

        public int AliveCount(CSTeam team)
        {
            var list = team == CSTeam.TeamA ? teamA : teamB;
            int count = 0;
            foreach (var p in list) if (p.alive && p.connected) count++;
            return count;
        }

        public void ResetRound()
        {
            foreach (var p in teamA) { p.alive = true; p.kills = 0; p.deaths = 0; p.assists = 0; }
            foreach (var p in teamB) { p.alive = true; p.kills = 0; p.deaths = 0; p.assists = 0; }
        }
    }
}
