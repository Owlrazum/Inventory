using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleButton : PuzzleCause
{
    [SerializeField]
    [Tooltip("Or change the color on press")]
    private Transform _moveOnPressPart;

    [SerializeField]
    private float _buttonPressTime;

    [SerializeField]
    private bool _shouldApplyOffsetOnPressState;

    [SerializeField]
    private Vector3 _localOffsetToPressState;

    [SerializeField]
    private bool _shouldChangeMaterialOnPressState;

    [SerializeField]
    private Material _idleMaterial;

    [SerializeField]
    private Material _pressMaterial;

    private enum State
    {
        NotPressed,
        Pressed
    }

    private MeshRenderer _meshRenderer;

    private State _state;
    private Vector3 _notPressedLocalPosition;
    private Vector3 _pressedLocalPosition;

    private bool _wasPressedOnce;

    private IEnumerator _currentSequence;
    

    protected override void Awake()
    {
        base.Awake();
        _moveOnPressPart.TryGetComponent(out _meshRenderer);

        _notPressedLocalPosition = _moveOnPressPart.localPosition;
        _pressedLocalPosition = _notPressedLocalPosition + _localOffsetToPressState;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_state == State.Pressed)
        {
            return;
        }

        if (_type == PuzzleCauseType.OneWayOneTime && _wasPressedOnce)
        {
            return;
        }

        if (other.gameObject.layer == LayersContainer.PLAYER_COLLISION_LAYER ||
            other.gameObject.layer == LayersContainer.PROJECTILES_LAYER ||
            other.gameObject.layer == LayersContainer.PUZZLE_ITEM_LAYER)
        {
            if (_currentSequence != null)
            {
                StopCoroutine(_currentSequence);
            }
            _currentSequence = PressSequence();
            StartCoroutine(_currentSequence);
        }
    }

    protected override bool OnFinishedReaction()
    {
        if (base.OnFinishedReaction())
        {
            _currentSequence = UnPressSequence();
            StartCoroutine(_currentSequence);
        }

        return false;
    }

    private IEnumerator PressSequence()
    {
        _wasPressedOnce = true;
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / _buttonPressTime;
            if (_shouldApplyOffsetOnPressState)
            {
                _moveOnPressPart.localPosition = Vector3.Lerp(_notPressedLocalPosition, _pressedLocalPosition, lerpParam);
            }
            if (_shouldChangeMaterialOnPressState)
            {
                _meshRenderer.material.Lerp(_idleMaterial, _pressMaterial, lerpParam);
            }
            yield return null;
        }

        _state = State.Pressed;
        NotifyReactables();
        _currentSequence = null;
    }

    private IEnumerator UnPressSequence()
    {
        float lerpParam = 0;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / _buttonPressTime;
            if (_shouldApplyOffsetOnPressState)
            {
                _moveOnPressPart.localPosition = Vector3.Lerp(_pressedLocalPosition, _notPressedLocalPosition, lerpParam);
            }
            if (_shouldChangeMaterialOnPressState)
            {
                _meshRenderer.material.Lerp(_pressMaterial, _idleMaterial, lerpParam);
            }
            yield return null;
        }
        _state = State.NotPressed;
        _currentSequence = null;
    }
}