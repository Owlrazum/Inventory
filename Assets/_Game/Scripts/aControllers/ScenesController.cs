using UnityEngine;
using UnityEngine.SceneManagement;

using SNG.UI;

public class ScenesController : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private int _sceneIndexToTest;
#endif

    private int _currentSceneIndex;
    private AsyncOperation _loadingScene;

    private void Awake()
    {
        EventsContainer.ShouldStartLoadingNextScene += StartLoadingNextScene;

        UIEventsContainer.EventExitToMainMenuPressed += LoadMainMenuScene;
        UIEventsContainer.EventContinueButtonPressed += FinishLoadingScene;

        UIQueriesContainer.FuncSceneLoadingProgress += GetSceneLoadingProgress;
        StartLoadingSavedScene();
    }

    private void OnDestroy()
    { 
        EventsContainer.ShouldStartLoadingNextScene -= StartLoadingNextScene;

        UIEventsContainer.EventExitToMainMenuPressed -= LoadMainMenuScene;
        UIEventsContainer.EventContinueButtonPressed -= FinishLoadingScene;

        UIQueriesContainer.FuncSceneLoadingProgress -= GetSceneLoadingProgress;
    }

    private void StartLoadingNextScene()
    { 
        _loadingScene = SceneManager.LoadSceneAsync(++_currentSceneIndex);
        EventsContainer.EventStartedLoadingNextScene?.Invoke();
    }

    private void StartLoadingSavedScene()
    { 
#if UNITY_EDITOR
        _loadingScene = SceneManager.LoadSceneAsync(_sceneIndexToTest);
        EventsContainer.EventStartedLoadingNextScene?.Invoke();
        return;
#endif
        int _currentSceneIndex = PlayerPrefs.GetInt(PlayerPrefsContainer.LAST_SCENE_INDEX, -1);
        if (_currentSceneIndex < 0)
        {
            Debug.LogError("Incorrect player pref");
        }

        _loadingScene = SceneManager.LoadSceneAsync(_currentSceneIndex);
        EventsContainer.EventStartedLoadingNextScene?.Invoke();
    }

    private float GetSceneLoadingProgress()
    {
        return _loadingScene.progress;
    }

    private void FinishLoadingScene()
    { 
        _loadingScene.allowSceneActivation = true;
    }

    private void LoadMainMenuScene()
    {
        _currentSceneIndex = 0;
        SceneManager.LoadScene(_currentSceneIndex);
    }
}