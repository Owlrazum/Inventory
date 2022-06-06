// using System.Collections;

// using UnityEngine;

// // TODO: add coyote time and input buffering
// public class JumpGroundMovementState : PlayerMovementState
// {
//     [SerializeField]
//     private float _jumpHeight;

//     [SerializeField]
//     [Tooltip("Set to zero if not air control needed")]
//     private float _airHorizontalMoveSpeed;

//     [SerializeField]
//     private float OnEntryGroundedIgnoranceTime = 0.3f;

//     [Header("Quality of life")]
//     [SerializeField]
//     private float _coyoteTime;

//     [SerializeField]
//     private float _inputBufferingDuration;

//     private bool _isJumping;

//     private const float TIME_IS_GROUNDED_IGNORANCE_AFTER_JUMP_START = 0.2f;

//     private float _fallingGravityAmount;
//     private float _horizontalInput;
//     private float _currentSpeed;

//     private float _groundedIgnoranceTimer;

//     private JumpCommand _jumpCommand;
//     private HorizontalMoveCommand _horizMoveCommand;

//     private float _jumpTriggerBufferTimer = -1;

//     private void Awake()
//     {
//         PlayerEventsContainer.EventInputBufferShouldReset += OnInputBufferShouldReset;

//         PlayerQueriesContainer.FuncIsGroundJumpTrigering += IsGroundJumpTriggering;
//         PlayerQueriesContainer.FuncIsCoyoteGroundJumpTriggering += IsCoyoteGroundJumpTriggering;
//         PlayerQueriesContainer.FuncIsBufferGroundJumpTriggering += IsBufferGroundJumpTriggering;
//     }

//     private void OnDestroy()
//     {
//         PlayerEventsContainer.EventInputBufferShouldReset -= OnInputBufferShouldReset;

//         PlayerQueriesContainer.FuncIsGroundJumpTrigering -= IsGroundJumpTriggering;
//         PlayerQueriesContainer.FuncIsCoyoteGroundJumpTriggering -= IsCoyoteGroundJumpTriggering;
//         PlayerQueriesContainer.FuncIsBufferGroundJumpTriggering -= IsBufferGroundJumpTriggering;
//     }

//     private void Start()
//     {
//         _fallingGravityAmount = PlayerQueriesContainer.QueryFallingGravityAmount();
        
//         _jumpCommand = InputQueriesContainer.QueryJumpCommand();
//         _horizMoveCommand = InputQueriesContainer.QueryHorizontalMoveCommand();
//     }

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             return true;
//         }

//         if (_groundedIgnoranceTimer > TIME_IS_GROUNDED_IGNORANCE_AFTER_JUMP_START)
//         { 
//             if ( PlayerQueriesContainer.QueryIsGrounded())
//             {
//                 PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Idle);
//                 return true;
//             }
//         }
//         else
//         {
//             _groundedIgnoranceTimer += Time.deltaTime;
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

//         if ( PlayerQueriesContainer.QueryIsCollidingAbove() || _currentSpeed < 0)
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Falling);
//             return true;
//         }

//         return false;
//     }

//     public override void OnEntry()
//     {
//         _jumpCommand.IsConsumed = true;

//         _groundedIgnoranceTimer = 0;
//         _currentSpeed = Mathf.Sqrt(2 * _jumpHeight * _fallingGravityAmount);

//         StartCoroutine(GroundedIgnoranceTime());
//     }

//     private IEnumerator GroundedIgnoranceTime()
//     {
//         yield return null;
//         PlayerEventsContainer.EventGroundedIgnoranceStart?.Invoke();
//         yield return new WaitForSeconds(OnEntryGroundedIgnoranceTime);
//         PlayerEventsContainer.EventGroundedIgnoranceEnd?.Invoke();
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         ProcessInputTriggeringAirControl();

//         displacement = Vector2.zero;
//         displacement.x = _horizontalInput * _airHorizontalMoveSpeed * Time.deltaTime;
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
//         displacement.y = _currentSpeed * Time.deltaTime;

//         _currentSpeed -= _fallingGravityAmount * Time.deltaTime;
//     }

//     private bool IsGroundJumpTriggering()
//     {
//         if (!_jumpCommand.IsTriggered)
//         {
//             return false;
//         }

//         if (PlayerQueriesContainer.QueryIsCollidingAbove() || 
//             !PlayerQueriesContainer.QueryIsGrounded())
//         {
//             return false;
//         }

//         return true;
//     }

//     private bool IsCoyoteGroundJumpTriggering()
//     {
//         if (!_jumpCommand.IsTriggered)
//         {
//             return false;
//         }

//         if (PlayerQueriesContainer.QueryIsCollidingAbove())
//         {
//             return false;
//         }

//         if (PlayerQueriesContainer.QueryPreviousStateType() != PlayerMovementStateType.Walking)
//         {
//             return false;
//         }
//         if (PlayerQueriesContainer.QueryTimeSinceLastGrounded() < _coyoteTime)
//         {
//             return true;
//         }

//         return false;
//     }

//     /// <summary>
//     /// Should be updated each frame to work.
//     /// </summary>
//     private bool IsBufferGroundJumpTriggering()
//     {
//         if (_jumpTriggerBufferTimer < 0 || _jumpTriggerBufferTimer > _inputBufferingDuration)
//         { 
//             if (_jumpCommand.IsTriggered)
//             { 
//                 _jumpTriggerBufferTimer = 0;
//             }
//             return false;
//         }

//         if (PlayerQueriesContainer.QueryIsGrounded())
//         {
//             _jumpTriggerBufferTimer = -1;
//             return true;
//         }

//         _jumpTriggerBufferTimer += Time.deltaTime;
//         return false;
//     }

//     private void ProcessInputTriggeringAirControl()
//     {
//         if (!_horizMoveCommand.IsTriggered)
//         {
//             _horizontalInput = 0;
//             return;
//         }

//         if (_horizMoveCommand.CommandingToRight)
//         {
//             _horizontalInput = 1;
//         }
//         else if (_horizMoveCommand.CommandingToLeft)
//         {
//             _horizontalInput = -1;
//         }
//         else
//         {
//             Debug.LogError("NoCommanding To LEft or Right Ground jump");
//             _horizontalInput = 0;
//         }
//     }

//     private void OnInputBufferShouldReset()
//     {
//         _jumpTriggerBufferTimer = -1;
//     }

//     public override string ToString()
//     {
//         return "GroundJump ab";
//     }
// }