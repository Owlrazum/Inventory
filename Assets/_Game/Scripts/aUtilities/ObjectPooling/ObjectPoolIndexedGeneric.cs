using System.Collections.Generic;

using UnityEngine;

public class ObjectPoolIndexed<TPoolable> where TPoolable : IPoolable
{
    private List<Queue<TPoolable>> _pools;
    private List<GameObject> _prefabs;

    private Transform _despawnParent;
    private int _extendAmount;

    /// <summary>
    /// The default strategy is initialCapasity = 3 and extendAmount = 3
    /// </summary>
    public ObjectPoolIndexed(List<GameObject> prefabsArg, Transform despawnParentArg, int initialCapasity = 3, int extendAmountArg = 3)
    {
        _prefabs = prefabsArg;
        _despawnParent = despawnParentArg;
        _extendAmount = extendAmountArg;

        _pools = new List<Queue<TPoolable>>();
        for (int i = 0; i < _prefabs.Count; i++)
        { 
            Queue<TPoolable> pool = new Queue<TPoolable>(initialCapasity);
            for (int j = 0; j < initialCapasity; j++)
            {
                GameObject gb = UnityEngine.Object.Instantiate(_prefabs[i]);
                if (!gb.TryGetComponent<TPoolable>(out TPoolable poolable))
                if (poolable == null)
                {
                    Debug.LogError("To use pool, a prefab should contain IPoolable");
                }

                pool.Enqueue(poolable);
                gb.transform.parent = _despawnParent;
                gb.SetActive(false);
            }
            _pools.Add(pool);
        }
    }

    public void OnDestroy()
    {
        foreach (var pool in _pools)
        {
            pool.Clear();
        }
        _pools.Clear();
    }

    public TPoolable Spawn(int index, Vector3 pos, Transform parentArg = null)
    {
        if (_pools[index].Count == 0)
        {
            Extend(index);
        }

        TPoolable poolable = _pools[index].Dequeue();

        poolable.GetTransform().position = pos;
        if (parentArg != null)
        {
            poolable.GetTransform().SetParent(parentArg, false);
        }

        return poolable;
    }

    public void Despawn(int index, TPoolable poolable)
    {
        poolable.GetTransform().parent = _despawnParent;
        _pools[index].Enqueue(poolable);
    }

    private void Extend(int index)
    {
        Debug.Log("Extending indexed at " + index);
        for (int i = 0; i < _extendAmount; i++)
        {
            GameObject gb = UnityEngine.Object.Instantiate(_prefabs[index]);
            TPoolable poolable = gb.GetComponent<TPoolable>();
            _pools[index].Enqueue(poolable);
            gb.transform.parent = _despawnParent;
            gb.SetActive(false);
        }
    }
}
