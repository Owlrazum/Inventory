using System.Collections;
using UnityEngine;

public class Item3DRepresentation : MonoBehaviour
{
    [SerializeField]
    private ItemSO _itemData;

    [SerializeField]
    private float _scaleDownTime;

    private Vector3 _initialScale;

    private Rigidbody _rigidBody;
    private Collider _collider;

    private void Awake()
    {
        _initialScale = transform.localScale;

        TryGetComponent(out _rigidBody);
        TryGetComponent(out _collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayersContainer.PLAYER_COLLISION_LAYER)
        {
            return;
        }

        if (!other.TryGetComponent(out InventoryHolder inventory))
        {
            Debug.LogError("Player does not have inventory");
            return;
        }

        inventory.AddItem(_itemData);
        _rigidBody.isKinematic = true;
        _collider.enabled = false;
        StartCoroutine(ScaleDown());
    }

    private IEnumerator ScaleDown()
    {
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / _scaleDownTime;
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.zero, lerpParam);
            yield return null;
        }

        Destroy(gameObject);
    }
}