using System;

public static class EventsContainer
{
    public static Action EventGameStart;

    public static Action ShouldLoadNextScene;
    public static Action ShouldStartLoadingNextScene;
    public static Action EventStartedLoadingNextScene;

    public static Action<LevelDescriptionSO> ShouldPrepareLevel;
}