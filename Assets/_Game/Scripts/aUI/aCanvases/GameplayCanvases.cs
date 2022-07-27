using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class GameplayCanvases : MonoBehaviour
{
    [SerializeField]
    private float _fadeSpeed = 2;

    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        TryGetComponent(out _canvasGroup);

        CraftingDelegatesContainer.EventRecipeEvaluationCompleted += OnRecipeEvaluationCompleted;
        GameDelegatesContainer.EventNoRecipeWasCrafted += OnNoRecipeWasCrafted;
        _canvasGroup.alpha = 0;
        StartCoroutine(FadeIn());
    }

    private void OnDestroy()
    { 
        CraftingDelegatesContainer.EventRecipeEvaluationCompleted -= OnRecipeEvaluationCompleted;
        GameDelegatesContainer.EventNoRecipeWasCrafted -= OnNoRecipeWasCrafted;
    }

    private void OnRecipeEvaluationCompleted(RecipeQualityType notUsed)
    {
        StartCoroutine(FadeOut());
    }

    private void OnNoRecipeWasCrafted()
    {
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeOut()
    {
        float value = 1;
        while (value > 0)
        {
            value -= _fadeSpeed * Time.deltaTime;   
            _canvasGroup.alpha = value;
            yield return null;
        }

        _canvasGroup.alpha = 0;
    }

    private IEnumerator FadeIn()
    {
        float value = 0;
        while (value < 1)
        {
            value += _fadeSpeed * Time.deltaTime;   
            _canvasGroup.alpha = value;
            yield return null;
        }

        _canvasGroup.alpha = 1;
    }
}