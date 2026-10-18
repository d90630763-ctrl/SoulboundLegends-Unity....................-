using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Runtime
{
    /// <summary>
    /// Procedural stylized-fantasy actor generator. Creates actual generated meshes, layered clothing,
    /// armor, facial features, a transform skeleton, and reusable procedural motion primitives.
    /// This is a deterministic runtime generator, not a replacement for hand-authored AAA character art.
    /// </summary>
    public static class SLProceduralActorFactory
    {
        public enum ActorStyle { Player, Villager, Beast, DivineBoss }
        static readonly Dictionary<int, Material> materialCache = new Dictionary<int, Material>();
        static Mesh unitSphere;
        public static bool UseHighDetailMeshes = true;
        static readonly Dictionary<int, Mesh> sphereMeshCache = new Dictionary<int, Mesh>();

        public static GameObject Create(Transform parent, string actorName, Vector3 worldPosition, ActorStyle style, int variant = 0)
        {
            var root = new GameObject(actorName);
            root.transform.SetParent(parent, true);
            root.transform.position = worldPosition;
            root.transform.localScale = style == ActorStyle.DivineBoss ? Vector3.one * 1.28f : Vector3.one;
            var rig = root.AddComponent<SLProceduralActorRig>(); rig.style = style; rig.variant = variant;

            Color[] elements = {
                new Color(.08f,.68f,1f), new Color(1f,.19f,.055f), new Color(.35f,.72f,.22f),
                new Color(.56f,.18f,.95f), new Color(1f,.72f,.08f), new Color(.12f,.9f,.62f),
                new Color(.9f,.94f,1f), new Color(.38f,.72f,1f), new Color(.55f,.85f,1f), new Color(.85f,.25f,.95f)
            };
            Color element = elements[Mathf.Abs(variant) % elements.Length];
            Color skin = style == ActorStyle.Beast || style == ActorStyle.DivineBoss ? Color.Lerp(element, new Color(.16f,.12f,.22f), .35f) :
                         style == ActorStyle.Villager ? new Color(.62f,.38f,.23f) : new Color(.72f,.43f,.30f);
            Color cloth = style == ActorStyle.Villager ? new Color(.48f,.24f,.12f) : Color.Lerp(element, new Color(.045f,.055f,.11f), .55f);
            Color clothLight = Color.Lerp(cloth, Color.white, .25f);
            Color metal = style == ActorStyle.Villager ? new Color(.35f,.27f,.18f) : new Color(.23f,.3f,.42f);
            Color gold = new Color(.9f,.57f,.13f);

            Transform hips = Bone(root.transform, "Hips", new Vector3(0, .96f, 0));
            Transform spine = Bone(hips, "Spine", new Vector3(0, .08f, 0));
            Transform chest = Bone(spine, "Chest", new Vector3(0, .36f, 0));
            Transform neck = Bone(chest, "Neck", new Vector3(0, .38f, 0));
            Transform head = Bone(neck, "Head", new Vector3(0, .13f, 0));
            rig.Bind(hips, spine, chest, head);

            // Anatomy is generated from parametric surface meshes, not Unity primitive objects.
            Part(hips, "PelvisBody", Vector3.zero, new Vector3(.39f,.25f,.27f), skin, 20, 14);
            Part(spine, "Abdomen", new Vector3(0,.16f,0), new Vector3(.31f,.31f,.23f), skin, 20, 14);
            Part(chest, "TorsoBody", new Vector3(0,.06f,0), new Vector3(.43f,.42f,.27f), skin, 24, 16);
            Part(neck, "NeckMesh", new Vector3(0,.02f,0), new Vector3(.13f,.19f,.13f), skin, 16, 10);
            Part(head, "HeadMesh", new Vector3(0,.04f,0), style == ActorStyle.DivineBoss ? new Vector3(.3f,.35f,.28f) : new Vector3(.235f,.29f,.22f), skin, 24, 18);

            // Face: inset eyes, brows, nose, mouth and ears, positioned on the forward (+Z) side.
            if (style != ActorStyle.Beast)
            {
                Part(head,"Ear_L",new Vector3(-.235f,.03f,0),new Vector3(.055f,.09f,.055f),skin,12,8);
                Part(head,"Ear_R",new Vector3(.235f,.03f,0),new Vector3(.055f,.09f,.055f),skin,12,8);
                Color eyeWhite = new Color(.92f,.94f,1f); Color iris = element;
                Part(head,"Eye_L",new Vector3(-.082f,.075f,.194f),new Vector3(.055f,.037f,.024f),eyeWhite,14,10);
                Part(head,"Eye_R",new Vector3(.082f,.075f,.194f),new Vector3(.055f,.037f,.024f),eyeWhite,14,10);
                Part(head,"Iris_L",new Vector3(-.08f,.075f,.216f),new Vector3(.022f,.027f,.012f),iris,12,8);
                Part(head,"Iris_R",new Vector3(.084f,.075f,.216f),new Vector3(.022f,.027f,.012f),iris,12,8);
                Part(head,"Brow_L",new Vector3(-.082f,.13f,.194f),new Vector3(.065f,.018f,.025f),cloth,12,8);
                Part(head,"Brow_R",new Vector3(.082f,.13f,.194f),new Vector3(.065f,.018f,.025f),cloth,12,8);
                Part(head,"Nose",new Vector3(0,.015f,.213f),new Vector3(.035f,.055f,.045f),Color.Lerp(skin,Color.black,.12f),12,8);
                Part(head,"Mouth",new Vector3(0,-.075f,.202f),new Vector3(.055f,.012f,.012f),new Color(.22f,.055f,.055f),12,8);
                // Hair cap and locks.
                Part(head,"HairCap",new Vector3(0,.205f,-.005f),new Vector3(.245f,.13f,.23f), style==ActorStyle.Villager?new Color(.16f,.075f,.035f):new Color(.055f,.065f,.12f),20,12);
                Part(head,"HairLock_L",new Vector3(-.16f,.09f,.035f),new Vector3(.07f,.19f,.095f),style==ActorStyle.Villager?new Color(.16f,.075f,.035f):new Color(.055f,.065f,.12f),14,10);
                Part(head,"HairLock_R",new Vector3(.16f,.09f,.035f),new Vector3(.07f,.16f,.095f),style==ActorStyle.Villager?new Color(.16f,.075f,.035f):new Color(.055f,.065f,.12f),14,10);
            }
            else
            {
                Part(head,"BeastEye_L",new Vector3(-.09f,.06f,.18f),new Vector3(.045f,.035f,.02f),new Color(1f,.15f,.03f),12,8);
                Part(head,"BeastEye_R",new Vector3(.09f,.06f,.18f),new Vector3(.045f,.035f,.02f),new Color(1f,.15f,.03f),12,8);
                Part(head,"Horn_L",new Vector3(-.13f,.27f,-.01f),new Vector3(.06f,.19f,.06f),gold,12,8).localRotation=Quaternion.Euler(0,0,22);
                Part(head,"Horn_R",new Vector3(.13f,.27f,-.01f),new Vector3(.06f,.19f,.06f),gold,12,8).localRotation=Quaternion.Euler(0,0,-22);
            }

            // Layered clothing: under-tunic, jacket panels, collar, belt and segmented armor.
            Part(spine,"Tunic",new Vector3(0,.18f,0),new Vector3(.345f,.34f,.245f),cloth,22,14);
            Part(chest,"ChestJacket",new Vector3(0,.07f,.018f),new Vector3(.445f,.37f,.285f),clothLight,22,14);
            Part(chest,"ChestInset",new Vector3(0,.055f,.258f),new Vector3(.19f,.26f,.025f),cloth,16,10);
            Part(chest,"Collar",new Vector3(0,.285f,.01f),new Vector3(.2f,.075f,.2f),gold,16,10);
            Part(hips,"WaistBelt",new Vector3(0,.06f,.012f),new Vector3(.405f,.075f,.285f),gold,20,10);
            Part(hips,"Buckle",new Vector3(0,.06f,.165f),new Vector3(.075f,.075f,.025f),element,12,8);
            Part(hips,"TunicSkirt",new Vector3(0,-.12f,0),new Vector3(.39f,.18f,.28f),cloth,20,12);
            Part(chest,"ShoulderArmor_L",new Vector3(-.39f,.22f,0),new Vector3(.17f,.13f,.18f),metal,16,10);
            Part(chest,"ShoulderArmor_R",new Vector3(.39f,.22f,0),new Vector3(.17f,.13f,.18f),metal,16,10);
            Part(chest,"ShoulderRune_L",new Vector3(-.39f,.22f,.155f),new Vector3(.07f,.035f,.018f),element,12,8);
            Part(chest,"ShoulderRune_R",new Vector3(.39f,.22f,.155f),new Vector3(.07f,.035f,.018f),element,12,8);
            Part(chest,"BackCrystal",new Vector3(0,.18f,-.3f),new Vector3(.12f,.28f,.12f),element,16,12);

            Transform leftUpperArm = MakeArm(chest, "Left", -1, skin, cloth, metal, gold, element, rig);
            Transform rightUpperArm = MakeArm(chest, "Right", 1, skin, cloth, metal, gold, element, rig);
            Transform leftUpperLeg = MakeLeg(hips, "Left", -1, skin, cloth, metal, rig);
            Transform rightUpperLeg = MakeLeg(hips, "Right", 1, skin, cloth, metal, rig);
            rig.BindLimbs(leftUpperArm, rightUpperArm, leftUpperLeg, rightUpperLeg);

            if (style == ActorStyle.Player || style == ActorStyle.DivineBoss)
            {
                Part(head,"CrownPoint_L",new Vector3(-.12f,.29f,-.015f),new Vector3(.045f,.12f,.045f),gold,12,8).localRotation=Quaternion.Euler(0,0,20);
                Part(head,"CrownPoint_R",new Vector3(.12f,.29f,-.015f),new Vector3(.045f,.12f,.045f),gold,12,8).localRotation=Quaternion.Euler(0,0,-20);
                Part(chest,"ChestSigil",new Vector3(0,.09f,.29f),new Vector3(.075f,.09f,.025f),element,14,10);
            }
            if (style == ActorStyle.Beast || style == ActorStyle.DivineBoss)
            {
                Part(hips,"Tail",new Vector3(0,-.02f,-.31f),new Vector3(.11f,.45f,.11f),metal,16,10).localRotation=Quaternion.Euler(55,0,0);
                Part(chest,"Spine_L",new Vector3(-.22f,.29f,-.18f),new Vector3(.08f,.18f,.08f),gold,12,8).localRotation=Quaternion.Euler(0,0,25);
                Part(chest,"Spine_R",new Vector3(.22f,.29f,-.18f),new Vector3(.08f,.18f,.08f),gold,12,8).localRotation=Quaternion.Euler(0,0,-25);
            }

            if (style == ActorStyle.DivineBoss)
            {
                root.AddComponent<SLSimpleEnemy>(); var combat = root.AddComponent<SLRuntimeCombatant>(); combat.maxHealth=5000; combat.health=5000;
            }
            else if (style == ActorStyle.Beast)
            {
                root.AddComponent<SLSimpleEnemy>(); var combat = root.AddComponent<SLRuntimeCombatant>(); combat.maxHealth=250; combat.health=250;
            }
            if (style == ActorStyle.Player)
            {
                root.AddComponent<SLRuntimeCombatInput>(); var combat = root.AddComponent<SLRuntimeCombatant>(); combat.maxHealth=1000; combat.health=1000;
            }
            rig.CacheBones();
            return root;
        }

        static Transform MakeArm(Transform chest, string side, int sign, Color skin, Color cloth, Color metal, Color gold, Color element, SLProceduralActorRig rig)
        {
            Transform upper = Bone(chest, side+"ArmUpper", new Vector3(sign*.42f,.13f,0));
            Part(upper,side+"UpperArmMesh",new Vector3(0,-.13f,0),new Vector3(.115f,.21f,.12f),skin,16,12);
            Part(upper,side+"Sleeve",new Vector3(0,-.12f,0),new Vector3(.145f,.19f,.145f),cloth,16,12);
            Part(upper,side+"Bracer",new Vector3(0,-.29f,.012f),new Vector3(.105f,.11f,.12f),metal,14,10);
            Transform lower = Bone(upper,side+"ArmLower",new Vector3(0,-.29f,0));
            Part(lower,side+"Forearm",new Vector3(0,-.13f,0),new Vector3(.085f,.2f,.09f),skin,16,12);
            Part(lower,side+"ForearmGuard",new Vector3(0,-.1f,.035f),new Vector3(.105f,.13f,.08f),metal,14,10);
            Transform hand = Bone(lower,side+"Hand",new Vector3(0,-.28f,0));
            Part(hand,side+"Palm",Vector3.zero,new Vector3(.09f,.09f,.045f),skin,14,10);
            for(int f=0;f<4;f++) Part(hand,side+"Finger"+f,new Vector3((f-1.5f)*.032f,-.07f,.012f),new Vector3(.022f,.07f,.024f),skin,10,7);
            Part(hand,side+"Thumb",new Vector3(sign*.055f,-.015f,.02f),new Vector3(.025f,.06f,.026f),skin,10,7);
            rig.RegisterArm(side, upper, lower, hand);
            return upper;
        }
        static Transform MakeLeg(Transform hips, string side, int sign, Color skin, Color cloth, Color metal, SLProceduralActorRig rig)
        {
            Transform upper=Bone(hips,side+"LegUpper",new Vector3(sign*.19f,-.1f,0));
            Part(upper,side+"Thigh",new Vector3(0,-.19f,0),new Vector3(.15f,.27f,.16f),cloth,16,12);
            Part(upper,side+"ThighArmor",new Vector3(0,-.17f,.035f),new Vector3(.16f,.19f,.13f),metal,14,10);
            Transform lower=Bone(upper,side+"LegLower",new Vector3(0,-.39f,0));
            Part(lower,side+"Shin",new Vector3(0,-.18f,0),new Vector3(.105f,.25f,.12f),cloth,16,12);
            Part(lower,side+"ShinGuard",new Vector3(0,-.17f,.065f),new Vector3(.11f,.2f,.065f),metal,14,10);
            Transform foot=Bone(lower,side+"Foot",new Vector3(0,-.37f,.055f));
            Part(foot,side+"Boot",new Vector3(0,-.015f,.035f),new Vector3(.13f,.085f,.21f),new Color(.12f,.085f,.065f),14,10);
            rig.RegisterLeg(side,upper,lower,foot);
            return upper;
        }
        static Transform Bone(Transform parent,string name,Vector3 pos){var g=new GameObject(name);g.transform.SetParent(parent,false);g.transform.localPosition=pos;return g.transform;}
        static Transform Part(Transform parent,string name,Vector3 pos,Vector3 scale,Color color,int lon=18,int lat=12)
        {
            var go=new GameObject(name);go.transform.SetParent(parent,false);go.transform.localPosition=pos;go.transform.localScale=scale;
            // Higher tessellation smooths silhouettes while shared meshes keep repeated body parts inexpensive.
            if (UseHighDetailMeshes) { lon = Mathf.Max(lon, 28); lat = Mathf.Max(lat, 20); }
            var mf=go.AddComponent<MeshFilter>();mf.sharedMesh=GetSphere(lon,lat);var mr=go.AddComponent<MeshRenderer>();mr.sharedMaterial=MaterialFor(color);return go.transform;
        }
        static Mesh GetSphere(int longitude,int latitude)
        {
            // Shared high-resolution parametric surface meshes. Reuse by resolution to limit allocations.
            int cacheKey = longitude * 1000 + latitude;
            if (sphereMeshCache.TryGetValue(cacheKey, out var cached) && cached != null) return cached;
            var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            for(int y=0;y<=latitude;y++){float v=y/(float)latitude,phi=v*Mathf.PI;for(int x=0;x<=longitude;x++){float u=x/(float)longitude,theta=u*Mathf.PI*2;Vector3 n=new Vector3(Mathf.Sin(phi)*Mathf.Cos(theta),Mathf.Cos(phi),Mathf.Sin(phi)*Mathf.Sin(theta));vertices.Add(n*.5f);normals.Add(n);uv.Add(new Vector2(u,v));}}
            for(int y=0;y<latitude;y++)for(int x=0;x<longitude;x++){int a=y*(longitude+1)+x,b=a+longitude+1;triangles.Add(a);triangles.Add(b);triangles.Add(a+1);triangles.Add(a+1);triangles.Add(b);triangles.Add(b+1);}
            var m=new Mesh{name="SL_ProceduralSurface_"+longitude+"x"+latitude};if(vertices.Count>65000)m.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;m.SetVertices(vertices);m.SetNormals(normals);m.SetUVs(0,uv);m.SetTriangles(triangles,0);m.RecalculateBounds();sphereMeshCache[cacheKey]=m;if(longitude==18&&latitude==12)unitSphere=m;return m;
        }
        static Material MaterialFor(Color color)
        {
            int key=ColorUtility.ToHtmlStringRGBA(color).GetHashCode();if(materialCache.TryGetValue(key,out var m)&&m!=null)return m;
            Shader shader=Shader.Find("Universal Render Pipeline/Lit");if(shader==null)shader=Shader.Find("Standard");if(shader==null)shader=Shader.Find("Diffuse");
            m=new Material(shader){name="SL_ProceduralMaterial_"+ColorUtility.ToHtmlStringRGBA(color),color=color,enableInstancing=true};
            if(m.HasProperty("_Metallic"))m.SetFloat("_Metallic",color.r>.65f&&color.g>.35f?.42f:.12f);if(m.HasProperty("_Smoothness"))m.SetFloat("_Smoothness",.42f);
            materialCache[key]=m;return m;
        }
    }

    /// <summary>Composable motion primitives: idle, walk/run, attack, block, dodge and jump are blended procedurally.</summary>
    public sealed class SLProceduralActorRig : MonoBehaviour
    {
        public SLProceduralActorFactory.ActorStyle style; public int variant;
        Transform hips,spine,chest,head,leftArm,rightArm,leftForearm,rightForearm,leftHand,rightHand,leftLeg,rightLeg,leftShin,rightShin,leftFoot,rightFoot;
        [Header("Optional procedural IK targets")]
        public Transform lookTarget, leftHandTarget, rightHandTarget;
        Vector3 rootStart; float actionUntil; string action="Idle"; float actionStart; Vector3 rootActionStart;
        public string CurrentAction=>action;
        public void Bind(Transform h,Transform s,Transform c,Transform hd){hips=h;spine=s;chest=c;head=hd;}
        public void RegisterArm(string side,Transform upper,Transform lower,Transform hand){if(side=="Left"){leftArm=upper;leftForearm=lower;leftHand=hand;}else{rightArm=upper;rightForearm=lower;rightHand=hand;}}
        public void RegisterLeg(string side,Transform upper,Transform lower,Transform foot){if(side=="Left"){leftLeg=upper;leftShin=lower;leftFoot=foot;}else{rightLeg=upper;rightShin=lower;rightFoot=foot;}}
        public void BindLimbs(Transform la,Transform ra,Transform ll,Transform rl){}
        public void CacheBones(){rootStart=transform.position;}
        public void PlayAction(string name,float duration=.55f){action=string.IsNullOrEmpty(name)?"Idle":name;actionStart=Time.time;actionUntil=actionStart+Mathf.Max(.05f,duration);rootActionStart=transform.position;}
        public void PlayCombo(string combo){ // Complex moves are decomposed into short, reusable action primitives.
            string c=(combo??"").ToLowerInvariant();if(c.Contains("heavy")){PlayAction("Windup",.22f);StartCoroutine(ComboSequence(new[]{"Windup","HeavyAttack","Recover"},new[]{.22f,.38f,.25f}));}
            else if(c.Contains("dodge"))PlayAction("Dodge",.38f);else if(c.Contains("jump"))PlayAction("Jump",.55f);else PlayAction("Attack",.42f);
        }
        System.Collections.IEnumerator ComboSequence(string[] steps,float[] durations){for(int i=0;i<steps.Length;i++){PlayAction(steps[i],durations[i]);yield return new WaitForSeconds(durations[i]);}}
        void Update()
        {
            float t=Time.time;float speed=style==SLProceduralActorFactory.ActorStyle.Beast?4.3f:2.3f;float wave=Mathf.Sin(t*speed);float idle=Mathf.Sin(t*1.6f);
            if(action!="Idle" && action!="Walk" && action!="Run" && t>=actionUntil)
            {
                if(action=="Dodge" || action=="Jump") transform.position=rootActionStart;
                action="Idle";
            }
            float walk=action=="Walk"||action=="Run"?1f:0f;
            float armSwing=walk*wave*28f;float legSwing=walk*wave*32f;
            SetEuler(leftArm,armSwing,0,0);SetEuler(rightArm,-armSwing,0,0);SetEuler(leftForearm,-Mathf.Max(0,armSwing)*.45f,0,0);
            SetEuler(rightForearm,-Mathf.Max(0,-armSwing)*.45f,0,0);SetEuler(leftLeg,-legSwing,0,0);SetEuler(rightLeg,legSwing,0,0);
            SetEuler(leftShin,Mathf.Max(0,legSwing)*.65f,0,0);SetEuler(rightShin,Mathf.Max(0,-legSwing)*.65f,0,0);
            SetEuler(head,0,Mathf.Sin(t*.5f)*4f,Mathf.Sin(t*1.2f)*1.2f);SetEuler(spine,Mathf.Sin(t*1.3f)*1.2f,0,0);
            if(action=="Attack"||action=="HeavyAttack"){float p=Mathf.Clamp01((t-actionStart)/Mathf.Max(.01f,actionUntil-actionStart));float swing=Mathf.Sin(p*Mathf.PI)* (action=="HeavyAttack"?115f:85f);SetEuler(rightArm,-25f-swing,0,-12f);SetEuler(rightForearm,-35f-swing*.35f,0,0);SetEuler(chest,0,-swing*.12f,0);}
            else if(action=="Windup"){SetEuler(rightArm,-105,0,-22);SetEuler(chest,0,-15,0);}
            else if(action=="Recover"){SetEuler(rightArm,-20,0,0);SetEuler(chest,0,4,0);}
            else if(action=="Block"){SetEuler(leftArm,-65,0,18);SetEuler(rightArm,-65,0,-18);SetEuler(leftForearm,-35,0,0);SetEuler(rightForearm,-35,0,0);}
            else if(action=="Dodge"){float p=Mathf.Clamp01((t-actionStart)/Mathf.Max(.01f,actionUntil-actionStart));float pulse=Mathf.Sin(p*Mathf.PI);SetEuler(hips,0,0,pulse*38);SetEuler(spine,20,0,-pulse*20);transform.position=rootActionStart+transform.right*(pulse*(variant%2==0?1:-1)*.9f);}
            else if(action=="Jump"){float p=Mathf.Clamp01((t-actionStart)/Mathf.Max(.01f,actionUntil-actionStart));transform.position=rootActionStart+Vector3.up*(Mathf.Sin(p*Mathf.PI)*.75f);SetEuler(leftLeg,-20,0,0);SetEuler(rightLeg,-20,0,0);}
            else {SetEuler(hips,0,0,0);SetEuler(chest,Mathf.Sin(t*1.3f)*1.2f,0,0);if(leftFoot)SetEuler(leftFoot,0,0,0);if(rightFoot)SetEuler(rightFoot,0,0,0);if(action!="Block"&&action!="Walk"&&action!="Run"){SetEuler(rightArm,idle*2f,0,-3);SetEuler(leftArm,-idle*2f,0,3);}}
        }
        public void SetLocomotion(float normalizedSpeed){if(action=="Idle"||action=="Walk"||action=="Run")action=normalizedSpeed<.05f?"Idle":normalizedSpeed<.55f?"Walk":"Run";}
        public void SetLookTarget(Transform target){lookTarget=target;}
        public void SetHandTarget(bool right,Transform target){if(right)rightHandTarget=target;else leftHandTarget=target;}
        void LateUpdate()
        {
            // Lightweight procedural target alignment for generated rigs; authored clips can be layered later.
            if(lookTarget&&head){Vector3 d=lookTarget.position-head.position;if(d.sqrMagnitude>.001f)head.rotation=Quaternion.Slerp(head.rotation,Quaternion.LookRotation(d,Vector3.up),Time.deltaTime*5f);}
            SolveArmTarget(rightArm,rightForearm,rightHand,rightHandTarget);
            SolveArmTarget(leftArm,leftForearm,leftHand,leftHandTarget);
        }
        static void SolveArmTarget(Transform upper,Transform lower,Transform hand,Transform target)
        {
            if(!upper||!lower||!hand||!target)return;
            Vector3 direction=target.position-upper.position;if(direction.sqrMagnitude<.001f)return;
            upper.rotation=Quaternion.FromToRotation(upper.TransformDirection(Vector3.down),direction.normalized)*upper.rotation;
            direction=target.position-lower.position;if(direction.sqrMagnitude>.001f)lower.rotation=Quaternion.FromToRotation(lower.TransformDirection(Vector3.down),direction.normalized)*lower.rotation;
        }
        static void SetEuler(Transform t,float x,float y,float z){if(t)t.localRotation=Quaternion.Euler(x,y,z);}
    }
}
