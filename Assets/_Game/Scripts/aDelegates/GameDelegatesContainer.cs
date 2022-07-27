using System;
using UnityEngine;

public static class GameDelegatesContainer
{
    public static Func<GameStateType> GetGameState;
    public static Func<Vector3> GetPlayerPos;
    // public static Func<PlayerStateType> GetPlayerState;

    public static Action StartCameraTransitionTo;

    public static Action<LevelDescriptionSO> StartLevel;
    public static Action<RecipeQualityType> CompleteLevel;
    public static Action FailLevel;

    public static Action EventLevelStarted;
    public static Action EventLevelCompleted;
    public static Action EventLevelFailed;

    public static Func<bool> GetShouldShowTutorial;

    public static Action StartDrumRoll;
    public static Action EventDrumRollCompleted;

    public static Action ShowNoRecipeMessage;
    public static Action EventNoRecipeWasCrafted;
    
    public static Action<RecipeQualityType> EventRecipeWasCrafted;
}