#if UNITY_EDITOR
using UnityEditor;using UnityEngine;
namespace GameSystems.MonsterHumanGeneration.Editor{
public sealed class MonsterHumanGeneratorWindow:EditorWindow{
 GenerationProfile profile;
 [MenuItem("Tools/Game Systems/Monster & Human Generation")]
 public static void Open(){GetWindow<MonsterHumanGeneratorWindow>("Monster/Human Generator");}
 void OnGUI(){EditorGUILayout.LabelField("System 01 - Monster & Human Generation",EditorStyles.boldLabel);
 profile=(GenerationProfile)EditorGUILayout.ObjectField("Generation Profile",profile,typeof(GenerationProfile),false);
 if(GUILayout.Button("Create New Profile"))CreateProfile();
 using(new EditorGUI.DisabledScope(profile==null)){if(GUILayout.Button("Validate"))Validate();if(GUILayout.Button("Generate Assets",GUILayout.Height(34)))Generate();}
 EditorGUILayout.HelpBox("Local deterministic generation. External AI/model providers connect through explicit provider interfaces.",MessageType.Info);}
 void CreateProfile(){var p=ScriptableObject.CreateInstance<GenerationProfile>();p.EnsureId();var path=EditorUtility.SaveFilePanelInProject("Save Generation Profile","GenerationProfile","asset","");if(!string.IsNullOrEmpty(path)){AssetDatabase.CreateAsset(p,path);AssetDatabase.SaveAssets();profile=p;Selection.activeObject=p;}}
 void Validate(){if(profile==null)return;bool ok=!string.IsNullOrWhiteSpace(profile.entityId)&&!string.IsNullOrWhiteSpace(profile.displayName)&&profile.height>0&&profile.bodyWidth>0;if(ok)EditorUtility.DisplayDialog("Validation","Profile passed basic validation.","OK");else EditorUtility.DisplayDialog("Validation","Entity ID, name, height and body width are required.","OK");}
 void Generate(){if(profile==null)return;profile.EnsureId();string safe=string.IsNullOrWhiteSpace(profile.displayName)?"GeneratedEntity":profile.displayName.Replace(" ","_");string folder="Assets/GeneratedMonsterHuman/"+safe;EnsureFolder(folder);
 Mesh mesh=ProceduralGeneration.Mesh(profile);AssetDatabase.CreateAsset(mesh,AssetDatabase.GenerateUniqueAssetPath(folder+"/"+safe+"_Mesh.asset"));
 Texture2D tex=profile.createTexture?ProceduralGeneration.Texture(profile,256):null;if(tex)AssetDatabase.CreateAsset(tex,AssetDatabase.GenerateUniqueAssetPath(folder+"/"+safe+"_Texture.asset"));
 Material mat=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));mat.name=safe+"_Material";mat.color=profile.baseColor;if(tex)mat.mainTexture=tex;AssetDatabase.CreateAsset(mat,AssetDatabase.GenerateUniqueAssetPath(folder+"/"+safe+"_Material.mat"));
 GameObject go=ProceduralGeneration.Build(profile,mesh,mat);PrefabUtility.SaveAsPrefabAsset(go,folder+"/"+safe+".prefab");DestroyImmediate(go);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
 EditorUtility.DisplayDialog("Generation Complete","Assets generated in:\n"+folder,"OK");}
 static void EnsureFolder(string path){string[] p=path.Split('/');string c=p[0];for(int i=1;i<p.Length;i++){string n=c+"/"+p[i];if(!AssetDatabase.IsValidFolder(n))AssetDatabase.CreateFolder(c,p[i]);c=n;}}
}
}
#endif