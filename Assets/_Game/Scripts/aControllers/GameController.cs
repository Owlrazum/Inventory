using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameDesciptionSO _gameDescription;

    [SerializeField]
    private ItemsListSO _itemList;

    private int _currentLevel = 0;

    private void Awake()
    {
        EventsContainer.EventGameStart += OnGameStart;
        CraftingDelegatesContainer.FuncGetItemSO += GetItemSO;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    { 
        EventsContainer.EventGameStart -= OnGameStart;
        CraftingDelegatesContainer.FuncGetItemSO -= GetItemSO;
    }

    private void OnGameStart()
    {
        EventsContainer.ShouldLoadNextScene?.Invoke();
        StartCoroutine(OnLevelLoad());
    }

    private IEnumerator OnLevelLoad()
    {
        yield return null;
        EventsContainer.ShouldPrepareLevel?.Invoke(_gameDescription.GameSequence[_currentLevel]);
        _currentLevel++;
    }

    private ItemSO GetItemSO(int id)
    {
        return _itemList.Items[id];
    }
}