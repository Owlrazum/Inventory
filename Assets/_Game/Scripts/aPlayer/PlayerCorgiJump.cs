// using System;
// using UnityEngine;

// using MoreMountains.CorgiEngine;
// using MoreMountains.Tools;

// public class PlayerCorgiJump : CharacterJump
// {
//     [SerializeField]
//     private float _minimumJumpHeight = 0.3f;
//     private float _initialJumpHeight;
//     private bool _isJumpHeightOverrided;
//     private bool _isHavingJumpingPossibility;

//     private void Awake()
//     {
//         _initialJumpHeight = JumpHeight;

//         PlayerEventsContainer.OverrideJumpSettingsBeforeJump += OnOverrideJumpHeight;
//         PlayerEventsContainer.JumpingPossibilityCame += OnJumpingPossibilityCame;
//         PlayerEventsContainer.JumpingPossibilityGone += OnJumpingPossibilityGone;
//     }

//     private void OnDestroy()
//     {
//         PlayerEventsContainer.OverrideJumpSettingsBeforeJump -= OnOverrideJumpHeight;
//         PlayerEventsContainer.JumpingPossibilityCame -= OnJumpingPossibilityCame;
//         PlayerEventsContainer.JumpingPossibilityGone -= OnJumpingPossibilityGone;
//     }

//     protected override bool EvaluateJumpConditions()
//     {
//         if (!AbilityAuthorized
//             | (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
//             || (_controller.State.IsCollidingAbove))
//         {
//             return false;
//         }

//         if ((!_controller.State.IsGrounded)
//             && !EvaluateJumpTimeWindow()
//             && (_movement.CurrentState != CharacterStates.MovementStates.LadderClimbing)
//             && (NumberOfJumpsLeft <= 0)
//             && !_isHavingJumpingPossibility)
//         {
//             return false;
//         }

//         if (_inputManager != null)
//         {
//             // if the character is standing on a one way platform and is also pressing the down button,
//             if (_verticalInput < -_inputManager.Threshold.y && _controller.State.IsGrounded)
//             {
//                 if (JumpDownFromOneWayPlatform())
//                 {
//                     return false;
//                 }
//             }

//             // if the character is standing on a moving platform and not pressing the down button,
//             if (_controller.State.IsGrounded)
//             {
//                 JumpFromMovingPlatform();
//             }
//         }

//         return true;
//     }


//     /// <summary>
//     /// A small override of behaviour when stepping off the platform.
//     /// </summary> 
//     public override void JumpStart()
//     {
//         if (!EvaluateJumpConditions())
//         {
//             return;
//         }

//         // Corgi: we reset our walking speed
//         if ((_movement.CurrentState == CharacterStates.MovementStates.Crawling)
//             || (_movement.CurrentState == CharacterStates.MovementStates.Crouching)
//             || (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing))
//         {
//             _characterHorizontalMovement.ResetHorizontalSpeed();
//         }

//         if (_movement.CurrentState == CharacterStates.MovementStates.LadderClimbing)
//         {
//             _characterLadder.GetOffTheLadder();
//         }

//         _controller.ResetColliderSize();

//         _lastJumpAt = Time.time;

//         // Corgi: if we're still here, the jump will happen
//         // Corgi: we set our current state to Jumping

//         // Corgi: we trigger a character event
//         MMCharacterEvent.Trigger(_character, MMCharacterEventTypes.Jump);


//         // Corgi: we start our feedbacks
//         if ((_controller.State.IsGrounded) || _coyoteTime)
//         {
//             PlayAbilityStartFeedbacks();
//         }
//         else
//         {
//             AirJumpFeedbacks?.PlayFeedbacks();
//         }

//         if (ResetCameraOffsetOnJump && (_sceneCamera != null))
//         {
//             _sceneCamera.ResetLookUpDown();
//         }

//         if (NumberOfJumpsLeft != NumberOfJumps)
//         {
//             _doubleJumping = true;
//         }

//         // Corgi: we decrease the number of jumps left
//         //!!NumberOfJumpsLeft = NumberOfJumpsLeft - 1;
//         bool shouldZeroOutNumberOfJumpsLeft =
//             NumberOfJumpsLeft == 2 &&
//             !_controller.State.IsGrounded &&
//             _movement.CurrentState == CharacterStates.MovementStates.Falling;

//         if (!_isHavingJumpingPossibility)
//         {
//             NumberOfJumpsLeft = shouldZeroOutNumberOfJumpsLeft ?
//                 0 : NumberOfJumpsLeft - 1;
//         }
//         else
//         { 
//             _isHavingJumpingPossibility = false;
//             if (shouldZeroOutNumberOfJumpsLeft)
//             { 
//                 NumberOfJumpsLeft = 1;
//             }
//         }

//         _movement.ChangeState(CharacterStates.MovementStates.Jumping);

//         // Corgi: we reset our current condition and gravity
//         _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
//         _controller.GravityActive(true);
//         _controller.CollisionsOn();

//         // Corgi: we set our various jump flags and counters
//         SetJumpFlags();
//         CanJumpStop = true;

//         // Corgi: we make the character jump

//         // BeforeJump event may cause override of jump height by jumpable.
//         PlayerEventsContainer.BeforeJump?.Invoke();
//         _controller.SetVerticalForce(Mathf.Sqrt(2f * JumpHeight * Mathf.Abs(_controller.Parameters.Gravity)));
//         JumpHappenedThisFrame = true;
//         PlayerEventsContainer.Jumped?.Invoke();
//         if (_isJumpHeightOverrided)
//         {
//             JumpHeight = _initialJumpHeight;
//             _isJumpHeightOverrided = false;
//         }
//     }

//     public void OnOverrideJumpHeight(float newJumpHeight)
//     {
//         if (newJumpHeight < _minimumJumpHeight || newJumpHeight > 100)
//         {
//             Debug.LogWarning("IncorrectOverriding of jump heightx");
//             newJumpHeight = _minimumJumpHeight;
//             _isJumpHeightOverrided = true;
//             return;
//         }
//         JumpHeight = newJumpHeight;
//         _isJumpHeightOverrided = true;
//     }

//     public void OnJumpingPossibilityCame()
//     {
//         _isHavingJumpingPossibility = true;
//     }

//     public void OnJumpingPossibilityGone()
//     {
//         _isHavingJumpingPossibility = false;
//     }
// }

// // EvaluateJumpConditions() 
// //|| (!_controller.CanGoBackToOriginalSize() && !onAOneWayPlatform)
// //|| ((_condition.CurrentState != CharacterStates.CharacterConditions.Normal) // or if we're not in the normal stance
// //    && (_condition.CurrentState != CharacterStates.CharacterConditions.ControlledMovement))
// //|| (_movement.CurrentState == CharacterStates.MovementStates.Jetpacking) // or if we're jetpacking
// //|| ((_movement.CurrentState == CharacterStates.MovementStates.WallClinging) && (_characterWallJump != null)) // or if we're wallclinging and can walljump
// //|| (_movement.CurrentState == CharacterStates.MovementStates.Pushing) // or if we're pushing     
// //|| (_controller.State.IsCollidingAbove && !onAOneWayPlatform)) // or if we're colliding with the ceiling   

// // if (_controller.State.IsGrounded
// //     && (NumberOfJumpsLeft <= 0))
// // {
// //     return false;
// // }



// // if ((_airControl < 1f) && !_playerController.IsGrounded())
// //         {
// //             _horizontalInput = Mathf.Lerp(_lastGroundedHorizontalInput, _horizontalInput, _airControl);
// //         }



// // if  (!_playerController.IsGrounded() && 
// //             (
// //             (_movementStateMachine.GetCurrentState() == PlayerMovementStateType.Walking) || 
// //             (_movementStateMachine.GetCurrentState() == PlayerMovementStateType.Idle))
// //             )
// //         {
// //             _movementStateMachine.SetState(PlayerMovementStateType.Falling);
// //         }