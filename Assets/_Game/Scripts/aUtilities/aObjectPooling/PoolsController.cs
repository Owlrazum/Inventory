using System.Collections.Generic;

using UnityEngine;

public class PoolsController : MonoBehaviour
{
    [Header("Item Stacks UIStacks")]
    [Space]
    [SerializeField]
    private UIStack _uiStackPrefab;

    [SerializeField]
    private Transform _despawnedStacksParent;

    private ObjectPool<UIStack> _stacksPool;

    private void Awake()
    {
        _stacksPool = new ObjectPool<UIStack>(
            _uiStackPrefab.gameObject, 
            _despawnedStacksParent
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
        uiStack.gameObject.SetActive(true);
        return uiStack;
    }

    private void DespawnUIStack(UIStack uiStack)
    {
        uiStack.gameObject.SetActive(false);
        _stacksPool.Despawn(uiStack);
    }
}