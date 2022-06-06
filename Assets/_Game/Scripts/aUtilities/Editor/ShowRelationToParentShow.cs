using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (ShowRelationToParent))]
public class ShowRelationToParentShow : Editor
{
    private void OnSceneGUI()
    {
        ShowRelationToParent movingPlatform = (ShowRelationToParent)target;
        Transform t = movingPlatform.transform;

        Handles.color = Color.red;
        Handles.DrawLine(t.position, t.parent.position, 2);
    }
}