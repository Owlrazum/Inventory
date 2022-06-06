using System.Collections;
using UnityEngine;

public class PuzzleItemSpawner : MonoBehaviour
{
    [SerializeField]
    private int _itemIndexToSpawnFromPoolsController;

    [SerializeField]
    private Transform _spawnTransform;

    [SerializeField]
    private float _timeDelayBeforeNewSpawn = 0.5f;

    [SerializeField]
    private bool _isAffectedByLevelSegmentation;

    [SerializeField]
    private MeshRenderer _toDisableInRuntimeRenderer;

    private PuzzleItem _puzzleItem;

    private void Awake()
    {
        if (_toDisableInRuntimeRenderer != null)
        { 
            _toDisableInRuntimeRenderer.enabled = false;
        }
    }

    private void Start()
    { 
        if (!_isAffectedByLevelSegmentation)
        {
            Spawn();
        }
    }

    public void OnLevelSegmentEnable()
    {
        if (_isAffectedByLevelSegmentation)
        { 
            Spawn();
        }
    }

    public void OnLevelSegmentDisable()
    {
        OnDespawn();
    }

    private void Spawn()
    {
        _puzzleItem = PoolingDelegatesContainer.SpawnPuzzleItemIndexedAndQueryIt(
            _itemIndexToSpawnFromPoolsController, _spawnTransform.position);
        _puzzleItem.AssignDespawnCallBack(OnDespawn);
    }

    private void OnDespawn()
    {
        PoolingDelegatesContainer.EventDespawnPuzzleItemIndexed.Invoke(
            _itemIndexToSpawnFromPoolsController, _puzzleItem);
            
        if (_timeDelayBeforeNewSpawn > 0)
        { 
            StartCoroutine(DelayBeforeNewSpawn());
        }
        else
        {
            Spawn();
        }
    }

    private IEnumerator DelayBeforeNewSpawn()
    {
        yield return new WaitForSeconds(_timeDelayBeforeNewSpawn);
        Spawn();
    }
}