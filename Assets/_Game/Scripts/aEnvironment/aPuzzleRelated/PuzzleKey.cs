// using System.Collections;
// using UnityEngine;

// public class PuzzleKey : PuzzleItem
// {
//     [SerializeField]
//     private int _openingID;

//     [SerializeField]
//     private float _openingMoveSpeed = 1;

//     public int GetOpeningID()
//     {
//         return _openingID;
//     }

//     public void OpenLock(PuzzleLock puzzleLock)
//     {
//         print("Starting opening");
//         StartCoroutine(OpeningSequence(puzzleLock));
//     }

//     private IEnumerator OpeningSequence(PuzzleLock puzzleLock)
//     {
//         float lerpParam = 0;
//         float distance = (puzzleLock.transform.position - transform.position).magnitude;
//         float lerpTime = distance / _openingMoveSpeed;
//         transform.parent = puzzleLock.transform;
//         Vector2 initialPos = transform.localPosition;
//         while (lerpParam < 1)
//         {
//             lerpParam += Time.deltaTime / lerpTime;
//             transform.localPosition = Vector2.Lerp(initialPos, Vector2.zero, CustomMath.EaseOut(lerpParam));
//             yield return null;
//         }

//         puzzleLock.Open();
//     }
// }