using System;
using UnityEngine;

public static class PlayerEventsContainer
{
    // public static Action<PlayerMovementStateType> EventEntryNewMovementState;
    // public static Action<PlayerMovementStateType> EventFinalFrameMovementState;

    #region LifeDeath
    #endregion//LifeDeath

    #region TriggerEnters
    public static Action<Collider> EventPlayerOnTriggerEnter;
    public static Action<Collider> EventPlayerOnTriggerStay;
    public static Action<Collider> EventPlayerOnTriggerExit;

    // public static Action<PuzzleItem> EventPuzzleItemTriggerEnter;
    // public static Action<PuzzleItem> EventPuzzleItemTriggerExit;
    #endregion
}