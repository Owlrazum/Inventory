using UnityEngine;
using UnityEngine.Pool;

public class PoolsController : MonoBehaviour
{
    [SerializeField]
    private UIStack _prefab;

    private ObjectPool<UIStack> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<UIStack>(
            Create,
            null,
            null,
            null,
            false,
            10,
            10000
        );

        PoolingDelegatesContainer.SpawnStack += Spawn;
        PoolingDelegatesContainer.DespawnStack += Despawn;
    }

    private void OnDestroy()
    {
        PoolingDelegatesContainer.SpawnStack -= Spawn;
        PoolingDelegatesContainer.DespawnStack -= Despawn;
    }

    private UIStack Spawn()
    {
        return _pool.Get();
    }

    private void Despawn(UIStack bs)
    {
        bs.gameObject.SetActive(false);
        _pool.Release(bs);
    }
    
    private UIStack Create()
    {
        return Instantiate(_prefab);
    }
}