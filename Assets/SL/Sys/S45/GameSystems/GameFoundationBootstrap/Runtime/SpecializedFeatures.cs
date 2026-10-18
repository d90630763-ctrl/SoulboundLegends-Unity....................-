using System;
using System.Collections.Generic;
using UnityEngine;
namespace GameSystems.GameFoundationBootstrap {
    public interface IGameSystem { string Id {get;} void Initialize(); void Shutdown(); }
    public sealed class GameFoundationRegistry:MonoBehaviour {
        static readonly List<IGameSystem> systems=new List<IGameSystem>();
        public static void Register(IGameSystem system) { if(system!=null&&!systems.Contains(system)) systems.Add(system); }
        public void InitializeAll() { foreach(var s in systems) s.Initialize(); }
        public void ShutdownAll() { for(int i=systems.Count-1;i>=0;i--) systems[i].Shutdown(); }
    }
}