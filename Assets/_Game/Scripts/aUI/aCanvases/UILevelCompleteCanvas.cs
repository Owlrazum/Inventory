using System.Collections;
using UnityEngine;


public class UILevelCompleteCanvas : UIBaseFadingCanvas
{
    [SerializeField]
    [Help("Requirements:\n child 0 - 3 canvas group; \nchild 1 - retryButton: 0, nextButton: 1")]
    private float _hiddenAlphaValue = 0.3f;

    [SerializeField]
    private float _shownAlphaValue = 1;

    [SerializeField]
    private float _showStarDeltaTime;

    private CanvasGroup _firstStar;
    private CanvasGroup _secondStar;
    private CanvasGroup _thirdStar;

    private CanvasGroup _buttonsCanvasGroup;
    private UIButton _retryButton;
    private UIButton _nextLevelButton;

    private RecipeQualityType _recipeQuality;

    protected override void Awake()
    {
        base.Awake();

        _canvasGroup.alpha = 0;

        bool isProperlyInitialized = true;
        isProperlyInitialized &= transform.GetChild(0).GetChild(0).GetChild(0).TryGetComponent(out _firstStar);
        isProperlyInitialized &= transform.GetChild(0).GetChild(0).GetChild(1).TryGetComponent(out _secondStar);
        isProperlyInitialized &= transform.GetChild(0).GetChild(0).GetChild(2).TryGetComponent(out _thirdStar);

        isProperlyInitialized &= transform.GetChild(0).GetChild(1).TryGetComponent(out _buttonsCanvasGroup);
        isProperlyInitialized &= transform.GetChild(0).GetChild(1).GetChild(0).TryGetComponent(out _retryButton);
        isProperlyInitialized &= transform.GetChild(0).GetChild(1).GetChild(1).TryGetComponent(out _nextLevelButton);

        _firstStar.alpha = _hiddenAlphaValue;
        _secondStar.alpha = _hiddenAlphaValue;
        _thirdStar.alpha = _hiddenAlphaValue;

        _buttonsCanvasGroup.alpha = 0;
        _retryButton.DeactivatePointerEvents();
        _nextLevelButton.DeactivatePointerEvents();

        gameObject.SetActive(false);

        if (!isProperlyInitialized)
        {
            Debug.LogError("Not initialized!!!");
        }

        GameDelegatesContainer.EventRecipeWasCrafted += OnRecipeWasCrafted;

        ActionCompletedShowItself += OnCompletedShowItself;
        _retryButton.EventOnTouch += OnRetryButtonPressed;
        _nextLevelButton.EventOnTouch += OnNextLevelButtonPressed;
    }

    private void OnDestroy()
    { 
        GameDelegatesContainer.EventRecipeWasCrafted -= OnRecipeWasCrafted;

        ActionCompletedShowItself -= OnCompletedShowItself;
        _retryButton.EventOnTouch -= OnRetryButtonPressed;
        _nextLevelButton.EventOnTouch -= OnNextLevelButtonPressed;
    }

    private void OnRecipeWasCrafted(RecipeQualityType recipeQualityArg)
    {
        _recipeQuality = recipeQualityArg;
        gameObject.SetActive(true);
        ShowItself();
    }

    private void OnCompletedShowItself()
    {
        StartCoroutine(CompletionAnimation());
    }

    private IEnumerator CompletionAnimation()
    {
        yield return new WaitForSeconds(_showStarDeltaTime);
        _firstStar.alpha = _shownAlphaValue;
        yield return new WaitForSeconds(_showStarDeltaTime);
        if (_recipeQuality == RecipeQualityType.Good ||
            _recipeQuality == RecipeQualityType.Perfect)
        {
            _secondStar.alpha = _shownAlphaValue;
            yield return new WaitForSeconds(_showStarDeltaTime);
            if (_recipeQuality == RecipeQualityType.Perfect)
            {
                _thirdStar.alpha = _shownAlphaValue;
                yield return new WaitForSeconds(_showStarDeltaTime);
            }
        }
        
        OnStarsShown();
        yield break;
    }

    private void OnStarsShown()
    {
        _buttonsCanvasGroup.alpha = 1;
        _retryButton.ActivatePointerEvents();
        _nextLevelButton.ActivatePointerEvents();
    }

    private bool _havePressedButton;
    private void OnRetryButtonPressed()
    {
        if (!_havePressedButton)
        {
            _havePressedButton = true;
            InputDelegatesContainer.RetryLevelCommand();
        }
    }

    private void OnNextLevelButtonPressed()
    {
        if (!_havePressedButton)
        {
            _havePressedButton = true;
            InputDelegatesContainer.NextLevelCommand();
        }
    }
}