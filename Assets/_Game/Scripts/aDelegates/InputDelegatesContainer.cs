using System;
using UnityEngine;

public static class InputDelegatesContainer
{
    public static Func<Vector2> GetPointerPosition;

    public static Action StartGameCommand;
    public static Action ExitToMainMenuCommand;
    public static Action ExitGameCommand;

    public static Action<UIStack, Vector2Int> SelectStackCommand;

    public static Action RetryLevelCommand;
    public static Action NextLevelCommand;
}