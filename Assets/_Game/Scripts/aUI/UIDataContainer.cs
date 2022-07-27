using UnityEngine;

public class UIDataContainer : MonoBehaviour
{
    [SerializeField]
    private Vector2Int _referenceScreenResolution = new Vector2Int(1080, 1920);

    private void Awake()
    { 
        UIDelegatesContainer.GetReferenceScreenResolution += GetReferenceScreenResolution;
    }

    private void OnDestroy()
    { 
        UIDelegatesContainer.GetReferenceScreenResolution -= GetReferenceScreenResolution;
    }

    private Vector2Int GetReferenceScreenResolution()
    {
        return _referenceScreenResolution;
    }
}