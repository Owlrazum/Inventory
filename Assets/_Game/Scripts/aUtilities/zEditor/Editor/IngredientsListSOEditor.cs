using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemsListSO))]
public class IngredientsListSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemsListSO so = (ItemsListSO)target;
        if (GUILayout.Button("Assing IDs"))
        {
            so.AssignIDs();
            Debug.Log("Assigning");
        }
    }
}