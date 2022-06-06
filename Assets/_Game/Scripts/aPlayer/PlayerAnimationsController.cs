// using System.Collections;
// using UnityEngine;

// public class PlayerAnimationsController : MonoBehaviour
// {
//     [SerializeField]
//     private Animator _humAnimator;

//     [SerializeField]
//     private Animator _sphereAnimator;

//     [SerializeField]
//     private float _waitTimeForToHumanTransition = 0.3f;

//     [SerializeField]
//     private float _waitTimeForToSphereTransition = 0.3f;

//     private const string STATE_NAME_LANDING = "Landing";

//     private const string STATE_NAME_IDLE = "Idle";
//     private const string STATE_NAME_RUN  = "Walking";
//     private const string STATE_NAME_GROUND_JUMP = "Jumping";
//     private const string STATE_NAME_GLIDING = "Gliding";

//     private const string STATE_NAME_JUMPABLE_JUMP = "JumpableJump";
//     private const string STATE_NAME_DOUBLE_JUMP = "DoubleJump";
//     private const string STATE_NAME_FALLING = "Falling";
//     private const string STATE_NAME_DASHING = "Dashing";
//     private const string STATE_NAME_GRAVITY_PULL = "GravityPull";
//     private const string STATE_NAME_SLIDING = "Sliding";

//     private const string STATE_NAME_SPIKE_DAMAGED = "SpikeDamaged";

//     private const string STATE_NAME_TO_SPHERE_FORM = "ToSphereForm";
//     private const string STATE_NAME_TO_HUMANOID_FORM = "ToHumanoidForm";

//     private const float FADE_DURATION = 0.05f;
    
//     private enum State
//     { 
//         Humanoid,
//         TransitionToHum,
//         Sphere,
//         TransitionToSphere
//     }

//     private State _currentState;

//     private string _previousAnimationStateParameterName;
//     private PlayerMovementStateType _previousState;

//     private bool _shouldUpdateAnimatorThisFrame;
    
//     private string _stateToTransitionTo;
//     private string _afterTransitionState;

//     private IEnumerator _waitCoroutine;

//     private void Awake()
//     {
//         _currentState = State.Humanoid;
//         PlayerEventsContainer.EventFinalFrameMovementState += OnFinalFrameMovementState;
//     }

//     private void OnDestroy()
//     {
//         PlayerEventsContainer.EventFinalFrameMovementState -= OnFinalFrameMovementState;
//     }

//     private void OnFinalFrameMovementState(PlayerMovementStateType newMovementState)
//     {
//         _shouldUpdateAnimatorThisFrame = true;

//         switch (newMovementState)
//         {
//             case PlayerMovementStateType.Idle:
//                 if (_previousState == PlayerMovementStateType.Falling)
//                 {
//                     _stateToTransitionTo = STATE_NAME_IDLE;
//                 }
//                 else
//                 { 
//                     _stateToTransitionTo = STATE_NAME_IDLE;
//                 }
//                 break;
//             case PlayerMovementStateType.Walking:
//                 _stateToTransitionTo = STATE_NAME_RUN;
//                 break;
//             case PlayerMovementStateType.GroundJumping:
//                 _stateToTransitionTo = STATE_NAME_GROUND_JUMP;
//                 break;
//             case PlayerMovementStateType.Gliding:
//                 _stateToTransitionTo = STATE_NAME_GLIDING;
//                 break;

//             case PlayerMovementStateType.DoubleJumping:
//                 _stateToTransitionTo = STATE_NAME_DOUBLE_JUMP;
//                 break;
//             case PlayerMovementStateType.JumpableJumping:
//                 _stateToTransitionTo = STATE_NAME_JUMPABLE_JUMP;
//                 break;

//             case PlayerMovementStateType.Falling:
//                 _stateToTransitionTo = STATE_NAME_FALLING;
//                 break;

//             case PlayerMovementStateType.Sliding:
//                 _stateToTransitionTo = STATE_NAME_SLIDING;
//                 break;

//             case PlayerMovementStateType.Dash:
//                 _stateToTransitionTo = STATE_NAME_DASHING;
//                 break;

//             case PlayerMovementStateType.SpikeDamaged:
//                 _stateToTransitionTo = STATE_NAME_SPIKE_DAMAGED;
//                 break;

//             case PlayerMovementStateType.GravityPull:
//                 _stateToTransitionTo = STATE_NAME_GRAVITY_PULL;
//                 break;
//         }

//         _previousState = newMovementState;
//     }

//     private void LateUpdate() 
//     {
//         if (_shouldUpdateAnimatorThisFrame)
//         {
//             if (_currentState == State.Humanoid)
//             { 
//                 _humAnimator.CrossFade(_stateToTransitionTo, FADE_DURATION);
//             }
//             else if (_currentState == State.Sphere)
//             {
//                 _sphereAnimator.CrossFade(_stateToTransitionTo, FADE_DURATION);
//             }

//             _shouldUpdateAnimatorThisFrame = false;
//         }
//     }

//     private void Log(string msg)
//     {
//         Debug.Log("PlayerAnimationsController: " + msg);
//     }
// }