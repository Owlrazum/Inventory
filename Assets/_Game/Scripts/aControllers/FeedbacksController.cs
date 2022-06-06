using System.Collections;
using UnityEngine;

public class FeedbacksController : MonoBehaviour
{
    [SerializeField]
    private Transform _particlesParent;

    [SerializeField]
    private Transform _audioSourcesParent;

    [SerializeField]
    private ParticleSystem _walkingStateParticles;

    [SerializeField]
    private ParticleSystem _groundJumpParticles;

    [SerializeField]
    private ParticleSystem _doubleJumpParticles;

    private Transform _playerTransformData;
    private Transform _playerTransform
    {
        get
        {
            if (_playerTransformData == null)
            {
                _playerTransformData = PlayerQueriesContainer.QueryTransform();
            }
            return _playerTransformData;
        }
    }

    // private PlayerMovementStateType _previousMovementState;

    // private void Awake()
    // {
    //     PlayerEventsContainer.EventFinalFrameMovementState += FinalFrameMovementState;
    // }

    // private void OnDestroy()
    // {
    //     PlayerEventsContainer.EventFinalFrameMovementState -= FinalFrameMovementState;
    // }

    // private void FinalFrameMovementState(PlayerMovementStateType newStateType)
    // {
    //     StopPreviousFeedbackIfNeeded();
    //     switch (newStateType)
    //     {
    //         case PlayerMovementStateType.Walking:
    //             _walkingStateParticles.transform.SetParent(_playerTransform, false);
    //             _walkingStateParticles.Play();
    //             break;
    //         case PlayerMovementStateType.GroundJumping:
    //             _groundJumpParticles.transform.SetParent(_playerTransform, false);
    //             _groundJumpParticles.Play();
    //             break;
    //         case PlayerMovementStateType.DoubleJumping:
    //             _doubleJumpParticles.transform.SetParent(_playerTransform, false);
    //             _doubleJumpParticles.Play();
    //             break;
    //     }

    //     _previousMovementState = newStateType;
    // }

    // private void StopPreviousFeedbackIfNeeded()
    // {
    //     switch (_previousMovementState)
    //     {
    //         case PlayerMovementStateType.Walking:
    //             _walkingStateParticles.transform.SetParent(_particlesParent, false);
    //             _walkingStateParticles.Stop();
    //             return;
    //         case PlayerMovementStateType.GroundJumping:
    //             _groundJumpParticles.transform.SetParent(_particlesParent, false);
    //             return;
    //         case PlayerMovementStateType.DoubleJumping:
    //             _doubleJumpParticles.transform.SetParent(_particlesParent, false);
    //             break;
    //     }
    // }
}