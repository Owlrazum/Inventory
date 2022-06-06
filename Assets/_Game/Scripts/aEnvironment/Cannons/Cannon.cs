// using System.Collections;
// using UnityEngine;

// public class Cannon : MonoBehaviour, ILevelSegmentable
// { 
//     [SerializeField]
//     private Transform _projectileSpawnTransform;

//     [SerializeField]
//     private float _timeOfFire;

//     [SerializeField]
//     private float _lifeTimeOfBullets;

//     [SerializeField]
//     [Tooltip("Debug only")]
//     private bool _isLevelSegmentRequired = true;

//     private bool _isActive;

//     private float _time;

//     private IEnumerator _shootingSequence;

//     private void Awake()
//     {
//         _isActive = true;

//         if (!_isLevelSegmentRequired)
//         { 
//             _shootingSequence = ShootingSequence();
//             StartCoroutine(_shootingSequence);
//         }
//     }

//     public void OnLevelSegmentEnable()
//     {
//         if (!_isActive)
//         {
//             return;
//         }

//         _shootingSequence = ShootingSequence();
//         StartCoroutine(_shootingSequence);
//     }

//     public void OnLevelSegmentDisable()
//     {
//         if (!_isActive)
//         {
//             return;
//         }
        
//         StopCoroutine(_shootingSequence);
//     }

//     private IEnumerator ShootingSequence()
//     {
//         while (true)
//         { 
//             _time += Time.deltaTime;
//             if (_time >= _timeOfFire)
//             {
//                 PoolingDelegatesContainer.EventSpawnProjectile?.Invoke(
//                     _projectileSpawnTransform.position, 
//                     _projectileSpawnTransform.right,
//                     _lifeTimeOfBullets
//                 );
//                 _time = 0;
//             }

//             yield return null;
//         }
//     }
// }