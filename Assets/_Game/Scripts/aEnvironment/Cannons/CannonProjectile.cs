// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// using SNG.DataStructures;

// /// <summary>
// /// Slightly copies CharacterJumpOverride.
// /// </summary>
// public class CannonProjectile : MonoBehaviour, IPoolable
// {
//     [Header("Projectile")]
//     [Space]
//     [SerializeField]
//     private float _moveSpeed;

//     [SerializeField]
//     private int _damageAmount;

//     [Header("GravityPull")]
//     [Space]
//     [SerializeField]
//     private float _pullSpeed;

//     [SerializeField]
//     private float _maxAlignSpeed;

//     private enum State
//     {
//         Despawned,
//         Flying,
//         HittedPlayer
//     }

//     private float _lifeTimeMax;
//     private Vector3 _direction;
//     private State _state;

//     public void OnSpawn(float lifeTimeMax, Vector3 directionArg)
//     {
//         _lifeTimeMax = lifeTimeMax;
//         _direction = directionArg;
//         gameObject.SetActive(true);
//         StartCoroutine(MoveUpdate());
//     }

//     public void OnDespawn()
//     {
//         _state = State.Despawned;
//         gameObject.SetActive(false);
//     }

//     private IEnumerator MoveUpdate()
//     {
//         _state = State.Flying;
//         float lifeTime = 0;
//         while (lifeTime < _lifeTimeMax)
//         {
//             lifeTime += Time.deltaTime;
//             switch (_state)
//             {
//                 case State.Flying:
//                     transform.Translate(_direction * _moveSpeed * Time.deltaTime, Space.Self);
//                     break;
//                 default:
//                     yield break;
//             }
//             yield return null;
//         }
//         if (_state == State.Flying)
//         {
//             PoolingDelegatesContainer.EventDespawnProjectile?.Invoke(this);
//         }
//     }

//     public void OnProjectileTriggerEnter(Collider2D other)
//     {
//         if (_state != State.Flying)
//         {
//             return;
//         }

//         if (other.gameObject.layer == LayersContainer.PLAYER_COLLISION_LAYER)
//         {
//             if (other.TryGetComponent(out PlayerHealth playerHealth))
//             {
//                 //playerHealth.TakeDamageFromPoint(DamageStrength.Weak, transform.position);
//                 _state = State.HittedPlayer;
//                 PoolingDelegatesContainer.EventDespawnProjectile?.Invoke(this);
//                 return;
//             }
//         }
//         else if (other.gameObject.layer == LayersContainer.LINE_THROUGH_PORTAL_LAYER
//             &&
//             other.gameObject.CompareTag(TagsContainer.GRAVITY_FIELD_TAG))
//         {

//             if (!other.TryGetComponent(out _enteredFieldBuffer))
//             {
//                 Debug.LogError("The object on WallRunZoneLayer does not have wallRunZoneComponent");
//             }

//             _enteredFieldSegments.Add(_enteredFieldBuffer);

//             _state = State.AffectedByGravityField;
//             _currentActiveSegment = _enteredFieldBuffer;
//             _currentActiveSegment.GetProjectileDirection(out _direction);
//         }
//         else if (other.gameObject.layer == LayersContainer.USUAL_PLATFORMS_LAYER
//         || other.gameObject.layer == LayersContainer.PUZZLE_ITEM_LAYER) // do not forget to review
//         {
//             _state = State.HittedPlayer;
//             PoolingDelegatesContainer.EventDespawnProjectile?.Invoke(this);
//             return;
//         }
//     }

//     public void OnProjectileTriggerExit(Collider2D other)
//     {
//         if (_state != State.Flying && _state != State.AffectedByGravityField)
//         {
//             return;
//         }

//         if (other.gameObject.layer == LayersContainer.LINE_THROUGH_PORTAL_LAYER
//             &&
//             other.gameObject.CompareTag(TagsContainer.GRAVITY_FIELD_TAG))
//         {

//             if (!other.TryGetComponent(out _enteredFieldBuffer))
//             {
//                 Debug.LogError("The object on WallRunZoneLayer does not have wallRunZoneComponent");
//             }

//             _enteredFieldSegments.Remove(_enteredFieldBuffer);
//             if (_currentActiveSegment.GetInstanceID() == _enteredFieldBuffer.GetInstanceID())
//             {
//                 if (_enteredFieldSegments.Count > 0)
//                 {
//                     _currentActiveSegment = _enteredFieldSegments[0];
//                     _state = State.Flying;
//                 }
//             }
//         }
//     }

//     public void OnPlayerJump()
//     {
//         _state = State.PlayerJumped;
//     }

//     public void OnJumpableReactionEnd()
//     {
//         PoolingDelegatesContainer.EventDespawnProjectile?.Invoke(this);
//     }

//     public void StartTeleporting(Portal destination)
//     {
//         transform.position = destination.transform.position;
//     }

//     public Transform GetTransform()
//     {
//         return transform;
//     }
// }