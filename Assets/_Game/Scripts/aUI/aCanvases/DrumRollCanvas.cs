using System.Collections;
using UnityEngine;

// TODO apply to scene and further make level flow
[RequireComponent(typeof(CanvasGroup), typeof(Canvas))]
public class DrumRollCanvas : MonoBehaviour
{
    [SerializeField]
    private float _drumRollTime = 1;

    [SerializeField]
    [Range(0, 0.2f)]
    private float _animationReturnToDefaultFraction = 0.2f;

    private CanvasGroup _groupAlpha;

    private float _upLerpSpeed;
    private float _downLerpSpeed;

    private void Awake()
    {
        float _animationFraction = 1 - _animationReturnToDefaultFraction;
        _upLerpSpeed = 1 / (_drumRollTime * _animationFraction);
        _downLerpSpeed = 1 / (_drumRollTime * _animationReturnToDefaultFraction);

        TryGetComponent(out _groupAlpha);
        _groupAlpha.alpha = 0;

        GameDelegatesContainer.StartDrumRoll += StartDrumRoll;
    }

    private void OnDestroy()
    { 
        GameDelegatesContainer.StartDrumRoll -= StartDrumRoll;
    }

    private void StartDrumRoll()
    {
        StartCoroutine(DrumRollSequence());
    }

    private IEnumerator DrumRollSequence()
    {
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += _upLerpSpeed * Time.deltaTime;
            _groupAlpha.alpha = Mathf.Lerp(0, 1, lerpParam);
            yield return null;
        }

        lerpParam = 0;
        while (lerpParam < 1)
        { 
            lerpParam += _upLerpSpeed * Time.deltaTime;
            _groupAlpha.alpha = Mathf.Lerp(1, 0, lerpParam);
            yield return null;
        }

        GameDelegatesContainer.EventDrumRollCompleted?.Invoke();
    }
}