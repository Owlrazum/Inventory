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

        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;
    }

    private void OnDestroy()
    { 
        InputDelegatesContainer.EventInventoryCommandTriggered -= OnInventoryCommandTriggered;
    }


    #region InvenotoryDisplay
    // Places items based on topLeftCorner
    private void OnInventoryCommandTriggered()
    {
        if (_canvas.enabled)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    private void HideInventory()
    {
        _canvas.enabled = false;
        _inventory.OnInventoryClose();
    }

    private void ShowInventory()
    { 
        _canvas.enabled = true;
        _inventory.OnInventoryShow();
    }
    #endregion
}