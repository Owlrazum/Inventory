// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using SNG.DataStructures;

// /// <summary>
// /// Currently picks up only puzzle item.
// /// </summary>
// public class PlayerPickUp : MonoBehaviour
// {
//     [SerializeField]
//     private Transform _pickUpPlace;

//     [SerializeField]
//     private float _pickUpTime;

//     private ListID<PuzzleItem> _interactableItems;
//     private PuzzleItem _nearestItem;
//     private PuzzleItem _holdedItem;

//     private IEnumerator _inputCheckSequence;
//     private IEnumerator _nearestCheckSequence;

//     private PickUpItemCommand _pickUpCommand;

//     private Transform _playerTransform;

//     private enum PickUpState
//     { 
//         Empty,
//         PickingUp,
//         Holding,
//     }

//     private PickUpState _pickUpState;

//     private void Awake()
//     {
//         _interactableItems = new ListID<PuzzleItem>();
        
//         PlayerEventsContainer.EventPuzzleItemTriggerEnter += OnPuzzleItemTriggerEnter;
//         PlayerEventsContainer.EventPuzzleItemTriggerExit += OnPuzzleItemTriggerExit;
//     }

//     private void OnDestroy()
//     { 
//         PlayerEventsContainer.EventPuzzleItemTriggerEnter -= OnPuzzleItemTriggerEnter;
//         PlayerEventsContainer.EventPuzzleItemTriggerExit -= OnPuzzleItemTriggerExit;
//     }

//     private void Start()
//     {
//         _pickUpCommand = InputQueriesContainer.QueryPickUpItemCommand();
//         _playerTransform = PlayerQueriesContainer.QueryTransform();
//     }

//     private void OnPuzzleItemTriggerEnter(PuzzleItem item)
//     {
//         _interactableItems.Add(item);
//         if (_interactableItems.Count == 1)
//         {
//             if (_inputCheckSequence == null || _nearestCheckSequence == null)
//             { 
//                 _inputCheckSequence = InputCheckSequence();
//                 StartCoroutine(_inputCheckSequence);

//                 _nearestCheckSequence = NearestCheckSequence();
//                 StartCoroutine(_nearestCheckSequence);
//             }
//         }

//         RecalculateNearestItem();
//     }

//     private void OnPuzzleItemTriggerExit(PuzzleItem item)
//     {
//         _interactableItems.Remove(item);
//         if (_interactableItems.Count == 0)
//         {
//             if (_inputCheckSequence != null || _nearestCheckSequence != null)
//             { 
//                 StopCoroutine(_inputCheckSequence);
//                 _inputCheckSequence = null;

//                 StopCoroutine(_nearestCheckSequence);
//                 _nearestCheckSequence = null;
//                 _nearestItem = null;
//                 _pickUpState = PickUpState.Empty;
//             }
//         }
//     }

//     private IEnumerator InputCheckSequence()
//     { 
//         while (true)
//         {
//             if (_pickUpCommand.IsTriggered)
//             {
//                 switch (_pickUpState)
//                 { 
//                     case PickUpState.Empty:
//                         PickUpCurrentItem();
//                         break;
//                     case PickUpState.Holding:
//                         StartReleasingCurrentItemByCommand();
//                         break;
//                 }
//             }
//             yield return null;
//         }
//     }

//     private IEnumerator NearestCheckSequence()
//     {
//         while (_interactableItems.Count > 0)
//         {
//             if (_interactableItems.Count == 1)
//             {
//                 _nearestItem = _interactableItems[0];
//             }
//             else
//             {
//                 RecalculateNearestItem();
//             }

//             yield return null;
//         }
//     }

//     private void RecalculateNearestItem()
//     {
//         float smallestDistance = -1;
//         for (int i = 0; i < _interactableItems.Count; i++)
//         {
//             float distance = (_interactableItems[i].transform.position - _playerTransform.position).sqrMagnitude;
//             if (smallestDistance == -1 || distance < smallestDistance)
//             {
//                 smallestDistance = distance;
//                 _nearestItem = _interactableItems[i];
//             }
//         }
//     }

//     private void PickUpCurrentItem()
//     {
//         _pickUpState = PickUpState.PickingUp;
//         _nearestItem.OnPickUp(out Quaternion targetRotation);
//         _nearestItem.transform.parent = _pickUpPlace;
//         _holdedItem = _nearestItem;
//         FeedbackEventsContainer.EventStartedPickingUpPuzzleItem?.Invoke();
//         StartCoroutine(PickUpSequence(targetRotation));
//     }

//     private IEnumerator PickUpSequence(Quaternion targetRotation)
//     {
//         float lerpParam = 0;
//         while (lerpParam < 1)
//         {
//             lerpParam += Time.deltaTime / _pickUpTime;
            
//             _holdedItem.transform.rotation = Quaternion.Slerp(
//                 _holdedItem.transform.rotation, 
//                 targetRotation,
//                 lerpParam
//             );
//             yield return null;
//         }
//         _pickUpState = PickUpState.Holding;
//     }

//     private void StartReleasingCurrentItemByCommand()
//     {
//         _pickUpState = PickUpState.Empty;
//         _holdedItem.OnRelease();
//         _holdedItem.TurnOnPhysics();
//         _holdedItem = null;
//         FeedbackEventsContainer.EventStartedReleasingPuzzleItem?.Invoke();
//     }

//     public void OnPuzzleLockInteractionEnter(PuzzleLock puzzleLock)
//     {
//         PuzzleKey key = _holdedItem as PuzzleKey;
//         if (key == null)
//         {
//             return;
//         }

//         if (key.GetOpeningID() == puzzleLock.GetOpeningID())
//         {
//             _pickUpState = PickUpState.Empty;
//             key.OpenLock(puzzleLock);
//             _holdedItem = null;
//             FeedbackEventsContainer.EventStartedReleasingPuzzleItem?.Invoke();
//         }
//     }
// }