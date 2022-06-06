// using UnityEngine;

// public class IdleMovementState : PlayerMovementState
// {
//     private LookUpCommand _lookUpCommand;
//     private LookDownCommand _lookDownCommand;

//     private enum LookState
//     {
//         Default,
//         Up,
//         Down
//     }

//     private LookState _lookState;

//     private void Awake()
//     {
//         PlayerQueriesContainer.FuncInitialMovementState += GetThis;
//     }

//     private void OnDestroy()
//     {
//         PlayerQueriesContainer.FuncInitialMovementState -= GetThis;
//     }

//     private void Start()
//     {
//         _lookUpCommand = InputQueriesContainer.QueryLookUpCommand();
//         _lookDownCommand = InputQueriesContainer.QueryLookDownCommand();
//     }

//     private IdleMovementState GetThis()
//     {
//         return this;
//     }

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             CheckLookState();
//             return true;
//         }

//         if (!PlayerQueriesContainer.QueryIsGrounded())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.Falling);
//             CheckLookState();
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsSlidingTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.Sliding);
//             CheckLookState();
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGroundJumpTrigering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.GroundJumping);
//             CheckLookState();
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsDashTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.Dash);
//             CheckLookState();
//             return true;
//         }

//         // if (PlayerQueriesContainer.QueryIsWallRunTriggering())
//         // {
//         //     PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.WallRun);
//         //     CheckLookState();
//         //     return true;
//         // }

//         HorizontalMoveCommand horizontalMoveCommand = InputQueriesContainer.QueryHorizontalMoveCommand();
//         if (horizontalMoveCommand.IsTriggered)
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Walking);
//             CheckLookState();
//             return true;
//         }
//         return false;
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         displacement = Vector2.zero;

//         if (_lookUpCommand.IsTriggered || _lookDownCommand.IsTriggered)
//         { 
//             if (_lookUpCommand.IsTriggered && _lookState != LookState.Up)
//             {
//                 PlayerEventsContainer.EventStartLookUp?.Invoke();
//                 _lookState = LookState.Up;
//             } else if (_lookDownCommand.IsTriggered && _lookState != LookState.Down)
//             {
//                 PlayerEventsContainer.EventStartLookDown?.Invoke();
//                 _lookState = LookState.Down;
//             }
//         } else
//         {
//             PlayerEventsContainer.EventStopLookUpOrDown?.Invoke();
//             _lookState = LookState.Default;
//         }
//     }

//     private void CheckLookState()
//     {
//         if (_lookState != LookState.Default)
//         { 
//             PlayerEventsContainer.EventStopLookUpOrDown?.Invoke();
//             _lookState = LookState.Default;
//         }
//     }
// }