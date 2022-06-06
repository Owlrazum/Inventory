// using System.Collections;
// using UnityEngine;

// public class PuzzleLock : PuzzleCause
// {
    // [SerializeField]
    // private int _openingID;

    // [SerializeField]
    // private float _openingTime;

    // [SerializeField]
    // private Material _lockedMaterial;

    // [SerializeField]
    // private Material _openedMaterial;

    // private MeshRenderer _meshRenderer;

    // private enum OpenState
    // {
    //     Closed,
    //     Opened
    // }

    // private OpenState _openState;

    // protected override void Awake()
    // {
    //     base.Awake();
    //     if (!transform.GetChild(0).TryGetComponent(out _meshRenderer))
    //     {
    //         Debug.LogError("The lock should have model as a first child");
    //     }
    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (_openState == OpenState.Opened)
    //     {
    //         return;
    //     }

    //     if (other.gameObject.layer == LayersContainer.PUZZLE_ITEM_LAYER)
    //     {
    //         if (other.TryGetComponent(out PuzzleKey key))
    //         {
    //             if (key.GetOpeningID() == _openingID)
    //             {
    //                 StartCoroutine(OpeningSequence());
    //             }
    //         }
    //         return;
    //     }

    //     if (other.gameObject.layer == LayersContainer.PLAYER_COLLISION_LAYER)
    //     {
    //         if (other.transform.parent.TryGetComponent(out PlayerPickUp _pickUp))
    //         {
    //             _pickUp.OnPuzzleLockInteractionEnter(this);
    //         }
    //         else
    //         {
    //             Debug.LogError("Pick up component was not found");
    //         }
    //     }
    // }

    // public void Open()
    // {
    //     _openState = OpenState.Opened;
    //     StartCoroutine(OpeningSequence());
    // }

    // private IEnumerator OpeningSequence()
    // {
    //     float lerpParam = 0;
    //     while (lerpParam < 1)
    //     {
    //         lerpParam += Time.deltaTime / _openingTime;
    //         _meshRenderer.material.Lerp(_lockedMaterial, _openedMaterial, lerpParam);
    //         yield return null;
    //     }
    //     NotifyReactables();
    // }

    // public int GetOpeningID()
    // {
    //     return _openingID;
    // }
//}