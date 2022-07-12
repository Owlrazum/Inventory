using UnityEngine;
using UnityEngine.UI;

public class UICraft : MonoBehaviour
{
    [SerializeField]
    private ItemsListSO _items;

    [SerializeField]
    private Image _craftButtonImage;

    private UIButton _craftButton;
    private UIButton _selectButton;


    private void Awake()
    {
        transform.GetChild(0).TryGetComponent(out _craftButton);
        transform.GetChild(1).TryGetComponent(out _selectButton);

        _selectButton.EventOnTouch += OnItemSelect;
        // _craftButton.interactable = true;
    }

    private void OnDestroy()
    {
        _craftButton.EventOnTouch -= OnCraftApproved;
        _selectButton.EventOnTouch -= OnItemSelect;
    }

    private void OnItemSelect()
    { 
        _craftButton.EventOnTouch += OnCraftApproved;
        // _craftButton.interactable = true;
    }

    private void OnCraftApproved()
    { 
        
    }
}