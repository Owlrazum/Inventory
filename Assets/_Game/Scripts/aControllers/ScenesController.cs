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
        _currentSceneIndex = 0;

        EventsContainer.ShouldLoadNextScene += LoadNextScene;
        EventsContainer.ShouldStartLoadingNextScene += StartLoadingNextScene;

        UIEventsContainer.EventExitToMainMenuPressed += LoadMainMenuScene;
        UIEventsContainer.EventContinueButtonPressed += FinishLoadingScene;

        UIQueriesContainer.FuncSceneLoadingProgress += GetSceneLoadingProgress;
    }

    private void OnDestroy()
    { 
        EventsContainer.ShouldLoadNextScene -= LoadNextScene;
        EventsContainer.ShouldStartLoadingNextScene -= StartLoadingNextScene;

        UIEventsContainer.EventExitToMainMenuPressed -= LoadMainMenuScene;
        UIEventsContainer.EventContinueButtonPressed -= FinishLoadingScene;

        UIQueriesContainer.FuncSceneLoadingProgress -= GetSceneLoadingProgress;
    }

    private void LoadNextScene()
    {
        _currentSceneIndex++;
        SceneManager.LoadScene(_currentSceneIndex);
    }

    private void StartLoadingNextScene()
    { 
        _loadingScene = SceneManager.LoadSceneAsync(++_currentSceneIndex);
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