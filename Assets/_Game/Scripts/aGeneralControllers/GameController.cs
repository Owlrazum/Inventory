using UnityEngine;

public enum GameStateType
{
    MainMenu,
    Crafting,
    DrumRoll,
    LevelComplete
}

public class GameController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private int _sceneIndexToTest = -1;
#endif

    [SerializeField]
    private GameDesciptionSO _gameDesc;

    [SerializeField]
    private ItemsListSO _itemList;

    private GameStateType _gameState;
    private RecipeQualityType _evaluatedRecipeQuality;

    private int _currentLevel = 0;

    private void Awake()
    {
        GameDelegatesContainer.GetGameState += GetGameState;
        CraftingDelegatesContainer.GetItemSO += GetItemSO;
        CraftingDelegatesContainer.GetTargetItem += GetTargetItem;

        InputDelegatesContainer.StartGameCommand += OnStartGameCommand;
        InputDelegatesContainer.ExitToMainMenuCommand += OnExitToMainMenuCommand;

        CraftingDelegatesContainer.EventRecipeEvaluationCompleted += OnRecipeEvaluationCompleted;
        GameDelegatesContainer.EventDrumRollCompleted += OnDrumRollCompleted;

        InputDelegatesContainer.RetryLevelCommand += OnRetryLevelCommand;
        InputDelegatesContainer.NextLevelCommand += OnNextLevelCommand;
        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        GameDelegatesContainer.GetGameState -= GetGameState;
        CraftingDelegatesContainer.GetItemSO -= GetItemSO;
        CraftingDelegatesContainer.GetTargetItem -= GetTargetItem;

        InputDelegatesContainer.StartGameCommand -= OnStartGameCommand;
        InputDelegatesContainer.ExitToMainMenuCommand -= OnExitToMainMenuCommand;

        CraftingDelegatesContainer.EventRecipeEvaluationCompleted += OnRecipeEvaluationCompleted;
        GameDelegatesContainer.EventDrumRollCompleted -= OnDrumRollCompleted;

        InputDelegatesContainer.RetryLevelCommand -= OnRetryLevelCommand;
        InputDelegatesContainer.NextLevelCommand -= OnNextLevelCommand;
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (_sceneIndexToTest >= 0)
        {
            _currentLevel = _sceneIndexToTest;
            ApplicationDelegatesContainer.StartLoadingScene(_sceneIndexToTest);
        }
#endif
        ApplicationDelegatesContainer.StartLoadingScene(1);
    }

    private GameStateType GetGameState()
    {
        return _gameState;
    }

    private ItemSO GetItemSO(int itemID)
    {
        return _itemList.Data[itemID];
    }

    private ItemSO GetTargetItem()
    {
        return _gameDesc.Levels[_currentLevel].TargetItem;
    }

    private void OnStartGameCommand()
    {
        // ApplicationDelegatesContainer.EventBeforeFinishingLoadingScene();
        ApplicationDelegatesContainer.FinishLoadingScene(OnStartGameLoadingSceneFinished);
    }

    private void OnStartGameLoadingSceneFinished()
    {
        _gameState = GameStateType.Crafting;
        GameDelegatesContainer.StartLevel(_gameDesc.Levels[_currentLevel]);
        GameDelegatesContainer.EventLevelStarted();
    }

    private void OnExitToMainMenuCommand()
    {
        _gameState = GameStateType.MainMenu;
        ApplicationDelegatesContainer.LoadMainMenu(null);
    }

    private void OnRecipeEvaluationCompleted(RecipeQualityType recipeQuality)
    {
        _gameState = GameStateType.DrumRoll;
        _evaluatedRecipeQuality = recipeQuality;
        GameDelegatesContainer.StartDrumRoll();
    }

    private void OnDrumRollCompleted()
    {
        switch (_evaluatedRecipeQuality)
        { 
            case RecipeQualityType.NoRecipe:
                _gameState = GameStateType.Crafting;
                GameDelegatesContainer.ShowNoRecipeMessage();
                break;
            case RecipeQualityType.Perfect:
            case RecipeQualityType.Good:
            case RecipeQualityType.Bad:
                ApplicationDelegatesContainer.StartLoadingScene(1);
                _gameState = GameStateType.LevelComplete;
                GameDelegatesContainer.EventRecipeWasCrafted?.Invoke(_evaluatedRecipeQuality);
                break;
        }
    }

    private void OnRetryLevelCommand()
    {
        ApplicationDelegatesContainer.FinishLoadingScene(OnStartGameLoadingSceneFinished);
    }

    private void OnNextLevelCommand()
    {
        _currentLevel++;
        ApplicationDelegatesContainer.FinishLoadingScene(OnStartGameLoadingSceneFinished);
    }
}