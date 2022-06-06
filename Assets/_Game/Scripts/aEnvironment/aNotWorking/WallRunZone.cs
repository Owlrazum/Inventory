using UnityEngine;

/// <summary>
/// An assumption is made that the transform pivot and collider's center are relatively the same.
/// </summary>
public class WallRunZone : MonoBehaviour
{
    private void Awake()
    { 
#if UNITY_EDITOR
        if (gameObject.layer != LayersContainer.WALL_RUN_ZONE_LAYER)
        {
            Debug.LogError("Incorrect layer for the wall run zone.");
        }
#endif
    }
}
