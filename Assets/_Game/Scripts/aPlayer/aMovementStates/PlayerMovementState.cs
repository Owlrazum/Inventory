// using UnityEngine;

// /// <summary>
// /// For each state its exit conditions, and to which state transition should occur should be defined.
// /// Abilities controller is responsible for checks and displacement application.
// /// </summary>
// public abstract class PlayerMovementState : MonoBehaviour
// {
//     private PlayerCharacter _playerCharacterData;
//     protected PlayerCharacter _playerCharacter
//     {
//         get
//         {
//             if (_playerCharacterData == null)
//             {
//                 _playerCharacterData = PlayerQueriesContainer.QueryPlayerCharacterInstance();
// #if UNITY_EDITOR
//                 if (_playerCharacterData == null)
//                 {
//                     Debug.LogError("_playerCharacter field could not be accessed during Awake()");
//                 }
// #endif
//             }
//             return _playerCharacterData;
//         }
//     }

//     public virtual void OnEntry()
//     {

//     }

//     /// <summary>
//     /// In case CheckForTransitions returns true in PlayerMovementStateMachine.
//     /// </summary>
//     public virtual void OnTransition()
//     { 
        
//     }

//     /// <summary>
//     /// Returns whether transition happened;
//     /// </summary>
//     public virtual bool CheckForTransitions()
//     {
//         if (PlayerQueriesContainer.QueryIsSpikeDamagedMovementTriggering())
//         { 
//             PlayerEventsContainer.EventEntryNewMovementState(PlayerMovementStateType.SpikeDamaged);
//             return true;
//         }
        
//         if (PlayerQueriesContainer.QueryIsGravityPullTriggering())
//         {
//             PlayerEventsContainer.EventEntryNewMovementState(PlayerMovementStateType.GravityPull);
//             return true;
//         }
//         return false;
//     }
//     /// <summary>
//     /// Serves as state's act
//     /// </summary>
//     public abstract void GetDisplacement(out Vector2 displacement);
// }