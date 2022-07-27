using System.Collections;
using UnityEngine;

public class NoRecipeMessageCanvas : UIBaseFadingCanvas
{
    [SerializeField]
    private float _messageShowTime = 2;

    protected override void Awake()
    {
        base.Awake();
        _canvasGroup.alpha = 0;

        GameDelegatesContainer.ShowNoRecipeMessage += ShowNoRecipeMessage;
        ActionCompletedShowItself += OnCompletedShowItself;
        ActionCompletedHideItself += OnCompletedHideItself;
    }

    private void OnDestroy()
    { 
        GameDelegatesContainer.ShowNoRecipeMessage -= ShowNoRecipeMessage;
        ActionCompletedShowItself -= OnCompletedShowItself;
        ActionCompletedHideItself -= OnCompletedHideItself;
    }

    private void ShowNoRecipeMessage()
    {
        gameObject.SetActive(true);
        ShowItself();
    }

    private void OnCompletedShowItself()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(_messageShowTime);
        HideItself();
    }

    private void OnCompletedHideItself()
    {
        GameDelegatesContainer.EventNoRecipeWasCrafted?.Invoke();
    }
}