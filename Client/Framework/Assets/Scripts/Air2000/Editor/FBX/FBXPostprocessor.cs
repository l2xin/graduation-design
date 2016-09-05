using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using Air2000;

public class FBXPostprocessor : AssetPostprocessor
{
    // This method is called just before importing an FBX.
    void OnPreprocessModel()
    {
        //return;
        Helper.Log("OnPreprocessModel " + assetPath);

        ModelImporter mi = (ModelImporter)assetImporter;
        //mi.globalScale = 1;
        bool tempExistCharacter = assetPath.Contains("anim/character/");
        bool tempExistMonster = assetPath.Contains("anim/monster/");

        if (tempExistCharacter || tempExistMonster)
        {
            mi.isReadable = true;
            if (tempExistCharacter)
            {
                mi.meshCompression = ModelImporterMeshCompression.Medium;
                mi.animationCompression = ModelImporterAnimationCompression.KeyframeReductionAndCompression;
            }
        }
        else
        {
            mi.isReadable = false;
        }
        mi.optimizeMesh = true;
        //mi.meshCompression = ModelImporterMeshCompression.Off;
        mi.importBlendShapes = false;
        mi.swapUVChannels = false;
        mi.normalImportMode = ModelImporterTangentSpaceMode.Import;
        mi.tangentImportMode = ModelImporterTangentSpaceMode.None;

        if (assetPath.Contains("/model/"))// model
        {
            if (assetPath.Contains("/npc/"))
            {
                string x = assetPath.Replace(".fbx", ".prefab");
                UnityEngine.Object tempPrefab = PrefabUtility.CreateEmptyPrefab(x);
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (asset == null)
                {
                    return;
                }
                GameObject sourceObj = asset as GameObject;
                //GameObject targetObj = 
                PrefabUtility.ReplacePrefab(sourceObj, tempPrefab)/* as GameObject*/;

            }
            if (assetPath.Contains("/obstacles/") == true || assetPath.Contains("/sceneEffect/") == true || assetPath.Contains("/npc/"))
            {
                mi.animationType = ModelImporterAnimationType.Legacy;
                mi.animationWrapMode = WrapMode.Loop;
            }
            else if (assetPath.Contains("/ZCCJ/Models/zhujue.fbx"))
            {

            }
            else
            {
                mi.animationType = ModelImporterAnimationType.None;

                if (assetPath.EndsWith("_colliders.fbx"))
                {
                    mi.importMaterials = false;
                    //mi.addCollider = true;
                }

            }
        }

        //clips txt
        TextAsset clipsAsset = (TextAsset)AssetDatabase.LoadAssetAtPath(assetPath.Substring(0, assetPath.LastIndexOf('.')) + "_clips.txt", typeof(TextAsset));
        if (clipsAsset != null)
        {
            List<ModelImporterClipAnimation> anims = new List<ModelImporterClipAnimation>();

            string[] clipsText = clipsAsset.text.Replace("\r\n", "\n").Split('\n');
            foreach (string c in clipsText)
            {
                string[] vs = c.Split(' ');
                if (vs.Length >= 3)
                {
                    string name = vs[0];
                    string begin = vs[1];
                    string end = vs[2];

                    ModelImporterClipAnimation clip = new ModelImporterClipAnimation();
                    clip.name = name;
                    clip.firstFrame = System.Convert.ToInt32(begin);
                    clip.lastFrame = System.Convert.ToInt32(end);
                    if (name == "idle" || name == "walk" || name == "run")
                    {
                        clip.loop = true;
                        clip.loopPose = true;
                    }

                    anims.Add(clip);
                }
            }

            mi.clipAnimations = anims.ToArray();
        }

        if (assetPath.Contains("/anim/") || assetPath.Contains("model/scene"))// anim
        {
            mi.animationType = ModelImporterAnimationType.Legacy;

            if (!assetPath.Contains("/anim/scene object/"))//当前文件夹不强制循环模式;
            {
                mi.animationWrapMode = WrapMode.Loop;
            }
            if (assetPath.Contains("@"))
            {
                mi.importMaterials = false;
            }
            if (assetPath.Contains("/character/") || assetPath.Contains("/machine/") ||
                assetPath.Contains("/monster/") || assetPath.Contains("/obstacles/") ||
                assetPath.Contains("/mount/") || assetPath.Contains("/pet/") || assetPath.Contains("/npc/")
                || assetPath.Contains("model/scene"))
            {
                if (assetPath.Contains("@"))
                {
                    return;
                }

                string x;
                if (assetPath.Contains(".fbx"))
                {
                    x = assetPath.Replace(".fbx", ".prefab");
                }
                else
                {
                    x = assetPath.Replace(".FBX", ".prefab");
                }

                UnityEngine.Object tempPrefab = PrefabUtility.CreateEmptyPrefab(x);
                UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (asset == null)
                {
                    return;
                }

                GameObject sourceObj = asset as GameObject;
                sourceObj = GameObject.Instantiate(sourceObj);

                GameObject targetObj = PrefabUtility.ReplacePrefab(sourceObj, tempPrefab, ReplacePrefabOptions.ReplaceNameBased) as GameObject;
                if (assetPath.Contains("/fashion/"))
                {
                    string texturePath = assetPath.Replace("/fashion/", "/fashion/texture/");
                    texturePath = texturePath.Replace(".fbx", ".jpg");
                    UnityEngine.Object textureObj = AssetDatabase.LoadMainAssetAtPath(texturePath);
                    if (textureObj != null)
                    {
                        Texture texture = textureObj as Texture;
                        SkinnedMeshRenderer[] render = targetObj.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                        if (render.Length != 0)
                        {
                            //Helper.LogError("render is not null!!!!!!!!");

                            render[0].sharedMaterial.mainTexture = texture;
                        }

                    }
                }
                else
                {
                    TextAsset animMoveInfo = (TextAsset)AssetDatabase.LoadAssetAtPath(assetPath.Substring(0, assetPath.LastIndexOf('.')) + "_animDis.txt", typeof(TextAsset));
                    if (animMoveInfo != null)
                    {
                        Dictionary<string, List<Vector4>> tmpData = null;
                        getAnimOffsetData(animMoveInfo.text, out tmpData);
                        //                        foreach (KeyValuePair<string, List<Vector4>> da in tmpData)
                        //                        {
                        //                           MotionMoveSaver moveSaver = targetObj.AddComponent<MotionMoveSaver>();
                        //                           moveSaver.setupData(da.Key, da.Value);
                        //                        }
                    }
                }

                GameObject.DestroyImmediate(sourceObj);
            }

            return;
        }
    }
    void getAnimOffsetData(string text, out Dictionary<string, List<Vector4>> tmpData)
    {
        tmpData = new Dictionary<string, List<Vector4>>();
        string[] clipsText = text.Replace("\r\n", "\n").Split('\n');
        List<Vector4> data = new List<Vector4>();
        foreach (string c in clipsText)
        {
            string[] vs = c.Split(',');
            if (vs.Length == 4)
            {
                Vector4 vec = Vector4.zero;
                for (int i = 0; i != vs.Length; ++i)
                {
                    float f = Convert.ToSingle(vs[i]);
                    vec[i] = f;
                }
                data.Add(vec);
            }
            else if (vs.Length == 1)
            {
                if (string.IsNullOrEmpty(vs[0]) == true)
                {
                    continue;
                }
                data = new List<Vector4>();
                tmpData.Add(vs[0], data);
            }
        }

    }

    // This method is called immediately after importing an FBX.
    void OnPostprocessModel(GameObject go)
    {
        Helper.Log("OnPostprocessModel " + assetPath);

        if (assetPath.Contains("/anim/"))
        {
            if (assetPath.Contains("@"))
            {
                // Remove SkinnedMeshRenderers and their meshes.
                foreach (SkinnedMeshRenderer smr in go.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    UnityEngine.Object.DestroyImmediate(smr.sharedMesh, true);
                    UnityEngine.Object.DestroyImmediate(smr.gameObject);
                }

                // Remove the bones.
                foreach (Transform o in go.transform)
                {
                    UnityEngine.Object.DestroyImmediate(o.gameObject);
                }
            }

            return;
        }

        //set ground material and texture
        if (assetPath.Contains("/model/"))
        {
            MeshRenderer[] meshes = go.GetComponentsInChildren<MeshRenderer>();

            if (assetPath.EndsWith("_colliders.fbx"))
            {
                foreach (MeshRenderer mesh in meshes)
                {
                    MeshFilter filter = mesh.gameObject.GetComponent<MeshFilter>();
                    if (filter != null)
                    {
                        GameObject.DestroyImmediate(filter);
                    }

                    GameObject.DestroyImmediate(mesh);
                }
            }
        }
    }

    public Material OnAssignMaterialModel(Material mat, Renderer render)
    {
        if (mat.shader.name.Contains("Particles"))
        {
            mat.shader = Shader.Find("Mobile/Particles/Additive");
        }
        return mat;
    }
}