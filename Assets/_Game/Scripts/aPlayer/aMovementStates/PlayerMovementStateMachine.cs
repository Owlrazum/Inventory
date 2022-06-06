// using UnityEngine;

// public enum PlayerMovementStateType
// { 
// 	Idle,
// 	Dash,
// 	Walking,
// 	Falling,

//     Gliding,
// 	Sliding,
// 	GravityPull,
//     VerticalGravityFieldPush,
	
//     GroundJumping,
// 	DoubleJumping,
// 	JumpableJumping,
    
//     Teleporting,
//     SpikeDamaged,
// 	LadderClimbing
// }

// public class PlayerMovementStateMachine : MonoBehaviour
// {
//     private PlayerController _playerController;

//     private IdleMovementState        _idleMovementState;
//     private DashMovementState        _dashMovementState;
//     private WalkingMovementState     _walkingMovementState;
//     private FallingMovementState     _fallingMovementState;

//     private GlidingMovementState     _glidingMovementState;
//     private SlidingMovementState     _slidingMovementState;
//     private GravityPullMovementState _gravityPullMovementState;
//     private VerticalGravityFieldPushMovementState _verticalGravityFieldPushMovementState;

//     private JumpGroundMovementState   _jumpGroundMovementState;
//     private JumpDoubleMovementState   _jumpDoubleMovementState;
//     private JumpJumpableMovementState _jumpJumpableMovementState;

//     private SpikeDamagedMovementState _spikeDamagedMovementState;
//     private TeleportingMovementState  _teleportingMovementState;

//     private const float CLEAR_LOG_TIME = 1f;

//     private const int MAX_CHANGES_COUNT_IN_ONE_FRAME = 10;
//     private int _changesCountOneFrame = 0;

//     private float _clearLogTimer = 0;

//     private bool _areAbilitiesActive;

//     private PlayerMovementState _currentState;
//     private PlayerMovementStateType _currentStateType;
//     private Vector2 _currentDisplacement;

//     private PlayerMovementStateType _previousStateType;

//     // private Stack<PlayerAbility> 

//     private void Awake()
//     {
//         bool areAllHere = 
//             TryGetComponent(out _idleMovementState)        && 
//             TryGetComponent(out _dashMovementState)        && 
//             TryGetComponent(out _walkingMovementState)     &&
//             TryGetComponent(out _fallingMovementState)     &&

//             TryGetComponent(out _glidingMovementState)     &&
//             TryGetComponent(out _slidingMovementState)     &&
//             TryGetComponent(out _gravityPullMovementState) &&
//             TryGetComponent(out _verticalGravityFieldPushMovementState) &&

//             TryGetComponent(out _jumpGroundMovementState)   &&
//             TryGetComponent(out _jumpDoubleMovementState)   &&
//             TryGetComponent(out _jumpJumpableMovementState) &&
            
//             TryGetComponent(out _spikeDamagedMovementState) &&
//             transform.parent.GetChild(0).TryGetComponent(out _teleportingMovementState);

//         if (!areAllHere)
//         {
//             Debug.LogError("Some movement state is missing. Check the " + gameObject.name + " or the script itself");
//         }

//         _areAbilitiesActive = true;

//         PlayerEventsContainer.EventEntryNewMovementState += OnEntryNewMovementState;
//         PlayerEventsContainer.EventFinishedTeleporting += RestorePreviousState;

//         PlayerEventsContainer.EventDeath += OnPlayerDeath;
//         PlayerEventsContainer.EventSpawnEnd += OnPlayerSpawn;

//         PlayerQueriesContainer.FuncPreviousStateType += GetPreviousStateType;
//     }

//     private void OnDestroy()
//     { 
//         PlayerEventsContainer.EventEntryNewMovementState -= OnEntryNewMovementState;
//         PlayerEventsContainer.EventFinishedTeleporting -= RestorePreviousState;

//         PlayerEventsContainer.EventDeath -= OnPlayerDeath;
//         PlayerEventsContainer.EventSpawnEnd -= OnPlayerSpawn;

//         PlayerQueriesContainer.FuncPreviousStateType -= GetPreviousStateType;
//     }

//     private void Start()
//     {
//         _currentState = PlayerQueriesContainer.QueryInitialMovementState();
//         _playerController = PlayerQueriesContainer.QueryPlayerControllerInstance();
//     }

//     private void OnEntryNewMovementState(PlayerMovementStateType newStateType)
//     {
//         _previousStateType = _currentStateType;
//         _changesCountOneFrame++;

//         if (_changesCountOneFrame > MAX_CHANGES_COUNT_IN_ONE_FRAME)
//         {
//             Debug.LogError("Changes in one frame exceeded max count");
//             Debug.Break();
//             return;
//         }

//         _currentState.OnTransition();
//         SetCurrentStateFromStateType(newStateType, out bool shouldReturn);
//         if (shouldReturn)
//         {
//             return;
//         }

//         _currentStateType = newStateType;

//         _currentState.OnEntry();
//         if (_currentState.CheckForTransitions())
//         {
//             return;
//         }

//         _currentState.GetDisplacement(out _currentDisplacement);
//         _playerController.ApplyDisplacement(_currentDisplacement);
//     }

//     /// <summary>
//     /// Almost the same as OnNewMovementState, except that is does not call OnEntry();
//     /// </summary>
//     private void RestorePreviousState()
//     {
//         SetCurrentStateFromStateType(_previousStateType, out bool shouldReturn);
//         if (shouldReturn)
//         {
//             Debug.LogError("returning on restoring");
//             return;
//         }       

//         _currentStateType = _previousStateType;

//         if (_currentState.CheckForTransitions())
//         {
//             return;
//         }

//         _currentState.GetDisplacement(out _currentDisplacement);
//         _playerController.ApplyDisplacement(_currentDisplacement);
//     }

//     private void Update()
//     { 
//         if (Time.timeScale == 0f || !_areAbilitiesActive)
//         {
//             return;
//         }

//         if (_currentState.CheckForTransitions())
//         {
//             PlayerEventsContainer.EventFinalFrameMovementState?.Invoke(_currentStateType);
//             //System.GC.Collect();
//             return;
//         }

//         _currentState.GetDisplacement(out _currentDisplacement);
//         _playerController.ApplyDisplacement(_currentDisplacement);
//     }

//     private void OnTeleportingMovementEntry()
//     {
//         _previousStateType = _currentStateType;
//         _currentState = _teleportingMovementState;
//         _currentStateType = PlayerMovementStateType.Teleporting;
//         _currentState.OnEntry();
//     }

//     private void OnPlayerDeath()
//     {
//         _areAbilitiesActive = false;
//     }

//     private void OnPlayerSpawn()
//     {
//         _areAbilitiesActive = true;
//         _currentState = _idleMovementState;
//         _currentStateType = PlayerMovementStateType.Idle;
//         PlayerEventsContainer.EventFinalFrameMovementState?.Invoke(_currentStateType);
//     }

//     private void SetCurrentStateFromStateType(PlayerMovementStateType stateType, out bool shouldReturn)
//     { 
//         switch (stateType)
//         { 
//             case PlayerMovementStateType.Idle:
//                 _currentState = _idleMovementState;
//                 break;
//             case PlayerMovementStateType.Dash:
//                 _currentState = _dashMovementState;
//                 break;
//             case PlayerMovementStateType.Walking:
//                 _currentState = _walkingMovementState;
//                 break;
//             case PlayerMovementStateType.Falling:
//                 _currentState = _fallingMovementState;
//                 break;
//             case PlayerMovementStateType.Gliding:
//                 _currentState = _glidingMovementState;
//                 break;
//             case PlayerMovementStateType.Sliding:
//                 _currentState = _slidingMovementState;
//                 break;
//             case PlayerMovementStateType.GravityPull:
//                 _currentState = _gravityPullMovementState;
//                 break;
//             case PlayerMovementStateType.VerticalGravityFieldPush:
//                 _currentState = _verticalGravityFieldPushMovementState;
//                 break;
//             case PlayerMovementStateType.GroundJumping:
//                 _currentState = _jumpGroundMovementState;
//                 break;
//             case PlayerMovementStateType.DoubleJumping:
//                 _currentState = _jumpDoubleMovementState;
//                 break;
//             case PlayerMovementStateType.JumpableJumping:
//                 _currentState = _jumpJumpableMovementState;
//                 break;
//             case PlayerMovementStateType.SpikeDamaged:
//                 _currentState = _spikeDamagedMovementState;
//                 break;
//             case PlayerMovementStateType.Teleporting:
//                 OnTeleportingMovementEntry();
//                 shouldReturn = true;
//                 return;
//         }
//         shouldReturn = false;
//     }

//     private PlayerMovementStateMachine GetAbilitiesControllerInstance()
//     {
//         return this;
//     }

//     private PlayerMovementStateType GetPreviousStateType()
//     {
//         return _previousStateType;
//     }

//     private void LateUpdate()
//     {
//         _changesCountOneFrame = 0;
//     }

//     private void Log(string msg)
//     {
//         Debug.Log("StateMachine: " + msg);
//     }
// }