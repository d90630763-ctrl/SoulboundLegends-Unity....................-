using System;
using System.Collections.Generic;
using UnityEngine;
namespace SoulboundLegends.Animation
{
    public sealed class SLDescriptionMotionProvider : ISLMotionProvider
    {
        public string ProviderId => "Soulbound.ProceduralDescriptionCompiler";
        public bool CanHandle(SLAnimationActionType type)=>true;
        public List<SLAnimationAction> BuildActions(string description)
        {
            var d=(description??"").ToLowerInvariant(); var r=new List<SLAnimationAction>();
            Add(d,r,new[]{"idle","stand","standing"},SLAnimationActionType.Idle,.8f);
            Add(d,r,new[]{"walk","walking"},SLAnimationActionType.Walk,.7f);
            Add(d,r,new[]{"run","running","sprint"},SLAnimationActionType.Run,.7f);
            Add(d,r,new[]{"turn","spin","rotate"},SLAnimationActionType.Turn,.45f);
            Add(d,r,new[]{"look at","look","gaze"},SLAnimationActionType.LookAt,.4f);
            Add(d,r,new[]{"reach","reaching"},SLAnimationActionType.Reach,.35f);
            Add(d,r,new[]{"grab","grabs"},SLAnimationActionType.Grab,.3f);
            Add(d,r,new[]{"pull","pulls"},SLAnimationActionType.Pull,.35f);
            Add(d,r,new[]{"push","pushes"},SLAnimationActionType.Push,.35f);
            Add(d,r,new[]{"draw sword","draw weapon","draws his sword","draws her sword"},SLAnimationActionType.DrawWeapon,.45f);
            Add(d,r,new[]{"sheath","sheathes"},SLAnimationActionType.SheathWeapon,.45f);
            Add(d,r,new[]{"heavy slash","heavy strike","power slash"},SLAnimationActionType.HeavySlash,.65f);
            Add(d,r,new[]{"slash","slashes","cut","cuts"},SLAnimationActionType.Slash,.45f);
            Add(d,r,new[]{"attack","attacks","strike","strikes"},SLAnimationActionType.Attack,.5f);
            Add(d,r,new[]{"block","blocks","guard"},SLAnimationActionType.Block,.45f);
            Add(d,r,new[]{"dodge","dodges","evade"},SLAnimationActionType.Dodge,.55f);
            Add(d,r,new[]{"jump","jumps","leap","leaps"},SLAnimationActionType.Jump,.65f);
            Add(d,r,new[]{"fall","falls","falling"},SLAnimationActionType.Fall,.65f);
            Add(d,r,new[]{"land","lands","landing"},SLAnimationActionType.Land,.4f);
            Add(d,r,new[]{"crouch","crouches"},SLAnimationActionType.Crouch,.35f);
            Add(d,r,new[]{"stand up","stands up","rise","rises"},SLAnimationActionType.Stand,.5f);
            Add(d,r,new[]{"hit","stagger"},SLAnimationActionType.HitReact,.45f);
            Add(d,r,new[]{"death","die","dies","dying"},SLAnimationActionType.Death,.9f);
            Add(d,r,new[]{"emote","wave","bow","celebrate"},SLAnimationActionType.Emote,.9f);
            if(r.Count==0) r.Add(new SLAnimationAction{id="Custom_01",type=SLAnimationActionType.HoldPose,instruction=description,duration=.8f});
            return r;
        }
        static void Add(string d,List<SLAnimationAction> r,string[] keys,SLAnimationActionType type,float dur)
        {
            string found=null; foreach(var k in keys) if(d.Contains(k)){found=k;break;} if(found==null)return;
            int n=Count(d,found); n=Mathf.Clamp(n,1,8);
            for(int i=0;i<n;i++) r.Add(new SLAnimationAction{id=type+"_"+(i+1).ToString("00"),type=type,instruction=found,duration=dur});
        }
        static int Count(string d,string k){ int c=0,p=0; while((p=d.IndexOf(k,p,StringComparison.Ordinal))>=0){c++;p+=k.Length;} return c; }
    }
}
