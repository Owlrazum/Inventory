// using UnityEngine;

// [RequireComponent(typeof(BoxCollider2D))]
// public class PuzzleItemInteractZone : MonoBehaviour
// { 
//     private BoxCollider2D _triggerCollider;
//     private PuzzleItem _main;

//     private void Awake()
//     {
//         if (!transform.parent.TryGetComponent(out _main))
//         {
//             Debug.LogError("Interact zone should have puzzleItem in its parent");
//         }

//         TryGetComponent(out _triggerCollider);
//         if (!_triggerCollider.isTrigger)
//         {
//             Debug.LogError("collider should be set to trigger");
//         }
//     }

//     public void DisableTriggerZone()
//     { 
//         _triggerCollider.enabled = false;
//     }

//     public void EnableTriggerZone()
//     { 
//         _triggerCollider.enabled = true;
//     }

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         _main.OnInteractTriggerEnter2D(other);
//     }

//     private void OnTriggerExit2D(Collider2D other)
//     {
//         _main.OnInteractTriggerExit2D(other);
//     }
// }