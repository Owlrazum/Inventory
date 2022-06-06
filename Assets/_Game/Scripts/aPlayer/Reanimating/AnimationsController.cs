// using System.Collections;
// using UnityEngine;

// public class AnimationsController : MonoBehaviour
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

//         CheckForm(newMovementState);
//         _previousState = newMovementState;
//     }

//     private void CheckForm(PlayerMovementStateType newMovementState)
//     {
//         switch (newMovementState)
//         { 
//             case PlayerMovementStateType.Idle:
//             case PlayerMovementStateType.Walking:
//             case PlayerMovementStateType.GroundJumping:
//             case PlayerMovementStateType.Gliding:
//                 if (_currentState == State.Humanoid || _currentState == State.TransitionToHum)
//                 {
//                     return;
//                 }

//                 _afterTransitionState = _stateToTransitionTo;
//                 _shouldUpdateAnimatorThisFrame = false;


//                 PlayerEventsContainer.EventHumanoidForm?.Invoke();
//                 if (_currentState == State.TransitionToSphere)
//                 { 
//                     float timeOffset = 1 - _sphereAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//                     _sphereAnimator.CrossFade(STATE_NAME_TO_HUMANOID_FORM, FADE_DURATION, 0, timeOffset);
//                     _humAnimator.CrossFade(STATE_NAME_TO_HUMANOID_FORM, FADE_DURATION, 0, timeOffset);
//                     _currentState = State.TransitionToHum;
//                     StartWaitingTillTransitionEnds(PlayerFormType.Humanoid);
//                     return;
//                 }

//                 _sphereAnimator.CrossFade(STATE_NAME_TO_HUMANOID_FORM, FADE_DURATION);
//                 _humAnimator.CrossFade(STATE_NAME_TO_HUMANOID_FORM, FADE_DURATION);
//                 _currentState = State.TransitionToHum;
//                 StartWaitingTillTransitionEnds(PlayerFormType.Humanoid);
//                 break;
//             default:
//                 if (_currentState == State.Sphere || _currentState == State.TransitionToSphere)
//                 {
//                     return;
//                 }

//                 _afterTransitionState = _stateToTransitionTo;
//                 _shouldUpdateAnimatorThisFrame = false;

//                 PlayerEventsContainer.EventSphereForm?.Invoke();
//                 if (_currentState == State.TransitionToHum)
//                 {
//                     float timeOffset = 1 - _sphereAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
//                     _sphereAnimator.CrossFade(STATE_NAME_TO_SPHERE_FORM, FADE_DURATION, 0, timeOffset);
//                     _humAnimator.CrossFade(STATE_NAME_TO_SPHERE_FORM, FADE_DURATION, 0, timeOffset);
//                     _currentState = State.TransitionToSphere;
//                     StartWaitingTillTransitionEnds(PlayerFormType.Sphere);
//                     return;
//                 }

//                 _sphereAnimator.CrossFade(STATE_NAME_TO_SPHERE_FORM, FADE_DURATION);
//                 _humAnimator.CrossFade(STATE_NAME_TO_SPHERE_FORM, FADE_DURATION);
//                 _currentState = State.TransitionToSphere;
//                 StartWaitingTillTransitionEnds(PlayerFormType.Sphere);
//                 break;
//         }
//     }

//     private void StartWaitingTillTransitionEnds(PlayerFormType transition)
//     {
//         if (_waitCoroutine != null)
//         {
//             StopCoroutine(_waitCoroutine);
//         }
//         _waitCoroutine = WaitTillTransitionEnds(transition);
//         StartCoroutine(_waitCoroutine);
//     }

//     private IEnumerator WaitTillTransitionEnds(PlayerFormType transition)
//     {
//         if (transition == PlayerFormType.Humanoid)
//         { 
//             yield return new WaitForSeconds(_waitTimeForToHumanTransition);
//             OnTransitionToHumanoidAnimationEnd();
//             _waitCoroutine = null;
//         }
//         else if (transition == PlayerFormType.Sphere)
//         { 
//             yield return new WaitForSeconds(_waitTimeForToSphereTransition);
//             OnTransitionToSphereAnimationEnd();
//             _waitCoroutine = null;
//         }
//     }

//     private void OnTransitionToHumanoidAnimationEnd()
//     {
//         _shouldUpdateAnimatorThisFrame = true;
//         _stateToTransitionTo = _afterTransitionState;
//         _currentState = State.Humanoid;
//     }

//     private void OnTransitionToSphereAnimationEnd()
//     {
//         _shouldUpdateAnimatorThisFrame = true;
//         _stateToTransitionTo = _afterTransitionState;
//         _currentState = State.Sphere;
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