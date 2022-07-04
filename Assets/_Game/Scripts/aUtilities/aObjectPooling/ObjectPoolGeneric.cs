using System.Collections.Generic;

using UnityEngine;

public class ObjectPool<TPoolable> where TPoolable : IPoolable
{
    private Queue<TPoolable> _pool;

    private GameObject _prefab;
    private Transform _despawnParent;
    private int _extendAmount;

    /// <summary>
    /// The default strategy is initialCapasity = 3 and extendAmount = 3
    /// </summary>
    public ObjectPool(GameObject prefabArg, Transform despawnParentArg, int initialCapasity = 3, int extendAmountArg = 3)
    {
        _prefab = prefabArg;
        _despawnParent = despawnParentArg;
        _extendAmount = extendAmountArg;
        _pool = new Queue<TPoolable>(initialCapasity);

        for (int i = 0; i < initialCapasity; i++)
        {
            GameObject gb = UnityEngine.Object.Instantiate(_prefab);
            if (!gb.TryGetComponent<TPoolable>(out TPoolable poolable))
            if (poolable == null)
            {
                Debug.LogError("To use pool, a prefab should contain IPoolable");
            }

            _pool.Enqueue(poolable);
            gb.transform.SetParent(_despawnParent, false);
            gb.SetActive(false);
        }
    }

    public void OnDestroy()
    {
        _pool.Clear();
    }

    public TPoolable Spawn(Transform parentArg = null)
    { 
        if (_pool.Count == 0)
        {
            Extend();
        }

        TPoolable poolable = _pool.Dequeue();

        if (parentArg != null)
        {
            poolable.GetTransform().SetParent(parentArg, false);
        }

        return poolable;
    }

    public TPoolable Spawn(Vector3 pos, Transform parentArg = null)
    {
        TPoolable poolable = Spawn(parentArg);
        poolable.GetTransform().position = pos;
        return poolable;
    }

    public void Despawn(TPoolable poolable)
    {
        poolable.GetTransform().parent = _despawnParent;
        _pool.Enqueue(poolable);
    }

    private void Extend()
    {
        Debug.Log("Extending");
        for (int i = 0; i < _extendAmount; i++)
        {
            GameObject gb = UnityEngine.Object.Instantiate(_prefab);
            TPoolable poolable = gb.GetComponent<TPoolable>();
            _pool.Enqueue(poolable);
            gb.transform.SetParent(_despawnParent, false);
            gb.SetActive(false);
        }
    }
}
