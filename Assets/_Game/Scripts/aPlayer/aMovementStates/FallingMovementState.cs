// using UnityEngine;

// /// <summary>
// /// It is tied to the falling movementState.
// /// </summary>
// public class FallingMovementState : PlayerMovementState
// {
//     [SerializeField]
//     private float _fallingGravityAmount;

//     [SerializeField]
//     private float _maxVerticalSpeed = 20;

//     [SerializeField]
//     [Tooltip("Set to zero if not air control is needed")]
//     private float _horizontalMoveSpeed = 3;

//     private const float _MOVING_PLATFORMS_GRAVITY = -500;

//     private bool _isFalling;
//     private float _currentVerticalSpeed;
//     private float _horizontalInput;

//     private float _minSlopeAngleForSliding;
//     private float _maxSlopeAngleForSliding;

//     private void Awake()
//     {
//         PlayerQueriesContainer.FuncFallSpeed += GetFallSpeed;
//         PlayerQueriesContainer.FuncFallingGravityAmount += GetFallingGravityAmount;
//     }

//     protected void OnDestroy()
//     {
//         PlayerQueriesContainer.FuncFallSpeed -= GetFallSpeed;
//         PlayerQueriesContainer.FuncFallingGravityAmount -= GetFallingGravityAmount;
//     }

//     private void Start()
//     {
//         _minSlopeAngleForSliding = PlayerQueriesContainer.QuerySlopeAngleMinThreshold();
//         _maxSlopeAngleForSliding = PlayerQueriesContainer.QuerySlopeAngleMaxThreshold();
//     }

//     private float GetFallingGravityAmount()
//     {
//         return _fallingGravityAmount;
//     }

//     private float GetFallSpeed()
//     {
//         return _currentVerticalSpeed;
//     }

//     public override void OnEntry()
//     {
//         _currentVerticalSpeed = 0;
//     }

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsSlidingTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Sliding);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsBufferGroundJumpTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.GroundJumping);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGrounded())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Idle);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsCoyoteGroundJumpTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.GroundJumping);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsJumpableJumpTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.JumpableJumping);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsDoubleJumpTrigering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.DoubleJumping);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsDashTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Dash);
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGlidingTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Gliding);
//             return true;
//         }

//         return false;
//     }

//     public override void OnTransition()
//     {
//         PlayerEventsContainer.EventInputBufferShouldReset?.Invoke();
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         _currentVerticalSpeed += _fallingGravityAmount * Time.deltaTime;
//         _currentVerticalSpeed = Mathf.Clamp(_currentVerticalSpeed, 0, _maxVerticalSpeed);
//         ProcessInputTriggeringAirControl();


//         displacement = Vector2.zero;
//         if (!PlayerQueriesContainer.QueryIsCollidingWithDashThroughZone(_horizontalInput * Vector2.right))
//         { 
//             displacement.x = _horizontalInput * _horizontalMoveSpeed * Time.deltaTime;
//         }
//         if (_playerCharacter.GetFacingDirection() == PlayerCharacter.FacingDirectionType.Left &&
//             _horizontalInput > 0)
//         {
//             _playerCharacter.ChangeFacingDirection(PlayerCharacter.FacingDirectionType.Right);
//         }
//         else if (_playerCharacter.GetFacingDirection() == PlayerCharacter.FacingDirectionType.Right &&
//                  _horizontalInput < 0)
//         { 
//             _playerCharacter.ChangeFacingDirection(PlayerCharacter.FacingDirectionType.Left);
//         }
//         displacement.y = -_currentVerticalSpeed * Time.deltaTime; //_gravityFromMovingPlatforms
//     }

//     private void ProcessInputTriggeringAirControl()
//     {
//         var horinzontalMoveCommand = InputQueriesContainer.QueryHorizontalMoveCommand();
//         if (!horinzontalMoveCommand.IsTriggered)
//         {
//             _horizontalInput = 0;
//             return;
//         }

//         if (horinzontalMoveCommand.CommandingToRight)
//         {
//             _horizontalInput = 1;
//         }
//         else if (horinzontalMoveCommand.CommandingToLeft)
//         {
//             _horizontalInput = -1;
//         }
//         else
//         {
//             Debug.LogError("NoCommanding To LEft or Gravity jump");
//             _horizontalInput = 0;
//         }
//     }

//     public override string ToString()
//     {
//         return "Gravity Fall";
//     }
// }