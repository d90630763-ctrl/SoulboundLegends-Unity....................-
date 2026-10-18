using System.Collections.Generic;
using UnityEngine;

namespace SoulboundLegends.Lobby
{
    /// <summary>
    /// Builds a procedural 3D fantasy lobby composition inspired by the supplied reference image.
    /// It does not use the reference image as the runtime background; it uses it only as a design reference.
    /// Replace generated pieces with your own prefabs through EnvironmentProfile lists when available.
    /// </summary>
    public class SL_EnvironmentGenerator : MonoBehaviour
    {
        public Texture2D referenceImage;
        public SL_EnvironmentProfile profile = new SL_EnvironmentProfile();
        public Transform generatedRoot;
        public Camera lobbyCamera;
        public bool clearBeforeGenerate = true;
        public bool generateOnStart = true;
        public int randomSeed = 20261004;

        private readonly List<GameObject> spawned = new List<GameObject>();

        private void Start() { if (generateOnStart) Generate(); }

        public void Generate()
        {
            if (clearBeforeGenerate) Clear();
            if (generatedRoot == null)
            {
                var root = new GameObject("Generated Lobby Environment");
                generatedRoot = root.transform;
                generatedRoot.SetParent(transform, false);
            }
            Random.InitState(randomSeed);
            RenderSettings.fog = true;
            RenderSettings.fogColor = profile.atmosphere;
            RenderSettings.fogDensity = profile.fogDensity;
            CreateSky();
            CreateMainPlatform();
            if (profile.generateProceduralEnvironment)
            {
                CreateMountains();
                CreateFloatingIslands();
                CreateDistantKingdom();
                CreateWaterAndFalls();
                CreateClouds();
                CreateVegetation();
            }
            ApplyCustomLists();
            CreateLighting();
            if (lobbyCamera != null) ConfigureCamera(lobbyCamera);
        }

        public void Clear()
        {
            for (int i = spawned.Count - 1; i >= 0; i--)
                if (spawned[i] != null) DestroyImmediateSafe(spawned[i]);
            spawned.Clear();
        }

        private void DestroyImmediateSafe(GameObject go)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(go); else Destroy(go);
#else
            Destroy(go);
#endif
        }

        private GameObject SpawnPrimitive(PrimitiveType type, string name, Vector3 position, Vector3 scale, Material material = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(generatedRoot, false);
            go.transform.localPosition = position;
            go.transform.localScale = scale;
            if (material != null)
            {
                var renderer = go.GetComponent<Renderer>();
                if (renderer != null) renderer.sharedMaterial = material;
            }
            spawned.Add(go);
            return go;
        }

        private Material Mat(Color color, float metallic = 0f, float smoothness = 0.25f)
        {
            var shader = Shader.Find("Standard") ?? Shader.Find("Universal Render Pipeline/Lit");
            var m = new Material(shader);
            if (m.HasProperty("_Color")) m.color = color;
            if (m.HasProperty("_Metallic")) m.SetFloat("_Metallic", metallic);
            if (m.HasProperty("_Glossiness")) m.SetFloat("_Glossiness", smoothness);
            if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", smoothness);
            return m;
        }

        private void CreateSky()
        {
            var shader = Shader.Find("Skybox/Procedural");
            if (shader != null)
            {
                var sky = new Material(shader);
                if (sky.HasProperty("_SkyTint")) sky.SetColor("_SkyTint", profile.skyTop);
                if (sky.HasProperty("_GroundColor")) sky.SetColor("_GroundColor", profile.skyBottom);
                if (sky.HasProperty("_Exposure")) sky.SetFloat("_Exposure", 1.15f);
                RenderSettings.skybox = sky;
            }
        }

        private void CreateMainPlatform()
        {
            var stone = Mat(new Color(0.65f, 0.72f, 0.82f), 0.15f, 0.55f);
            var gold = Mat(new Color(0.95f, 0.65f, 0.16f), 0.7f, 0.75f);
            SpawnPrimitive(PrimitiveType.Cylinder, "Hero Display Platform", new Vector3(0, 0.15f, 0), new Vector3(5.5f, 0.25f, 5.5f), stone);
            SpawnPrimitive(PrimitiveType.Cylinder, "Hero Gold Ring", new Vector3(0, 0.43f, 0), new Vector3(4.6f, 0.06f, 4.6f), gold);
        }

        private void CreateMountains()
        {
            var mountain = Mat(new Color(0.30f, 0.52f, 0.64f), 0f, 0.2f);
            for (int i = 0; i < 10; i++)
            {
                float x = -35f + i * 7.5f;
                float z = 18f + Random.Range(-3f, 6f);
                float h = Random.Range(8f, 18f);
                SpawnPrimitive(PrimitiveType.Cylinder, "Distant Mountain", new Vector3(x, h * 0.5f - 1f, z), new Vector3(h * 0.65f, h, h * 0.65f), mountain);
            }
        }

        private void CreateFloatingIslands()
        {
            var rock = Mat(new Color(0.32f, 0.43f, 0.48f), 0f, 0.2f);
            var grass = Mat(new Color(0.24f, 0.58f, 0.35f), 0f, 0.35f);
            for (int i = 0; i < 9; i++)
            {
                float angle = i * 40f * Mathf.Deg2Rad;
                float radius = 14f + (i % 3) * 5f;
                float x = Mathf.Cos(angle) * radius;
                float z = 9f + Mathf.Sin(angle) * 12f;
                float y = 7f + (i % 4) * 2.3f;
                var island = SpawnPrimitive(PrimitiveType.Cylinder, "Floating Island", new Vector3(x, y, z), new Vector3(2.6f, 0.45f, 2.1f), rock);
                SpawnPrimitive(PrimitiveType.Cylinder, "Floating Island Grass", new Vector3(x, y + 0.55f, z), new Vector3(2.35f, 0.12f, 1.85f), grass);
                island.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), Random.Range(-8f, 8f));
            }
        }

        private void CreateDistantKingdom()
        {
            var wall = Mat(new Color(0.77f, 0.83f, 0.93f), 0.05f, 0.65f);
            var roof = Mat(new Color(0.20f, 0.34f, 0.58f), 0.1f, 0.55f);
            float z = 28f;
            SpawnPrimitive(PrimitiveType.Cube, "Distant Kingdom Base", new Vector3(0, 5.0f, z), new Vector3(15f, 3.5f, 3f), wall);
            for (int i = -3; i <= 3; i++)
            {
                float x = i * 4f;
                var tower = SpawnPrimitive(PrimitiveType.Cylinder, "Kingdom Tower", new Vector3(x, 9f + Mathf.Abs(i) * 0.5f, z - 0.2f), new Vector3(1.1f, 4.8f, 1.1f), wall);
                SpawnPrimitive(PrimitiveType.Cylinder, "Kingdom Roof", new Vector3(x, 14.1f + Mathf.Abs(i) * 0.5f, z - 0.2f), new Vector3(1.55f, 2f, 1.55f), roof);
                tower.transform.rotation = Quaternion.identity;
            }
            SpawnPrimitive(PrimitiveType.Cube, "Kingdom Palace", new Vector3(0, 11f, z - 1.2f), new Vector3(7f, 9f, 2.5f), wall);
            SpawnPrimitive(PrimitiveType.Cylinder, "Palace Roof", new Vector3(0, 17f, z - 1.2f), new Vector3(5.5f, 4f, 5.5f), roof);
        }

        private void CreateWaterAndFalls()
        {
            var water = Mat(new Color(0.15f, 0.65f, 0.95f, 0.78f), 0.05f, 0.85f);
            for (int i = -2; i <= 2; i++)
            {
                float x = i * 5.5f;
                SpawnPrimitive(PrimitiveType.Cube, "Waterfall", new Vector3(x, 6f, 21f), new Vector3(1.1f, 5.5f, 0.15f), water);
                SpawnPrimitive(PrimitiveType.Cylinder, "Water Pool", new Vector3(x, 0.65f, 12f), new Vector3(2.3f, 0.05f, 1.1f), water);
            }
            SpawnPrimitive(PrimitiveType.Cube, "Main Water Channel", new Vector3(0, 0.25f, 11f), new Vector3(13f, 0.08f, 3.0f), water);
        }

        private void CreateClouds()
        {
            var cloud = Mat(new Color(0.95f, 0.98f, 1f), 0f, 0.05f);
            for (int i = 0; i < 12; i++)
            {
                float x = Random.Range(-30f, 30f);
                float y = Random.Range(12f, 25f);
                float z = Random.Range(4f, 35f);
                SpawnPrimitive(PrimitiveType.Sphere, "Cloud", new Vector3(x, y, z), new Vector3(2.5f, 0.8f, 1.4f), cloud);
            }
        }

        private void CreateVegetation()
        {
            var trunk = Mat(new Color(0.30f, 0.20f, 0.12f), 0f, 0.2f);
            var leaves = Mat(new Color(0.18f, 0.55f, 0.30f), 0f, 0.3f);
            for (int i = 0; i < 16; i++)
            {
                float x = Random.Range(-13f, 13f);
                float z = Random.Range(3f, 13f);
                if (Mathf.Abs(x) < 5f) x += x < 0 ? -4f : 4f;
                SpawnPrimitive(PrimitiveType.Cylinder, "Fantasy Tree Trunk", new Vector3(x, 1.7f, z), new Vector3(0.18f, 1.7f, 0.18f), trunk);
                SpawnPrimitive(PrimitiveType.Sphere, "Fantasy Tree Crown", new Vector3(x, 3.8f, z), new Vector3(1.2f, 1.7f, 1.2f), leaves);
            }
        }

        private void ApplyCustomLists()
        {
            ApplyList(profile.sky); ApplyList(profile.clouds); ApplyList(profile.floatingIslands); ApplyList(profile.kingdoms);
            ApplyList(profile.castles); ApplyList(profile.buildings); ApplyList(profile.waterfalls); ApplyList(profile.water);
            ApplyList(profile.mountains); ApplyList(profile.vegetation); ApplyList(profile.platforms); ApplyList(profile.bridges);
            ApplyList(profile.decorations); ApplyList(profile.particles); ApplyList(profile.specialEffects);
        }

        private void ApplyList(List<SL_EnvironmentElement> list)
        {
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null || e.prefab == null || !e.active) continue;
                var go = Instantiate(e.prefab, e.position, Quaternion.Euler(e.rotation), generatedRoot);
                go.name = e.name;
                go.transform.localScale = e.scale;
                spawned.Add(go);
                if (e.animate) go.AddComponent<SL_EnvironmentSpinner>().Configure(e.animationAxis, e.animationSpeed);
            }
        }

        private void CreateLighting()
        {
            var go = new GameObject("Fantasy Sun");
            go.transform.SetParent(generatedRoot, false);
            go.transform.rotation = Quaternion.Euler(35f, -35f, 0f);
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            light.color = new Color(1f, 0.91f, 0.76f);
            spawned.Add(go);
        }

        private void ConfigureCamera(Camera cam)
        {
            cam.fieldOfView = 55f;
            cam.transform.position = new Vector3(0f, 4.5f, -11f);
            cam.transform.rotation = Quaternion.Euler(12f, 0f, 0f);
        }
    }

    public class SL_EnvironmentSpinner : MonoBehaviour
    {
        private Vector3 axis = Vector3.up;
        private float speed = 1f;
        public void Configure(Vector3 a, float s) { axis = a; speed = s; }
        private void Update() { transform.Rotate(axis.normalized, speed * Time.deltaTime, Space.Self); }
    }
}
