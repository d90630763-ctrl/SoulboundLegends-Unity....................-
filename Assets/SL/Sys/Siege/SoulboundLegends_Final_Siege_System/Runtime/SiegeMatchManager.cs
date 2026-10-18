using System; using UnityEngine;
namespace SoulboundLegends.Siege {
public enum SiegeMatchState{Waiting,RoundActive,RoundCompleted,ScoreOneOne,FinalRoundActive,Completed,ReturningToLobby}
public enum SiegeTeamRole{Attacker,Defender,FinalAttackAndDefend}
[Serializable] public class SiegeTeamState{public string teamName;public int points;public SiegeTeamRole currentRole;public bool coreAlive=true,eliminated;public float attackElapsedSeconds,coreHealth=100000;}
public class SiegeMatchManager:MonoBehaviour{
public SiegeRules rules;public SiegeMatchState state=SiegeMatchState.Waiting;public SiegeTeamState teamA=new SiegeTeamState{teamName="Team A"},teamB=new SiegeTeamState{teamName="Team B"};public int currentRound;public float roundTimeRemaining;public event Action<string> MatchEvent;public event Action<int,int> ScoreChanged;public event Action<string> MatchCompleted;
public void StartMatch(){currentRound=1;teamA.points=teamB.points=0;teamA.currentRole=SiegeTeamRole.Defender;teamB.currentRole=SiegeTeamRole.Attacker;StartNormalRound();}
void StartNormalRound(){state=SiegeMatchState.RoundActive;roundTimeRemaining=rules?rules.normalRoundTimeSeconds:600;ResetRound();Emit("Normal Siege round started.");}
void ResetRound(){teamA.coreAlive=teamB.coreAlive=true;teamA.eliminated=teamB.eliminated=false;teamA.coreHealth=teamB.coreHealth=100000;}
public void Tick(float dt){if(state!=SiegeMatchState.RoundActive&&state!=SiegeMatchState.FinalRoundActive)return;roundTimeRemaining-=dt;if(roundTimeRemaining<=0&&state==SiegeMatchState.RoundActive)DefenderTimeout();}
public void NotifyCoreDestroyed(string defendingTeam){if(state==SiegeMatchState.FinalRoundActive){Complete(defendingTeam=="Team A"?"Team B":"Team A");return;}Award(defendingTeam=="Team A"?"Team B":"Team A");}
public void NotifyAttackersWiped(string attackingTeam){if(state==SiegeMatchState.RoundActive)Award(attackingTeam=="Team A"?"Team B":"Team A");}
void DefenderTimeout(){Award(teamA.currentRole==SiegeTeamRole.Defender?"Team A":"Team B");}
void Award(string winner){if(winner=="Team A")teamA.points++;else teamB.points++;ScoreChanged?.Invoke(teamA.points,teamB.points);state=SiegeMatchState.RoundCompleted;Emit(winner+" won the round.");if(teamA.points>=2||teamB.points>=2){Complete(teamA.points>=2?"Team A":"Team B");return;}if(currentRound==1){currentRound=2;var r=teamA.currentRole;teamA.currentRole=teamB.currentRole;teamB.currentRole=r;StartNormalRound();}else if(teamA.points==1&&teamB.points==1)StartFinalRound();else Complete(teamA.points>teamB.points?"Team A":"Team B");}
void StartFinalRound(){state=SiegeMatchState.FinalRoundActive;roundTimeRemaining=rules?rules.finalRoundTimeSeconds:900;teamA.currentRole=teamB.currentRole=SiegeTeamRole.FinalAttackAndDefend;ResetRound();Emit("Final Siege: both teams attack and defend simultaneously.");}
void Complete(string winner){state=SiegeMatchState.Completed;MatchCompleted?.Invoke(winner);Emit("Siege winner: "+winner);}
void Emit(string s){MatchEvent?.Invoke(s);}
}}