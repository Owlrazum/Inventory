using UnityEngine;

public class PoolsController : MonoBehaviour
{
    [SerializeField]
    private UIStack _uiStackPrefab;

    private ObjectPool<UIStack> _stacksPool;

    private void Awake()
    {
        _stacksPool = new ObjectPool<UIStack>(
            _uiStackPrefab.gameObject, 
            transform,
            10
        );

        PoolingDelegatesContainer.FuncSpawnUIStack += SpawnUIStack;
        PoolingDelegatesContainer.EventDespawnUIStack += DespawnUIStack;
    }

    private void OnDestroy()
    {
        _stacksPool.OnDestroy();

        PoolingDelegatesContainer.FuncSpawnUIStack -= SpawnUIStack;
        PoolingDelegatesContainer.EventDespawnUIStack -= DespawnUIStack;
    }

    private UIStack SpawnUIStack()
    {
        UIStack uiStack = _stacksPool.Spawn();
        return uiStack;
    }

    private void DespawnUIStack(UIStack uiStack)
    {
        _stacksPool.Despawn(uiStack);
    }
}