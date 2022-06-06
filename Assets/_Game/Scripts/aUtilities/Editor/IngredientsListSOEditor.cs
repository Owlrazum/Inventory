using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IngredientsListSO))]
public class IngredientsListSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        IngredientsListSO so = (IngredientsListSO)target;
        if (GUILayout.Button("Assing IDs"))
        {
            so.AssignIDs();
            Debug.Log("Assigning");
        }
    }
}