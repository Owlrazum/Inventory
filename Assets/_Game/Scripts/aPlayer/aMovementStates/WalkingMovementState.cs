// using UnityEngine;

// public class WalkingMovementState : PlayerMovementState
// {
//     [SerializeField]
//     private float _moveSpeed = 3;

//     // [SerializeField]
//     // private bool _shouldStopWalkingWhenCollidingWithWall;

//     private float _horizontalInput;
//     private float _speedNormalizedByInput;

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             return true;
//         }

//         if (!PlayerQueriesContainer.QueryIsGrounded())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Falling);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsSlidingTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.Sliding);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGroundJumpTrigering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.GroundJumping);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsDashTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Dash);
//             return true;
//         }

//         // if (PlayerQueriesContainer.FuncIsWallRunTriggering())
//         // {
//         //     PlayerEventsContainer.EventEntryNewMovementState.Invoke(PlayerMovementStateType.WallRun);
//         //     return true;
//         // }

//         if (!IsInputTriggeringAbility())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Idle);
//             return true;
//         }

//         return false;
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         if (_horizontalInput > 0)
//         {
//             _playerCharacter.ChangeFacingDirection(PlayerCharacter.FacingDirectionType.Right);
//             _speedNormalizedByInput = _moveSpeed;
//         }
//         else if (_horizontalInput < 0)
//         {
//             _playerCharacter.ChangeFacingDirection(PlayerCharacter.FacingDirectionType.Left);
//             _speedNormalizedByInput = -_moveSpeed;
//         }

//         displacement   = Vector2.zero;
//         displacement.x = _speedNormalizedByInput * Time.deltaTime;
//     }

//     private bool IsInputTriggeringAbility()
//     {
//         var horizontalMoveCommand = InputQueriesContainer.QueryHorizontalMoveCommand();
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
//             Debug.LogError("NoCommanding to direction in moveCommand PlayerWalking");
//             return false;
//         }
//     }


//     public override string ToString()
//     {
//         return "Walk ab";
//     }
// }