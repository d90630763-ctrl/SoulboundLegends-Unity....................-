using UnityEngine;
namespace GameSystems.MonsterHumanGeneration {
public sealed class TwoBoneIK:MonoBehaviour{
 public Transform upper,lower,end,target;[Range(0,1)]public float weight=1;
 void LateUpdate(){if(!upper||!lower||!end||!target)return;Vector3 ab=lower.position-upper.position,bc=end.position-lower.position,dir=target.position-upper.position;float l1=ab.magnitude,l2=bc.magnitude,d=Mathf.Clamp(dir.magnitude,.001f,l1+l2-.001f);dir.Normalize();Vector3 n=Vector3.Cross(ab,bc);if(n.sqrMagnitude<1e-6f)n=Vector3.Cross(dir,Vector3.up);n.Normalize();Vector3 bend=Vector3.Cross(n,dir).normalized;float x=(d*d+l1*l1-l2*l2)/(2*d),h=Mathf.Sqrt(Mathf.Max(0,l1*l1-x*x));Vector3 wanted=upper.position+dir*x+bend*h;lower.position=Vector3.Lerp(lower.position,wanted,weight);upper.rotation=Quaternion.Slerp(upper.rotation,Quaternion.FromToRotation(end.position-upper.position,target.position-upper.position)*upper.rotation,weight);}
}
}