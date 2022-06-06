using System.Collections;

using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UILoadingProgress : MonoBehaviour
{
    private Slider _loadingSlider;

    private void Awake()
    {
        TryGetComponent(out _loadingSlider);
        EventsContainer.EventStartedLoadingNextScene += OnStartedLoadingNextScene;
    }

    private void OnDestroy()
    {
        EventsContainer.EventStartedLoadingNextScene -= OnStartedLoadingNextScene;
    }

    private void OnStartedLoadingNextScene()
    {
        StartCoroutine(LoadingProgressUpdateLoop());
    }

    private IEnumerator LoadingProgressUpdateLoop()
    {
        float _saveTimer = 0;
        while (_saveTimer < 100000)
        {
            _saveTimer += Time.deltaTime;
            float progress = UIQueriesContainer.QuerySceneLoadingProgress();
            progress += Time.deltaTime / 5;
            _loadingSlider.normalizedValue = progress;
            yield return null;
        }
    }
}