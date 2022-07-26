using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class EvaluateRecipeCanvas : MonoBehaviour
{
    private UIButtonPressAnimated _evaluateButton;
    private void Awake()
    {
        if (!transform.GetChild(0).TryGetComponent(out _evaluateButton))
        {
            Debug.LogError("The button for evaluation was not found");
            return;
        }

        _evaluateButton.EventOnTouch += OnEvaluateButtonPressed;
    }

    private void OnDestroy()
    {
        _evaluateButton.EventOnTouch -= OnEvaluateButtonPressed;
    }

    private void OnEvaluateButtonPressed()
    {
        print("-------===========--------");
        RecipeQualityType recipeQuality = CraftingDelegatesContainer.EvaluateRecipeQuality();
        print("button pressed, the quality is " + recipeQuality);
        if (recipeQuality == RecipeQualityType.NoRecipe)
        {
            return;
        }

        print("Complete level");
        GameDelegatesContainer.CompleteLevel(recipeQuality);
    }
}