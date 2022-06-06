using System;

using UnityEngine;

public static class UIEventsContainer
{
    public static Action EscapePressed;
    public static Action EventSettingsEnter;
    public static Action EventSettingsExit;

    public static Action EventExitToMainMenuPressed;

#region MainMenu
    public static Action EventContinueButtonPressed;
#endregion

    public static Action<string> EventBuildLog;
}