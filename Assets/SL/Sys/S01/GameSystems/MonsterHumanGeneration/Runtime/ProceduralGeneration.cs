using System.Collections.Generic;
using UnityEngine;

namespace GameSystems.MonsterHumanGeneration
{
    /// <summary>High-detail deterministic mesh/profile generator used by the System 01 editor workflow.</summary>
    public static class ProceduralGeneration
    {
        struct Shape { public Vector3 center, scale; public Color ignored; public Shape(Vector3 c,Vector3 s){center=c;scale=s;ignored=Color.white;} }

        public static Mesh Mesh(GenerationProfile p)
        {
            if(p==null) p=ScriptableObject.CreateInstance<GenerationProfile>();
            int lon=Mathf.Clamp(p.radialSegments*2,16,64),lat=Mathf.Clamp(p.verticalSegments*3,12,48);
            float h=Mathf.Max(1f,p.height),w=Mathf.Max(.25f,p.bodyWidth),d=Mathf.Max(.2f,p.bodyDepth);
            var shapes=new List<Shape>();
            if(p.bodyType==GeneratedBodyType.Humanoid)
            {
                shapes.Add(new Shape(new Vector3(0,h*.51f,0),new Vector3(w*.78f,h*.22f,d*.78f)));// torso
                shapes.Add(new Shape(new Vector3(0,h*.31f,0),new Vector3(w*.63f,h*.12f,d*.68f)));// abdomen
                shapes.Add(new Shape(new Vector3(0,h*.22f,0),new Vector3(w*.72f,h*.10f,d*.75f)));// pelvis
                shapes.Add(new Shape(new Vector3(0,h*.79f,0),new Vector3(w*.43f,h*.105f,d*.48f)));// head
                shapes.Add(new Shape(new Vector3(0,h*.68f,0),new Vector3(w*.24f,h*.055f,d*.25f)));// neck
                for(int sign=-1;sign<=1;sign+=2)
                {
                    shapes.Add(new Shape(new Vector3(sign*w*.68f,h*.52f,0),new Vector3(w*.22f,h*.10f,d*.25f)));// shoulder
                    shapes.Add(new Shape(new Vector3(sign*w*.78f,h*.39f,0),new Vector3(w*.13f,h*.14f,d*.16f)));// forearm
                    shapes.Add(new Shape(new Vector3(sign*w*.27f,h*.13f,0),new Vector3(w*.22f,h*.19f,d*.24f)));// thigh
                    shapes.Add(new Shape(new Vector3(sign*w*.27f,h*.045f,d*.03f),new Vector3(w*.15f,h*.13f,d*.18f)));// shin
                    shapes.Add(new Shape(new Vector3(sign*w*.27f,h*.012f,d*.12f),new Vector3(w*.17f,h*.045f,d*.3f)));// boot
                }
                // Layered garment and silhouette accessories, included as real mesh surfaces.
                shapes.Add(new Shape(new Vector3(0,h*.49f,d*.035f),new Vector3(w*.84f,h*.20f,d*.84f)));// coat
                shapes.Add(new Shape(new Vector3(0,h*.31f,d*.02f),new Vector3(w*.75f,h*.055f,d*.79f)));// belt
                shapes.Add(new Shape(new Vector3(0,h*.865f,-d*.04f),new Vector3(w*.45f,h*.055f,d*.47f)));// hair
                shapes.Add(new Shape(new Vector3(-w*.52f,h*.55f,0),new Vector3(w*.22f,h*.07f,d*.3f)));
                shapes.Add(new Shape(new Vector3(w*.52f,h*.55f,0),new Vector3(w*.22f,h*.07f,d*.3f)));
                shapes.Add(new Shape(new Vector3(0,h*.79f,d*.43f),new Vector3(w*.1f,h*.045f,d*.035f)));// face plate/sigil
            }
            else
            {
                shapes.Add(new Shape(new Vector3(0,h*.48f,0),new Vector3(w,h*.28f,d)));
                shapes.Add(new Shape(new Vector3(0,h*.75f,0),new Vector3(w*.48f,h*.17f,d*.5f)));
                for(int i=0;i<Mathf.Clamp(p.legCount,2,8);i++){float x=(i%2==0?-1:1)*w*.45f;float z=(i/2)*d*.2f;shapes.Add(new Shape(new Vector3(x,h*.22f,z),new Vector3(w*.18f,h*.24f,d*.18f)));}
                if(p.creatureLayout==CreatureBodyLayout.WingedBiped){shapes.Add(new Shape(new Vector3(-w,h*.58f,0),new Vector3(w*.65f,h*.16f,d*.08f)));shapes.Add(new Shape(new Vector3(w,h*.58f,0),new Vector3(w*.65f,h*.16f,d*.08f)));}
                if(p.tailLength>0)shapes.Add(new Shape(new Vector3(0,h*.35f,-p.tailLength*.35f),new Vector3(w*.18f,p.tailLength*.2f,p.tailLength*.65f)));
            }
            var vertices=new List<Vector3>();var normals=new List<Vector3>();var uv=new List<Vector2>();var triangles=new List<int>();
            foreach(var shape in shapes) AppendEllipsoid(shape.center,shape.scale,lon,lat,vertices,normals,uv,triangles);
            var mesh=new Mesh{name=(string.IsNullOrWhiteSpace(p.displayName)?"GeneratedCharacter":p.displayName)+"_DetailedProceduralMesh"};
            if(vertices.Count>65000)mesh.indexFormat=UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.SetVertices(vertices);mesh.SetNormals(normals);mesh.SetUVs(0,uv);mesh.SetTriangles(triangles,0);mesh.RecalculateBounds();return mesh;
        }

        static void AppendEllipsoid(Vector3 center,Vector3 scale,int lon,int lat,List<Vector3> vertices,List<Vector3> normals,List<Vector2> uv,List<int> triangles)
        {
            int start=vertices.Count;
            for(int y=0;y<=lat;y++)
            {
                float v=y/(float)lat,phi=v*Mathf.PI;
                for(int x=0;x<=lon;x++)
                {
                    float u=x/(float)lon,theta=u*Mathf.PI*2f;
                    Vector3 unit=new Vector3(Mathf.Sin(phi)*Mathf.Cos(theta),Mathf.Cos(phi),Mathf.Sin(phi)*Mathf.Sin(theta));
                    vertices.Add(center+Vector3.Scale(unit,scale));
                    Vector3 n=new Vector3(unit.x/Mathf.Max(.001f,scale.x),unit.y/Mathf.Max(.001f,scale.y),unit.z/Mathf.Max(.001f,scale.z)).normalized;normals.Add(n);uv.Add(new Vector2(u,v));
                }
            }
            for(int y=0;y<lat;y++)for(int x=0;x<lon;x++){int a=start+y*(lon+1)+x,b=a+lon+1;triangles.Add(a);triangles.Add(a+1);triangles.Add(b);triangles.Add(a+1);triangles.Add(b+1);triangles.Add(b);}
        }

        public static Texture2D Texture(GenerationProfile p,int size=512)
        {
            size=Mathf.Clamp(size,64,2048);var tex=new Texture2D(size,size,TextureFormat.RGBA32,true){name=(p.displayName??"Generated")+"_ProceduralTexture"};Color[] px=new Color[size*size];
            string d=(p.textureDescription??"").ToLowerInvariant();bool stripes=d.Contains("stripe"),flakes=d.Contains("flake")||d.Contains("scale"),glow=d.Contains("glow"),cloth=d.Contains("cloth")||d.Contains("fabric");
            for(int y=0;y<size;y++)for(int x=0;x<size;x++)
            {
                float nx=x/(float)size,ny=y/(float)size;float noise=Mathf.PerlinNoise(nx*42f+3.1f,ny*42f+7.7f);Color c=p.baseColor;
                c=Color.Lerp(c,p.secondaryColor,(noise-.45f)*.24f);
                if(stripes&&Mathf.Sin(ny*36f+nx*4f)>.35f)c=Color.Lerp(c,p.secondaryColor,.68f);
                if(flakes&&noise>.78f)c=Color.Lerp(c,p.emissionColor==Color.black?Color.white:p.emissionColor,.72f);
                if(cloth)c*=.92f+.08f*Mathf.Sin(nx*size*1.3f);
                if(glow)c=Color.Lerp(c,p.emissionColor==Color.black?Color.white:p.emissionColor,.12f);
                px[y*size+x]=c;
            }
            tex.SetPixels(px);tex.Apply(true,true);return tex;
        }

        public static GameObject Build(GenerationProfile p,Mesh mesh,Material mat)
        {
            if(p==null)throw new System.ArgumentNullException(nameof(p));p.EnsureId();
            // Use the same clothed, animated actor factory as Build Game, so editor-generated prefabs
            // and in-game generated actors share one implementation instead of diverging.
            var style=p.bodyType==GeneratedBodyType.Humanoid?SoulboundLegends.Runtime.SLProceduralActorFactory.ActorStyle.Player:SoulboundLegends.Runtime.SLProceduralActorFactory.ActorStyle.Beast;
            var root=SoulboundLegends.Runtime.SLProceduralActorFactory.Create(null,p.displayName,Vector3.zero,style,Mathf.Abs(p.entityId.GetHashCode())%10);
            root.AddComponent<GeneratedEntityIdentity>().Set(p.entityId);root.AddComponent<GenerationRuntimeData>().Initialize(p);
            root.name=p.displayName;return root;
        }
    }

    public sealed class GeneratedEntityIdentity:MonoBehaviour
    {
        [SerializeField]string id;public string Id=>id;public void Set(string v){id=v??"";}
    }
    public sealed class GenerationRuntimeData:MonoBehaviour
    {
        [SerializeField]string id,nameText,source;public void Initialize(GenerationProfile p){id=p.entityId;nameText=p.displayName;source=p.description;}
    }
}
