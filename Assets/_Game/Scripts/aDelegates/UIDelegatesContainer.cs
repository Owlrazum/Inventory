using System;
using UnityEngine;
using Orazum.UI;

public static class UIDelegatesContainer
{
    public static Func<Vector2Int> GetReferenceScreenResolution;

    // UIEventsUpdater
    public static Func<UIPointerEventsUpdater> GetEventsUpdater;

    public static Action ShowEndLevelCanvas;

    public static Action<string> BuildLog;
}