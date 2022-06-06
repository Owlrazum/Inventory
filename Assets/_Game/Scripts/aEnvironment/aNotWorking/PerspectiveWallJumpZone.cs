using UnityEngine;

public class PerspectiveWallJumpZone : MonoBehaviour
{
    private void Awake()
    { 
#if UNITY_EDITOR
        if (gameObject.layer != LayersContainer.PERSPECTIVE_WALL_JUMP_ZONE_LAYER)
        {
            Debug.LogError("Incorrect layer for the wall run zone.");
        }
#endif
    }
}