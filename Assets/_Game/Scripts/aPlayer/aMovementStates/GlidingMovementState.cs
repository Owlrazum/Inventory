// using UnityEngine;

// public class GlidingMovementState : PlayerMovementState
// { 
//     [SerializeField]
//     private float _moveSpeed = 3;

//     [SerializeField]
//     private float _modifiedFallSpeed = 3;

//     private float _horizontalInput;
//     private float _speedNormalizedByInput;

//     private bool _isGliding;

//     private void Awake()
//     {
//         PlayerQueriesContainer.FuncIsGlidingTriggering += IsGlidingTriggering;
//     }

//     private void OnDestroy()
//     { 
//         PlayerQueriesContainer.FuncIsGlidingTriggering -= IsGlidingTriggering;  
//     }

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGrounded())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Idle);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsJumpableJumpTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.JumpableJumping);
//             return true;
//         }

//         if (!IsInputTriggeringGlide())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Falling);
//             return true;
//         }
        
//         if (PlayerQueriesContainer.QueryIsDashTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Dash);
//             return true;
//         }

//         return false;
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         ProcessInputTriggeringAirControl();

//         displacement = Vector2.zero;
//         displacement.x = _horizontalInput * _moveSpeed * Time.deltaTime;

//         displacement.y = -_modifiedFallSpeed * Time.deltaTime;
//     }

//     private bool IsGlidingTriggering()
//     {
//         if (!IsInputTriggeringGlide())
//         {
//             return false;
//         }

//         if (PlayerQueriesContainer.QueryIsGrounded())
//         {
//             return false;
//         }

//         return true;
//     }

//     private bool IsInputTriggeringGlide()
//     { 
//         var glideCommand = InputQueriesContainer.QueryGlideCommand();
//         if (!glideCommand.IsTriggered)
//         {
//             return false;
//         }
//         return true;
//     }

//     private bool ProcessInputTriggeringAirControl()
//     {
//         var horizontalMoveCommand = InputQueriesContainer.QueryHorizontalMoveCommand();
//         _horizontalInput = 0;
//         if (!horizontalMoveCommand.IsTriggered)
//         {
//             return false;
//         }

//         if (horizontalMoveCommand.CommandingToRight)
//         {
//             _horizontalInput = 1;
//             return true;
//         }
//         else if (horizontalMoveCommand.CommandingToLeft)
//         {
//             _horizontalInput = -1;
//             return true;
//         }
//         else
//         {
//             Debug.LogError("NoCommanding to direction in moveCommand PlayerGliding");
//             return false;
//         }
//     }

//     public override string ToString()
//     {
//         return "Gliding ab";
//     }
// }