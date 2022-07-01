using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemSO _itemData;

    [SerializeField]
    private int _amount;

    [SerializeField]
    private float _scaleDownTime = 0.5f;

    private Vector3 _initialScale;

    private Rigidbody _rigidBody;
    private Collider _collider;
    private Collider _triggerCollider;

    private void Awake()
    {
        _initialScale = transform.localScale;

        TryGetComponent(out _rigidBody);
        TryGetComponent(out _collider);
        transform.GetChild(0).TryGetComponent(out _triggerCollider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayersContainer.PLAYER_COLLISION_LAYER)
        {
            return;
        }

        if (!other.TryGetComponent(out PlayerInventoryHolder inventory))
        {
            Debug.LogError("Player does not have inventory");
            return;
        }

        if (!CraftingDelegatesContainer.FuncNewItemsPlacementIfPossible(_itemData, _amount))
        {
            return;
        }

        _rigidBody.isKinematic = true;
        _collider.enabled = false;
        _triggerCollider.enabled = false;
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