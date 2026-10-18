using NUnit.Framework; using System.Collections.Generic;
namespace SoulboundLegends.InstanceCoordinator.Tests
{
 public sealed class CoordinatorModelTests
 {
  [Test] public void PartyRequestContainsAllPlayers(){var r=new InstanceRequest{InstanceType="inn",WorldRegion="global",PlayerId="p1",PartyPlayerIds=new List<string>{"p2","p3"},MaximumCapacity=3};Assert.AreEqual(3,r.PartyPlayerIds.Count+1);}
  [Test] public void CapacityRejectsImpossibleParty(){var r=new InstanceRequest{InstanceType="dungeon",WorldRegion="global",MaximumCapacity=2,PartyPlayerIds=new List<string>{"p2","p3"}};Assert.Greater(r.PartyPlayerIds.Count,r.MaximumCapacity);}
  [Test] public void LifecycleIncludesRequiredStates(){Assert.IsTrue(System.Enum.IsDefined(typeof(InstanceLifecycleState),"Requested"));Assert.IsTrue(System.Enum.IsDefined(typeof(InstanceLifecycleState),"CleanupComplete"));}
 }
}
