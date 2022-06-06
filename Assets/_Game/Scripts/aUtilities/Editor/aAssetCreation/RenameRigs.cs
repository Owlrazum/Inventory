using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RenameRigs : MonoBehaviour
{
    [SerializeField]
    private Transform root;

    [SerializeField]
    private string toReplace;

    [SerializeField]
    private string withReplace;

    private void Start()
    {
        RecursiveReplace(root);
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/aGame/Prefabs/ReplacedRigs.prefab");
        AssetDatabase.SaveAssets();
    }

    private void RecursiveReplace(Transform root)
    {
        root.name = root.name.Replace(toReplace, withReplace);
        for (int i = 0; i < root.childCount; i++)
        {
            RecursiveReplace(root.GetChild(i));
        }
    }
}
