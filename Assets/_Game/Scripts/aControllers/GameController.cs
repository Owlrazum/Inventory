using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GameDesciptionSO _gameDescription;

    private int _currentLevel = 1;

    private void Awake()
    {
        EventsContainer.EventGameStart += OnGameStart;
        
        DontDestroyOnLoad(gameObject);
    }

    private void OnGameStart()
    {
        EventsContainer.ShouldLoadNextScene?.Invoke();
        EventsContainer.ShouldPrepareLevel?.Invoke(_gameDescription.GameSequence[_currentLevel]);
    }
}