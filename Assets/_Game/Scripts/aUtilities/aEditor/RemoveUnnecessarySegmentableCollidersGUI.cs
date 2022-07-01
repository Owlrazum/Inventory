using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RemoveUnnecessarySegmentableColliders))]
public class RemoveUnnecessarySegmentableCollidersGUI : Editor
{ 
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RemoveUnnecessarySegmentableColliders remover = (RemoveUnnecessarySegmentableColliders) target;
        if(GUILayout.Button("Remove Colliders"))
        {
            remover.Remove();
        }
    }
}