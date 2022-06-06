using UnityEngine;
using UnityEditor;

public class RemoveUnnecessarySegmentableColliders : MonoBehaviour
{
    public void Remove()
    {
        if (transform.name != "SegmentablesParent")
        {
            Debug.LogError("Remover should reside in segmentablesParent");
            return;
        }
        
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out BoxCollider2D boxCollider))
            {
                Undo.DestroyObjectImmediate(boxCollider);
            }
        }
    }
}