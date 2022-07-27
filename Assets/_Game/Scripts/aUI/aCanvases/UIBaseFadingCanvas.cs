using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasGroup))]
public class UIBaseFadingCanvas : MonoBehaviour
{
    protected virtual void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        fadeState = FadeState.Hided;
    }

    [Header("FadingParameters")]
    [Space]
    [SerializeField]
    protected float fadeInTime = 0.5f;

    [SerializeField]
    protected float fadeOutTime = 0.5f;

    private enum FadeState
    {
        Shown,
        Hided,
        FadingIn,
        FadingOut
    }

    private FadeState fadeState;

    /// <summary>
    /// Should be used carefully, watch what is in the base class before using it in derived classes.
    /// </summary>
    protected CanvasGroup _canvasGroup;
    
    protected Action ActionCompletedShowItself;
    protected Action ActionCompletedHideItself;

    private IEnumerator fadingCoroutine;

    public virtual void ShowItself()
    {
        if (fadeState == FadeState.Hided)
        {
            fadingCoroutine = FadingIn();
            StartCoroutine(fadingCoroutine);
        }
        else if (fadeState == FadeState.FadingOut)
        {
            StopCoroutine(fadingCoroutine);
            fadingCoroutine = FadingIn();
            StartCoroutine(fadingCoroutine);
        }
    }

    private IEnumerator FadingIn()
    {
        fadeState = FadeState.FadingIn;
        float fadeParam = _canvasGroup.alpha;
        while (fadeParam < 1)
        {
            fadeParam += Time.unscaledDeltaTime / fadeInTime;
            _canvasGroup.alpha = fadeParam;
            yield return null;
        }
        _canvasGroup.alpha = 1;
        fadeState = FadeState.Shown;
        ActionCompletedShowItself?.Invoke();
    }   

    public virtual void HideItself()
    { 
        if (fadeState == FadeState.Shown)
        {
            fadingCoroutine = FadingOut();
            StartCoroutine(fadingCoroutine);
        }
        else if (fadeState == FadeState.FadingIn)
        {
            StopCoroutine(fadingCoroutine);
            fadingCoroutine = FadingOut();
            StartCoroutine(fadingCoroutine);
        }
    }

    private IEnumerator FadingOut()
    {
        fadeState = FadeState.FadingOut;
        float fadeParam = _canvasGroup.alpha;
        while (fadeParam > 0)
        {
            fadeParam -= Time.unscaledDeltaTime / fadeOutTime;
            _canvasGroup.alpha = fadeParam;
            yield return null;
        }
        _canvasGroup.alpha = 0;
        fadeState = FadeState.Hided;
        ActionCompletedHideItself?.Invoke();
    }
}
