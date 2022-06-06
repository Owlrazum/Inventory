using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PuzzleHealthDestroyer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayersContainer.PUZZLE_HEALTH_LAYER ||
            other.gameObject.layer == LayersContainer.PUZZLE_ITEM_LAYER)
        {
            if (other.TryGetComponent(out PuzzleHealth puzzleHealth))
            {
                puzzleHealth.Obliterate();
            }
        }
    }
}