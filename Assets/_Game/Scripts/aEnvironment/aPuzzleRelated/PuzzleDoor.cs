using System;
using System.Collections;
using UnityEngine;

public class PuzzleDoor : PuzzleCauseReactionable
{
    [SerializeField]
    private Transform _modelTransform;

    [SerializeField]
    private float _openingTime;

    [SerializeField]
    private State _initialState;

    private enum OpeningType
    { 
        Rotation,
        Move
    }

    [SerializeField]
    private OpeningType _openingType;

    [Header("In case OpeningType is Rotation")]
    [Space]
    [SerializeField]
    private Vector3 _modelTransformClosedLocalEulerAngles;

    [SerializeField]
    private Vector3 _modelTransformOpenedLocalEulerAngles;

    [Header("In case two model transforms are required")]
    [SerializeField]
    private Transform _secondModelTransform;

    [SerializeField]
    private Vector3 _secondModelTransformClosedRotation;

    [SerializeField]
    private Vector3 _secondModelTransformOpenedRotation;

    [Header("In case OpeningType is Move")]
    [Space]
    [SerializeField]
    private Vector3 _modelTransformClosedLocalPos;

    [SerializeField]
    private Vector3 _modelTransformOpenedLocalPos;

    private enum State
    { 
        Opened,
        Closed
    }

    private State _state;
    private bool _debugOnlyOneSequence;

    private void Awake()
    {
        _state = _initialState;
    }

    private Func<bool> _finishingCallback;
    public override void ReactToOneWayOneTimePuzzleCause(Func<bool> finishingCallbackArg)
    {
        if (_debugOnlyOneSequence)
        {
            Debug.LogError("Logical Error");
            return;
        }

        _finishingCallback = finishingCallbackArg;
        switch (_state)
        { 
            case State.Opened:
                StartCoroutine(ClosingSequence());
                break;
            case State.Closed:
                StartCoroutine(OpeningSequence());
                break;
        }
    }

    public override void ReactToTwoWayMultipleTimesPuzzleCause(Func<bool> finishingCallbackArg)
    {
        _finishingCallback = finishingCallbackArg;
        switch (_state)
        { 
            case State.Opened:
                StartCoroutine(ClosingSequence());
                break;
            case State.Closed:
                StartCoroutine(OpeningSequence());
                break;
        }
    }

    private IEnumerator OpeningSequence()
    {
        _debugOnlyOneSequence = true;

        float lerpParam = 0;
        if (_openingType == OpeningType.Rotation)
        {
            Quaternion modelStartRotation = Quaternion.Euler(_modelTransformClosedLocalEulerAngles);
            Quaternion modelEndRotation = Quaternion.Euler(_modelTransformOpenedLocalEulerAngles);
            if (_secondModelTransform == null)
            {
                while (lerpParam < 1)
                {
                    lerpParam += Time.deltaTime / _openingTime;
                    _modelTransform.localRotation = Quaternion.Slerp(modelStartRotation, modelEndRotation, lerpParam);
                    yield return null;
                }
            }
            else
            {
                Quaternion secondModelStartRotation = Quaternion.Euler(_secondModelTransformClosedRotation);
                Quaternion secondModelEndRotation = Quaternion.Euler(_secondModelTransformOpenedRotation);
                while (lerpParam < 1)
                {
                    lerpParam += Time.deltaTime / _openingTime;
                    _modelTransform.localRotation = 
                        Quaternion.Slerp(modelStartRotation, modelEndRotation, lerpParam);
                    _secondModelTransform.localRotation = 
                        Quaternion.Slerp(secondModelStartRotation, secondModelEndRotation, lerpParam);
                    yield return null;
                }
            }
        }
        else
        {
            while (lerpParam < 1)
            {
                lerpParam += Time.deltaTime / _openingTime;
                _modelTransform.localPosition =
                    Vector3.Lerp(_modelTransformClosedLocalPos, _modelTransformOpenedLocalPos, lerpParam);
                yield return null;
            }
        }

        _state = State.Opened;
        _finishingCallback.Invoke();
    }

    private IEnumerator ClosingSequence()
    {
        _debugOnlyOneSequence = true;
        float lerpParam = 0;
        if (_openingType == OpeningType.Rotation)
        {
            Quaternion modelStartRotation = Quaternion.Euler(_modelTransformOpenedLocalEulerAngles);
            Quaternion modelEndRotation = Quaternion.Euler(_modelTransformClosedLocalEulerAngles);
            if (_secondModelTransform == null)
            {
                while (lerpParam < 1)
                {
                    lerpParam += Time.deltaTime / _openingTime;
                    _modelTransform.localRotation = Quaternion.Slerp(modelStartRotation, modelEndRotation, lerpParam);
                    yield return null;
                }
            }
            else
            {
                Quaternion secondModelStartRotation = Quaternion.Euler(_secondModelTransformOpenedRotation);
                Quaternion secondModelEndRotation = Quaternion.Euler(_secondModelTransformClosedRotation);
                while (lerpParam < 1)
                {
                    lerpParam += Time.deltaTime / _openingTime;
                    _modelTransform.localRotation = 
                        Quaternion.Slerp(modelStartRotation, modelEndRotation, lerpParam);
                    _secondModelTransform.localRotation = 
                        Quaternion.Slerp(secondModelStartRotation, secondModelEndRotation, lerpParam);
                    yield return null;
                }
            }
        }
        else
        {
            while (lerpParam < 1)
            {
                lerpParam += Time.deltaTime / _openingTime;
                _modelTransform.localPosition =
                    Vector3.Lerp(_modelTransformOpenedLocalPos, _modelTransformClosedLocalPos, lerpParam);
                yield return null;
            }
        }

        _state = State.Closed;
        _debugOnlyOneSequence = false;
        _finishingCallback.Invoke();
    }
}