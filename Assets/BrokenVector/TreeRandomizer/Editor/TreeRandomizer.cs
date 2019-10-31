using UnityEngine;
using UnityEditor;

using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace BrokenVector.TreeRandomizer
{
    public static class TreeRandomizer
    {
        private static Tree GetAssetTree(Tree tree)
        {
            if (AssetDatabase.Contains(tree))
                return tree;

            GameObject go = tree.gameObject;
            GameObject asset = PrefabUtility.GetPrefabParent(go) as GameObject;
            if (asset == null)
                return null;

            return asset.GetComponent<Tree>();
        }

        public static void RandomizeTree(Tree template, int treeCount, bool cloneMaterials)
        {
            if (template == null)
                return;

            Debug.Log("Starting generation of " + treeCount + " trees.");

            template = GetAssetTree(template);

            if (!AssetDatabase.Contains(template))
            {
                Debug.LogError("The tree was not found in the AssetDatabase.", template);
                return;
            }

            string path = AssetDatabase.GetAssetPath(template);
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);

            string outputFolder = Path.Combine(dir, Constants.OUTPUT_FOLDER);
            if (!AssetDatabase.IsValidFolder(outputFolder))
                AssetDatabase.CreateFolder(dir, Constants.OUTPUT_FOLDER);

            var templateSerialized = new SerializedObject(template.data);

            Material[] materials = template.GetComponent<MeshRenderer>().sharedMaterials;
            Material barkmat = templateSerialized.FindProperty("optimizedSolidMaterial").objectReferenceValue as Material;
            if (barkmat == null)
            {
                Debug.LogError("bark material not found!");
                return;
            }
            Material leafmat = templateSerialized.FindProperty("optimizedCutoutMaterial").objectReferenceValue as Material;
            if (leafmat == null)
            {
                Debug.LogError("leaf material not found");
                return;
            }

            List<Tree> generatedTrees = new List<Tree>();
            for (int i = 0; i < treeCount; i++)
            {
                string outFile = name + "_" + i + ext;
                string outPath = Path.Combine(outputFolder, outFile);

                bool success = AssetDatabase.CopyAsset(path, outPath);
                AssetDatabase.Refresh();
                if (!success)
                {
                    Debug.LogError("Could not copy the tree from " + path + " to " + outPath);
                    return;
                }

                AssetDatabase.ImportAsset(outPath);
                Tree newTree = AssetDatabase.LoadAssetAtPath(outPath, typeof(Tree)) as Tree;

                SerializedObject newTreeSerialized = new SerializedObject(newTree.data);
                Material newTreeBark = newTreeSerialized.FindProperty("optimizedSolidMaterial").objectReferenceValue as Material;
                Material newTreeLeaf = newTreeSerialized.FindProperty("optimizedCutoutMaterial").objectReferenceValue as Material;

                if (!cloneMaterials)
                {
                    if (newTreeBark != null)
                        Object.DestroyImmediate(newTreeBark, true);
                    if (newTreeLeaf != null)
                        Object.DestroyImmediate(newTreeLeaf, true);

                    newTreeSerialized.FindProperty("optimizedSolidMaterial").objectReferenceValue = barkmat;
                    newTreeSerialized.FindProperty("optimizedCutoutMaterial").objectReferenceValue = leafmat;

                    newTree.GetComponent<MeshRenderer>().sharedMaterials = materials;

                    AssetDatabase.DeleteAsset(outputFolder + "/" + name + "_" + i + "_Textures");
                }

                AssetDatabase.SaveAssets();

                int randomSeed = Random.Range(0, 9999999);
                newTreeSerialized.FindProperty("root.seed").intValue = randomSeed;
                newTreeSerialized.ApplyModifiedProperties();
                MethodInfo meth = newTree.data.GetType().GetMethod("UpdateMesh", new[] { typeof(Matrix4x4), typeof(Material[]).MakeByRefType() });
                object[] arguments = new object[] { newTree.transform.worldToLocalMatrix, null };
                meth.Invoke(newTree.data, arguments);

                generatedTrees.Add(newTree);
            }
        }
    }
}