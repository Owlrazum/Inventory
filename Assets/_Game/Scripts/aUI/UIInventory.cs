using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class UIInventory : MonoBehaviour
{
    private void Awake()
    {
        InputDelegatesContainer.EventInventoryCommandTriggered += OnInventoryCommandTriggered;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    { 
        InputDelegatesContainer.EventInventoryCommandTriggered -= OnInventoryCommandTriggered;
    }

    private void OnInventoryCommandTriggered()
    {
        gameObject.SetActive(true);
        
    }
}