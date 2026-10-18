using System;
using UnityEngine;
namespace GameSystems.MonsterHumanGeneration {
[CreateAssetMenu(fileName="GenerationProfile",menuName="Game Systems/Monster Human/Generation Profile")]
public sealed class GenerationProfile:ScriptableObject{
 public string entityId="entity_001",displayName="Generated Entity"; [TextArea(2,8)] public string description;
 public GeneratedBodyType bodyType=GeneratedBodyType.Humanoid; public CreatureBodyLayout creatureLayout=CreatureBodyLayout.Biped;
 [Min(.1f)]public float height=1.8f,bodyWidth=.55f,bodyDepth=.32f,limbThickness=.12f;
 [Range(4,64)]public int radialSegments=16; [Range(1,16)]public int verticalSegments=4;
 [Min(0)]public float bodyLength=1.8f,wingSpan=2.4f,tailLength=.8f; [Range(1,8)]public int legCount=4;
 public Color baseColor=new Color(.15f,.2f,.5f),secondaryColor=new Color(.75f,.75f,.8f),emissionColor=Color.black;
 [Min(0)]public float emissionStrength; [TextArea(1,5)]public string textureDescription;
 [TextArea(1,5)]public string animationDescription="Idle breathing, walk forward, run forward, attack with the right arm.";
 public bool createTexture=true,createMaterial=true,createAnimationClips=true,createPrefab=true;
 public void EnsureId(){if(string.IsNullOrWhiteSpace(entityId))entityId=Guid.NewGuid().ToString("N");}
}
public enum GeneratedBodyType{Humanoid,Creature}
public enum CreatureBodyLayout{Biped,Quadruped,Serpentine,WingedBiped}
}