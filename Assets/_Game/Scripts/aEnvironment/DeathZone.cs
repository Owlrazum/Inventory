// using UnityEngine;

// public class DeathZone : MonoBehaviour
// {
//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.gameObject.layer != LayersContainer.PLAYER_HEALTH_HIT_LAYER &&
//             other.gameObject.layer != LayersContainer.PUZZLE_ITEM_LAYER &&
//             other.gameObject.layer != LayersContainer.PUZZLE_HEALTH_LAYER)
//         {
//             return;
//         }

//         if (other.TryGetComponent(out PlayerHealth playerHealth))
//         {
//             playerHealth.TakeDamageFromDirection(DamageDirection.Down, DamageStrength.Death);
//         }

//         if (other.TryGetComponent(out PuzzleHealth puzzleHealth))
//         {
//             puzzleHealth.Obliterate();
//         }
//     }
// }
