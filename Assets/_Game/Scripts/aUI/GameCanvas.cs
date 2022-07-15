using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class GameCanvas : MonoBehaviour
{ 
    private Canvas _canvas;
    private UIWindowCraft _uiCraftWindow;

    private void Awake()
    {
        TryGetComponent(out _canvas);
        if (!transform.GetChild(0).GetChild(0).TryGetComponent(out _uiCraftWindow))
        {
            Debug.LogError("Invenotry was not found!");
        }
    }

    private void OnDestroy()
    { 
    }
}