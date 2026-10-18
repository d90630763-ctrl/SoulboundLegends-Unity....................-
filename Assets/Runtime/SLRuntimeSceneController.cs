using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace SoulboundLegends.Runtime
{
    public enum SLSceneKind { Opening, CharacterCreation, Lobby, AdventureWorld, Clash, BattleRoyale, Siege, SupremeRuin, Rift }

    public sealed class SLRuntimeSceneController : MonoBehaviour
    {
        public SLSceneKind sceneKind;
        public string sceneDisplayName = "Soulbound Legends";
        public int seed = 1001;
        public bool buildRuntimeWorld = true;
        public bool buildHud = true;
        public bool allowSceneNavigation = true;
        [Header("Opening Video — assign your intro clip here")]
        public VideoClip introVideo;
        public bool playOpeningVideo = true;
        private Transform worldRoot;
        private Transform player;
        private readonly List<GameObject> spawned = new();
        private readonly Dictionary<string, Material> mats = new();

        void Start()
        {
            Application.targetFrameRate = 60;
            if (buildRuntimeWorld) BuildWorld();
            if (buildHud) BuildHUD();
            if (sceneKind == SLSceneKind.Opening && playOpeningVideo && introVideo != null) PlayOpeningVideo();
        }


        void PlayOpeningVideo()
        {
            if (!Camera.main) { Debug.LogWarning("Opening video clip is assigned, but there is no Main Camera."); return; }
            var video = Camera.main.gameObject.GetComponent<VideoPlayer>();
            if (!video) video = Camera.main.gameObject.AddComponent<VideoPlayer>();
            video.playOnAwake = false; video.isLooping = false; video.source = VideoSource.VideoClip;
            video.clip = introVideo; video.renderMode = VideoRenderMode.CameraNearPlane; video.targetCamera = Camera.main;
            video.audioOutputMode = VideoAudioOutputMode.Direct; video.Play();
        }

        void BuildWorld()
        {
            worldRoot = new GameObject("SL_RuntimeWorld").transform;
            Random.InitState(seed);
            MakeLight();
            MakeGround();
            switch (sceneKind)
            {
                case SLSceneKind.Opening: BuildOpening(); break;
                case SLSceneKind.CharacterCreation: BuildCharacterCreation(); break;
                case SLSceneKind.Lobby: BuildLobby(); break;
                case SLSceneKind.AdventureWorld: BuildAdventure(); break;
                case SLSceneKind.Clash: BuildArena("CLASH / CS", 45f); break;
                case SLSceneKind.BattleRoyale: BuildArena("BATTLE ROYALE", 80f); break;
                case SLSceneKind.Siege: BuildSiege(); break;
                case SLSceneKind.SupremeRuin: BuildRuin(); break;
                case SLSceneKind.Rift: BuildRift(); break;
            }
            SpawnPlayer(new Vector3(0, .1f, -5));
            Camera.main.transform.SetParent(player, true);
            Camera.main.transform.localPosition = new Vector3(0, 4.5f, -8f);
            Camera.main.transform.localRotation = Quaternion.Euler(18, 0, 0);
            var follow = Camera.main.gameObject.AddComponent<SLThirdPersonCamera>();
            follow.target = player;
        }

        void MakeLight()
        {
            var go = new GameObject("Sun"); go.transform.SetParent(worldRoot); spawned.Add(go);
            var l = go.AddComponent<Light>(); l.type = LightType.Directional; l.intensity = 1.25f; l.shadows = LightShadows.Soft;
            go.transform.rotation = Quaternion.Euler(50, -35, 0);
            RenderSettings.ambientIntensity = 0.65f;
            RenderSettings.fog = true; RenderSettings.fogDensity = 0.006f;
        }
        void MakeGround()
        {
            CreatePrimitive("Ground", PrimitiveType.Plane, new Vector3(0,0,0), new Vector3(30,1,30), Mat("Grass", new Color(.12f,.32f,.16f)));
            if (sceneKind != SLSceneKind.Opening && sceneKind != SLSceneKind.CharacterCreation)
                CreatePrimitive("Path", PrimitiveType.Cube, new Vector3(0,.04f,0), new Vector3(4,.08f,45), Mat("Path", new Color(.28f,.22f,.14f)));
        }
        void BuildOpening()
        {
            RenderSettings.fogColor = new Color(.025f,.04f,.09f); RenderSettings.fogDensity=.018f;
            CreatePrimitive("CrystalSpire", PrimitiveType.Cylinder, new Vector3(0,6,8), new Vector3(3,12,3), Mat("Crystal", new Color(.15f,.65f,1f)));
            for(int i=0;i<12;i++) CreatePrimitive("Star"+i, PrimitiveType.Sphere, Random.insideUnitSphere*25f + Vector3.up*10, Vector3.one*.15f, Mat("Glow", new Color(.5f,.8f,1f)));
        }
        void BuildCharacterCreation()
        {
            CreatePrimitive("Platform", PrimitiveType.Cylinder, new Vector3(0,.25f,0), new Vector3(6,.5f,6), Mat("Platform", new Color(.18f,.18f,.24f)));
            CreatePrimitive("Crystal", PrimitiveType.Cylinder, new Vector3(0,3,5), new Vector3(2,6,2), Mat("Crystal", new Color(.15f,.65f,1f)));
        }
        void BuildLobby()
        {
            RenderSettings.fogColor = new Color(.08f,.12f,.18f);
            for(int i=0;i<10;i++) CreatePrimitive("Tower"+i, PrimitiveType.Cube, new Vector3(-18+i*4, 2, 14+Random.Range(-2,3)), new Vector3(2,4+Random.Range(0,5),2), Mat("Stone", new Color(.25f,.29f,.35f)));
            CreatePrimitive("LobbyCrystal", PrimitiveType.Cylinder, new Vector3(0,5,18), new Vector3(4,10,4), Mat("Crystal", new Color(.2f,.7f,1f)));
        }
        void BuildAdventure()
        {
            RenderSettings.fogColor = new Color(.18f,.42f,.48f);
            for(int i=0;i<35;i++)
            {
                float x=Random.Range(-25f,25f), z=Random.Range(2f,28f);
                CreatePrimitive("Tree", PrimitiveType.Cylinder, new Vector3(x,2,z), new Vector3(.8f,2.5f,.8f), Mat("Wood", new Color(.22f,.11f,.05f)));
                CreatePrimitive("Canopy", PrimitiveType.Sphere, new Vector3(x,5,z), new Vector3(2.5f,2.5f,2.5f), Mat("Leaf", new Color(.08f,.38f,.14f)));
            }
            CreatePrimitive("VillageHall", PrimitiveType.Cube, new Vector3(10,2,15), new Vector3(8,4,6), Mat("Stone", new Color(.35f,.36f,.4f)));
            CreatePrimitive("VillageRoof", PrimitiveType.Cube, new Vector3(10,4.5f,15), new Vector3(9,1,7), Mat("Roof", new Color(.28f,.08f,.06f)));
            for(int i=0;i<6;i++) SpawnNPC(new Vector3(Random.Range(-8,8),1,Random.Range(2,18)));
            for(int i=0;i<8;i++) SpawnMonster(new Vector3(Random.Range(-22,22),1,Random.Range(18,30)));
        }
        void BuildArena(string label, float size)
        {
            RenderSettings.fogDensity=.002f;
            CreatePrimitive("Arena", PrimitiveType.Cylinder, new Vector3(0,.15f,10), new Vector3(size/4,.3f,size/4), Mat("Arena", new Color(.16f,.16f,.2f)));
            for(int i=0;i<12;i++) CreatePrimitive("Cover", PrimitiveType.Cube, new Vector3(Random.Range(-size/3,size/3),1,Random.Range(-size/3,size/3)+10), new Vector3(3,2,3), Mat("Cover", new Color(.25f,.27f,.3f)));
            for(int i=0;i<10;i++) SpawnMonster(new Vector3(Random.Range(-15,15),1,Random.Range(5,25)));
        }
        void BuildSiege()
        {
            CreatePrimitive("FortWall", PrimitiveType.Cube, new Vector3(0,3,22), new Vector3(35,6,2), Mat("Stone", new Color(.3f,.3f,.34f)));
            CreatePrimitive("Gate", PrimitiveType.Cube, new Vector3(0,3,20.5f), new Vector3(7,6,3), Mat("Gate", new Color(.16f,.1f,.06f)));
            for(int i=0;i<8;i++) SpawnNPC(new Vector3(Random.Range(-15,15),1,Random.Range(0,18)));
            for(int i=0;i<16;i++) SpawnMonster(new Vector3(Random.Range(-18,18),1,Random.Range(15,30)));
        }
        void BuildRuin()
        {
            RenderSettings.fogColor = new Color(.08f,.05f,.12f); RenderSettings.fogDensity=.014f;
            CreatePrimitive("RuinCore", PrimitiveType.Cylinder, new Vector3(0,5,18), new Vector3(5,10,5), Mat("Ruin", new Color(.25f,.12f,.4f)));
            for(int i=0;i<5;i++) SpawnBoss(new Vector3(-10+i*5,1,20), i+1);
        }
        void BuildRift()
        {
            RenderSettings.fogColor = new Color(.18f,.015f,.03f); RenderSettings.fogDensity=.02f;
            CreatePrimitive("Rift", PrimitiveType.Cylinder, new Vector3(0,5,18), new Vector3(7,.3f,7), Mat("Rift", new Color(.65f,.03f,.15f)));
            for(int i=0;i<12;i++) SpawnMonster(new Vector3(Random.Range(-16,16),1,Random.Range(12,30)));
            SpawnBoss(new Vector3(0,1,24), 10);
        }
        void SpawnPlayer(Vector3 pos)
        {
            player = SLProceduralActorFactory.Create(worldRoot, "Player_RiggedProcedural", pos, SLProceduralActorFactory.ActorStyle.Player, 0).transform;
            player.gameObject.AddComponent<SLThirdPersonPlayer>();
        }
        void SpawnNPC(Vector3 pos) => SLProceduralActorFactory.Create(worldRoot, "VillageNPC", pos, SLProceduralActorFactory.ActorStyle.Villager, Random.Range(0,10));
        void SpawnMonster(Vector3 pos) => SLProceduralActorFactory.Create(worldRoot, "ElementalBeast", pos, SLProceduralActorFactory.ActorStyle.Beast, Random.Range(0,10));
        void SpawnBoss(Vector3 pos, int level) => SLProceduralActorFactory.Create(worldRoot, "DivineCreature_"+level, pos, SLProceduralActorFactory.ActorStyle.DivineBoss, level-1);
        GameObject CreatePrimitive(string n, PrimitiveType t, Vector3 p, Vector3 s, Material m)
        {
            var g=GameObject.CreatePrimitive(t); g.name=n; g.transform.SetParent(worldRoot); g.transform.position=p; g.transform.localScale=s; g.GetComponent<Renderer>().sharedMaterial=m; spawned.Add(g); return g;
        }
        Material Mat(string n, Color c)
        {
            if(mats.TryGetValue(n,out var m)) return m;
            m=new Material(Shader.Find("Standard")){name="SL_"+n}; m.color=c; mats[n]=m; return m;
        }
        void BuildHUD()
        {
            var canvas=new GameObject("SL_HUD").AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay; canvas.gameObject.AddComponent<CanvasScaler>(); canvas.gameObject.AddComponent<GraphicRaycaster>();
            var title=MakeText(canvas.transform, sceneDisplayName, new Vector2(0,1), new Vector2(30,-30), 26);
            title.alignment=TextAnchor.UpperLeft;
            MakeText(canvas.transform,"HP 1000   LV 1   XP 0/100",new Vector2(0,0),new Vector2(25,25),20);
            MakeText(canvas.transform,"QUEST  •  Explore the world",new Vector2(1,1),new Vector2(-30,-80),18).alignment=TextAnchor.UpperRight;
            if(sceneKind==SLSceneKind.Lobby)
            {
                MakeText(canvas.transform,"ADVENTURE WORLD   •   CLASH [LV10]   •   BATTLE ROYALE [LV10]   •   SIEGE [LV10]",new Vector2(.5f,0),new Vector2(0,125),18).alignment=TextAnchor.MiddleCenter;
                MakeButton(canvas.transform,"ADVENTURE",new Vector2(.5f,0),new Vector2(-250,65),()=>LoadSceneByName("03_AdventureWorld"));
                MakeButton(canvas.transform,"CS / CLASH",new Vector2(.5f,0),new Vector2(-85,65),()=>LoadSceneByName("04_Clash"));
                MakeButton(canvas.transform,"BATTLE ROYALE",new Vector2(.5f,0),new Vector2(85,65),()=>LoadSceneByName("05_BattleRoyale"));
                MakeButton(canvas.transform,"SIEGE",new Vector2(.5f,0),new Vector2(250,65),()=>LoadSceneByName("06_Siege"));
            }
            if(allowSceneNavigation && sceneKind!=SLSceneKind.Lobby) MakeButton(canvas.transform,"NEXT",new Vector2(1,0),new Vector2(-70,70),()=>NextScene());
        }
        Text MakeText(Transform parent,string text,Vector2 anchor,Vector2 pos,int size)
        {
            var go=new GameObject("Text"); go.transform.SetParent(parent,false); var rt=go.AddComponent<RectTransform>(); rt.anchorMin=rt.anchorMax=anchor; rt.anchoredPosition=pos; rt.sizeDelta=new Vector2(700,70); var t=go.AddComponent<Text>(); t.text=text;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=size;t.color=Color.white; return t;
        }
        void MakeButton(Transform parent,string label,Vector2 anchor,Vector2 pos,UnityEngine.Events.UnityAction action)
        {
            var go=new GameObject("Button");go.transform.SetParent(parent,false);var rt=go.AddComponent<RectTransform>();rt.anchorMin=rt.anchorMax=anchor;rt.anchoredPosition=pos;rt.sizeDelta=new Vector2(120,55);var img=go.AddComponent<Image>();img.color=new Color(.05f,.1f,.2f,.85f);var b=go.AddComponent<Button>();b.onClick.AddListener(action);var txt=MakeText(go.transform,label,new Vector2(.5f,.5f),Vector2.zero,16);txt.alignment=TextAnchor.MiddleCenter;
        }
        void LoadSceneByName(string sceneName) { if (Application.CanStreamedLevelBeLoaded(sceneName)) SceneManager.LoadScene(sceneName); else Debug.LogWarning("Scene not found in Build Settings: " + sceneName); }
        void NextScene()
        {
            int i=SceneManager.GetActiveScene().buildIndex; int next=(i+1)%SceneManager.sceneCountInBuildSettings; SceneManager.LoadScene(next);
        }
    }

    public sealed class SLThirdPersonPlayer : MonoBehaviour
    {
        public float speed=5f; public float gravity=-20f; CharacterController cc;
        void Awake(){cc=gameObject.AddComponent<CharacterController>(); cc.height=2f; cc.radius=.35f; cc.center=new Vector3(0,1f,0);}
        void Update(){Vector2 stick=SLVirtualJoystick.Movement; float x=Input.GetAxisRaw("Horizontal")+stick.x; float z=Input.GetAxisRaw("Vertical")+stick.y; Transform cam=Camera.main?Camera.main.transform:null; Vector3 forward=cam?cam.forward:Vector3.forward; Vector3 right=cam?cam.right:Vector3.right; forward.y=0; right.y=0; forward.Normalize(); right.Normalize(); Vector3 input=forward*z+right*x; if(input.sqrMagnitude>1)input.Normalize(); Vector3 move=input*speed; if(!cc.isGrounded)move.y+=gravity*Time.deltaTime; cc.Move(move*Time.deltaTime); var proceduralRig=GetComponent<SLProceduralActorRig>(); if(proceduralRig)proceduralRig.SetLocomotion(input.magnitude); if(input.sqrMagnitude>.01f)transform.forward=Vector3.Slerp(transform.forward,input.normalized,.2f);}
    }
    public sealed class SLThirdPersonCamera : MonoBehaviour
    {
        public Transform target; public float smooth=8f;
        void LateUpdate(){if(!target)return;Vector3 desired=target.position+new Vector3(0,4.5f,-8f);transform.position=Vector3.Lerp(transform.position,desired,1-Mathf.Exp(-smooth*Time.deltaTime));transform.LookAt(target.position+Vector3.up*1.2f);}
    }
    public sealed class SLSimpleEnemy : MonoBehaviour
    {
        public float moveSpeed = 1.8f; public float noticeRange = 16f; float t; Transform player;
        void Start() { var found = GameObject.Find("Player_RiggedProcedural"); if (found) player = found.transform; }
        void Update()
        {
            t += Time.deltaTime;
            if (player && Vector3.Distance(transform.position, player.position) < noticeRange)
            {
                Vector3 d = player.position - transform.position; d.y = 0;
                if (d.sqrMagnitude > 2.8f) { transform.position += d.normalized * moveSpeed * Time.deltaTime; if(d.sqrMagnitude>.01f) transform.forward=Vector3.Slerp(transform.forward,d.normalized,Time.deltaTime*3f); }
            }
            else transform.position += new Vector3(Mathf.Sin(t*.7f),0,Mathf.Cos(t*.6f))*.15f*Time.deltaTime;
        }
    }
}
