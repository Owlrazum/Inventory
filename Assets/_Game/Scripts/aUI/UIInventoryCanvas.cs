using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIInventoryCanvas : MonoBehaviour
{ 
    private Canvas _canvas;
    private UIInventory _inventory;

    private void Awake()
    {
        TryGetComponent(out _canvas);
        if (!transform.GetChild(0).GetChild(0).TryGetComponent(out _inventory))
        {
            Debug.LogError("Invenotry was not found!");
        }

        _canvas.enabled = false;

    }

    private void OnDestroy()
    { 
    }
}