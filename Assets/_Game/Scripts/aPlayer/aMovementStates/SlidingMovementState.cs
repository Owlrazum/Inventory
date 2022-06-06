// using UnityEngine;
// using System.Collections;

// public class SlidingMovementState : PlayerMovementState
// {
//     [SerializeField]
//     private float _maxHorizSpeed = 5;

//     [SerializeField]
//     private float _minHorizAcceleration = 0.5f;

//     [SerializeField]
//     private float _maxHorizAcceleration = 2;

//     [SerializeField]
//     private float _timeForStandUp = 0.2f;

//     [SerializeField]
//     private float _maxSlopeAngleForSliding = 60;


//     private const float VERTICAL_SPEED_WHILE_STANDUP = 1;

//     private float _minSlopeAngleForSliding;

//     private float _currentSlopeAngleDeg;
//     private float _currentSlopeAngleDegAbs;
    
//     private float _currentSpeed;
//     private Vector2 _direction;

//     private float _currentAcceleration;

//     private bool _isStandingUp;
//     private float _standUpTimer;
//     private float _standUpStartSpeed;

//     private void Awake()
//     {
//         PlayerQueriesContainer.FuncSlopeAngleMaxThreshold += GetSlopeAngleMaxThreshold;

//         PlayerQueriesContainer.FuncIsSlidingTriggering += IsSlidingTriggering;
//     }

//     private void OnDestroy()
//     {
//         PlayerQueriesContainer.FuncSlopeAngleMaxThreshold -= GetSlopeAngleMaxThreshold;

//         PlayerQueriesContainer.FuncIsSlidingTriggering -= IsSlidingTriggering;
//     }

//     private float GetSlopeAngleMaxThreshold()
//     {
//         return _maxSlopeAngleForSliding;
//     }

//     private void Start()
//     {
//         _minSlopeAngleForSliding = PlayerQueriesContainer.QuerySlopeAngleMinThreshold();
//     }

//     public override void OnEntry()
//     {
//         _currentSlopeAngleDeg = PlayerQueriesContainer.QuerySlopeAngleBelowRaycast();
//         _currentSlopeAngleDegAbs = Mathf.Abs(_currentSlopeAngleDeg);
//         PlayerCharacter.FacingDirectionType angleFacingDirection;
//         if (_currentSlopeAngleDeg > 0)
//         {
//             angleFacingDirection = PlayerCharacter.FacingDirectionType.Right;
//             _direction = Quaternion.Euler(-Vector3.forward * _currentSlopeAngleDeg) * Vector2.right;
//         }
//         else
//         {
//             angleFacingDirection = PlayerCharacter.FacingDirectionType.Left;
//             _direction = Quaternion.Euler(-Vector3.forward * _currentSlopeAngleDeg) * Vector2.left;
//         }

//         if (_playerCharacter.GetFacingDirection() != angleFacingDirection)
//         {
//             _playerCharacter.ChangeFacingDirection(angleFacingDirection);
//         }

//         float lerpParam = Mathf.InverseLerp(0, 90, _currentSlopeAngleDegAbs);
//         _currentSpeed = Mathf.Abs(PlayerQueriesContainer.QueryFallSpeed() * lerpParam);
//     }

//     public override bool CheckForTransitions()
//     {
//         if (base.CheckForTransitions())
//         {
//             return true;
//         }

//         if (!PlayerQueriesContainer.QueryIsGrounded() && 
//             !PlayerQueriesContainer.QueryIsCollidingToTheLeft() && 
//             !PlayerQueriesContainer.QueryIsCollidingToTheRight())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Falling);
//             return true;
//         }

//         if (!CheckSlopeAngle())
//         {
//             return true;
//         }

//         if (PlayerQueriesContainer.QueryIsGroundJumpTrigering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.GroundJumping);
//             _isStandingUp = false;
//             return true;
//         }

//         return false;
//     }

//     private bool CheckSlopeAngle()
//     { 
//         _currentSlopeAngleDeg = PlayerQueriesContainer.QuerySlopeAngleBelowRaycast();
//         _currentSlopeAngleDegAbs = Mathf.Abs(_currentSlopeAngleDeg);
//         if (_currentSlopeAngleDeg > _maxSlopeAngleForSliding)
//         {
//             PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Falling);
//             _isStandingUp = false;
//             return false;
//         }

//         if (_currentSlopeAngleDegAbs < _minSlopeAngleForSliding)
//         {
//             if (!_isStandingUp)
//             {
//                 _isStandingUp = true;
//                 _standUpTimer = 0;
//                 _standUpStartSpeed = _currentSpeed;
//             }
//             else if (_standUpTimer > _timeForStandUp)
//             {
//                 PlayerEventsContainer.EventEntryNewMovementState?.Invoke(PlayerMovementStateType.Idle);
//                 _isStandingUp = false;
//                 return false;
//             }
//         }

//         return true;
//     }

//     public override void GetDisplacement(out Vector2 displacement)
//     {
//         displacement = Vector2.zero;

//         if (_isStandingUp)
//         {
//             _standUpTimer += Time.deltaTime;
//             if (_standUpTimer > _timeForStandUp)
//             {
//                 return;
//             }
//             float lerpParam = _standUpTimer / _timeForStandUp;
//             _currentSpeed = Mathf.Lerp(_standUpStartSpeed, 0, lerpParam);
//             _direction = _direction.x > 0 ?
//                 Quaternion.Euler(-Vector3.forward * _currentSlopeAngleDeg) * Vector2.right
//                 :
//                 Quaternion.Euler(-Vector3.forward * _currentSlopeAngleDeg) * Vector2.left;
//         }

//         displacement.x = _currentSpeed * _direction.x * Time.deltaTime;
//         displacement.y = (_currentSpeed * _direction.y) * Time.deltaTime;

//         Debug.DrawRay(Vector3.zero, displacement * 10, Color.red, 0.4f, false);

//         if (_currentSpeed != _maxHorizSpeed && !_isStandingUp)
//         { 
//             float lerpParam = Mathf.InverseLerp(_minSlopeAngleForSliding, _maxSlopeAngleForSliding, _currentSlopeAngleDegAbs);
//             _currentAcceleration = Mathf.Lerp(_minHorizAcceleration, _maxHorizAcceleration, lerpParam);
//             _currentSpeed += _currentAcceleration * Time.deltaTime;
//             if (_currentSpeed > _maxHorizSpeed)
//             {
//                 _currentSpeed = _maxHorizSpeed;
//             }
//         }
//     }

//     private bool IsSlidingTriggering()
//     {
//         if (!PlayerQueriesContainer.QueryIsGrounded())
//         {
//             return false;
//         }
        
//         float slopeAngleBelowRaycast = Mathf.Abs(PlayerQueriesContainer.QuerySlopeAngleBelowRaycast());
//         if (slopeAngleBelowRaycast > _minSlopeAngleForSliding
//             &&
//             slopeAngleBelowRaycast < _maxSlopeAngleForSliding)
//         { 
//             return true;
//         }
        
//         return false;
//     }
// }