using System.Collections.Generic;

using UnityEngine;

public class PoolsController : MonoBehaviour
{
    // [Header("Projectiles")]
    // [Space]
    // [SerializeField]
    // private CannonProjectile _projectilePrefab;

    // [SerializeField]
    // private int _projectilesPoolInitialCapacity = 100;

    // [SerializeField]
    // private Transform _despawnedProjectilesParent;

    // private ObjectPool<CannonProjectile> _projectilesPool;

    [Header("PuzzleItems")]
    [Space]
    [SerializeField]
    private List<GameObject> _puzzleItemPrefabs;

    [SerializeField]
    private Transform _depspawnedPuzzleItemsParent;

    private ObjectPoolIndexed<PuzzleItem> _puzzleItemsPool;

    private void Awake()
    {
        // _projectilesPool = new ObjectPool<CannonProjectile>(
        //     _projectilePrefab.gameObject, 
        //      _despawnedProjectilesParent,
        //     _projectilesPoolInitialCapacity,
        //     extendAmountArg: 50
        // );

        _puzzleItemsPool = new ObjectPoolIndexed<PuzzleItem>(
            _puzzleItemPrefabs, 
            _depspawnedPuzzleItemsParent
        );


        // PoolingDelegatesContainer.EventSpawnProjectile += SpawnProjectile;
        // PoolingDelegatesContainer.EventDespawnProjectile += DespawnProjectile;

        PoolingDelegatesContainer.FuncSpawnPuzzleItemIndexed += SpawnPuzzleItemIndexed;
        PoolingDelegatesContainer.EventDespawnPuzzleItemIndexed += DespawnPuzzleItemIndexed;
    }

    private void OnDestroy()
    {
        //_projectilesPool.OnDestroy();

        _puzzleItemsPool.OnDestroy();

        // PoolingDelegatesContainer.EventSpawnProjectile -= SpawnProjectile;
        // PoolingDelegatesContainer.EventDespawnProjectile -= DespawnProjectile;

        PoolingDelegatesContainer.FuncSpawnPuzzleItemIndexed -= SpawnPuzzleItemIndexed;
        PoolingDelegatesContainer.EventDespawnPuzzleItemIndexed -= DespawnPuzzleItemIndexed;
    }

    // private void SpawnProjectile(Vector3 pos, Vector3 dir, float lifeTimeMax)
    // {
    //     var projectile = _projectilesPool.Spawn(pos);
    //     projectile.OnSpawn(lifeTimeMax, dir);
    // }

    // private void DespawnProjectile(CannonProjectile projectile)
    // {
    //     projectile.OnDespawn();
    //     _projectilesPool.Despawn(projectile);
    // }

    private PuzzleItem SpawnPuzzleItemIndexed(int index, Vector3 pos)
    {
        PuzzleItem puzzleItem = _puzzleItemsPool.Spawn(index, pos);
        puzzleItem.OnSpawn();
        return puzzleItem;
    }

    private void DespawnPuzzleItemIndexed(int index, PuzzleItem puzzleItem)
    {
        _puzzleItemsPool.Despawn(index, puzzleItem);
    }
}