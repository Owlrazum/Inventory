using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlayerTriggerEventsBroadcaster : MonoBehaviour
{
    private Collider _collider;

    private void OnTriggerEnter(Collider other)
    {
        PlayerEventsContainer.EventPlayerOnTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerEventsContainer.EventPlayerOnTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerEventsContainer.EventPlayerOnTriggerExit?.Invoke(other);
    }
}