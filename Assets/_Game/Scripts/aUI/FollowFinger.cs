using UnityEngine;
using UnityEngine.EventSystems;

public class FollowFinger : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }
}