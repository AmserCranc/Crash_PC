// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Unity.VisualScripting;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.UIElements;

// public class LevelExtractor : EditorWindow
// {
//     private GameObject targetGameObject;
//     private Texture2D textureData;

//     [MenuItem("Tools/Simple Editor Window")]
//     public static void ShowWindow()
//     {
//         GetWindow<LevelExtractor>("Simple Editor");
//     }

//     private void OnGUI()
//     {
//         GUILayout.Label("Simple Editor Window", EditorStyles.boldLabel);

//         EditorGUILayout.Space();

//         targetGameObject = (GameObject)EditorGUILayout.ObjectField(
//             "GameObject",
//             targetGameObject,
//             typeof(GameObject),
//             true);

//         EditorGUILayout.Space();

//         GUI.enabled = targetGameObject != null;

//         if (GUILayout.Button("Confirm", GUILayout.Height(30)))
//         {
//             Confirm();
//         }

//         GUI.enabled = true;
//     }

//     private void Confirm()
//     {
//         Debug.Log($"Confirmed: {targetGameObject.name}");

//         textureData = GLOBAL.DEBUG_atlas;
//         Mesh mesh = targetGameObject.GetComponent<MeshFilter>().mesh;

//         HashSet<propertySet> sets = new();
//         sets = ConstructDatabase(ref mesh);

//         Dictionary<page_colour, Texture2D> convertedTextures = new();
//         convertedTextures = ExtractTextures(textureData);
//     }


//     public struct propertySet
//     {
//         public ushort  CLUTx; 
//         public byte    CLUTy, texPage, blendMode, cMode, u, v;

//         public bool IsEqualTo(propertySet other)
//         {
//             return CLUTx     == other.CLUTx
//                 && CLUTy     == other.CLUTy
//                 && texPage   == other.texPage
//                 && blendMode == other.blendMode
//                 && cMode     == other.cMode
//                 && u         == other.u
//                 && v         == other.v;
//         }

//         public long GetHash()
//         {
//             return HashCode.Combine(CLUTx, CLUTy, texPage, blendMode, cMode, u, v);
//         }
//     }

//     public struct page_colour
//     {
//         public byte page, cMode;

//         public bool IsEqualTo(page_colour other)
//         {
//             return page  == other.page
//                 && cMode == other.cMode;
//         }
//     }

//     public HashSet<propertySet> ConstructDatabase(ref Mesh mesh)
//     {
//         HashSet<propertySet> sets = new();

//         int i = 0;
//         // Use lists for UV channels; GetUVs expects a List<T>
//         List<Vector4> meta0 = new List<Vector4>();
//         List<Vector4> meta1 = new List<Vector4>();
//         List<Vector2> uv0 = new List<Vector2>();

//         mesh.GetUVs(1, meta0);
//         mesh.GetUVs(2, meta1);
//         mesh.GetUVs(0, uv0);

//         int count = mesh.vertexCount;
//         for (i = 0; i < count; i++)
//         {
//             propertySet ps = new propertySet();

//             if (i < meta1.Count) ps.CLUTx       = (ushort)meta1[i].x;
//             if (i < meta1.Count) ps.CLUTy       =   (byte)meta1[i].y;
//             if (i < meta1.Count) ps.texPage     =   (byte)meta1[i].z;
//             if (i < meta1.Count) ps.blendMode   =   (byte)meta1[i].w;
  
//             if (i < meta0.Count) ps.cMode       =   (byte)meta0[i].x;
  
//             if (i < uv0.Count)   ps.u           =   (byte)uv0[i].x;
//             if (i < uv0.Count)   ps.v           =   (byte)uv0[i].y;

//             sets.Add(ps);
//         }


//         return sets;
//     }
//     public Dictionary<page_colour, Texture2D> ExtractTextures(Texture2D atlas, HashSet<propertySet> accesses)
//     {
//         Dictionary<page_colour, Texture2D> outTextures = new();

// //Made a new texture for each BPP/page pair
//         foreach(propertySet s in accesses)
//         {
//             page_colour textureID = new page_colour{page = s.texPage, cMode = s.cMode};
//             if(!outTextures.ContainsKey(textureID))
//                 outTextures.Add(textureID, new Texture2D(
//                     width: s.cMode == 0 ? 512 
//                             : s.cMode == 1 ? 256
//                                 : 128,
                    
//                     height: 128    
//                 ));            
//         }
// //Pair new UVs with old
//         Dictionary<Vector2, Vector2> oldNewUVs = new();
//         foreach(propertySet s in accesses)
//         {
//             if(s.cMode == 0)
//             {
//                 new
//             }
//         }



//         return outTextures;
//     }


// }