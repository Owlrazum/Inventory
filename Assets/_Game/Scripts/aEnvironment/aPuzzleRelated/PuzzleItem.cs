using System;
using System.Collections;

using UnityEngine;

/// <summary>
/// Note: in editor mode restores initial parent
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PuzzleItem : MonoBehaviour, IPuzzleHealth, IPoolable
{
    [SerializeField]
    private float _releaseForceAmount = 10;

    [SerializeField]
    private float _spawnTime = 1;

    [SerializeField]
    private float _depspawnTime = 1;

    private Action _spawnCallBack;
    private Action _despawnCallBack;

    private float _initialDepthPos;
    private Quaternion _initialRotation;

    private Rigidbody2D _rigidBody2D;
    private Collider2D _collider2D;
    private MeshRenderer _meshRenderer;
    private bool _isPickedUp;

    private PuzzleItemInteractZone _interactZone;

    private IEnumerator _deltaPosUpdateLoop;
    private Vector2 _currenVelocity;

    private void Awake()
    {
#if UNITY_EDITOR
        if (gameObject.layer != LayersContainer.PUZZLE_ITEM_LAYER)
        {
            Debug.LogError("Wrong layer");
        }
#endif   

        if (!transform.GetChild(0).TryGetComponent(out _interactZone))
        {
            Debug.LogError("The first child of Puzzle movable does not contain interactZone");
        }

        if (!transform.GetChild(1).TryGetComponent(out _meshRenderer))
        {
            Debug.LogError("The second child of Puzzle movable does not contain meshRenderer. " +
                "The model should be the seconds");
        }

        TryGetComponent(out _rigidBody2D);
        TryGetComponent(out _collider2D);
    }

    public void OnSpawn()
    {
        _initialDepthPos = transform.position.z;
        _initialRotation = transform.rotation;

        gameObject.SetActive(true);
        _spawnCallBack?.Invoke();

        StartCoroutine(SpawnAnimation());
    }

    private IEnumerator SpawnAnimation()
    {
        float lerpParam = 0;
        Color color = _meshRenderer.material.color;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / _spawnTime;
            color.a = lerpParam;
            _meshRenderer.material.color = color;
            yield return null;
        }
    }

    public void OnDespawn(Transform despawnParent)
    { 
        gameObject.SetActive(false);
        if (despawnParent != null)
        { 
            transform.parent = despawnParent;
        }
    }

    public void OnHealthLost()
    {
        StartCoroutine(DespawnSequence());
    }

    public void AssignSpawnCallBack(Action callBack)
    {
        _spawnCallBack = callBack;
    }

    public void AssignDespawnCallBack(Action callBack)
    {
        _despawnCallBack = callBack;
    }

    private IEnumerator DespawnSequence()
    {
        float lerpParam = 0;
        Color color = _meshRenderer.material.color;
        while (lerpParam < 1)
        {
            lerpParam += Time.deltaTime / _depspawnTime;
            color.a = 1 - lerpParam;
            _meshRenderer.material.color = color;
            yield return null;
        }

        if (_despawnCallBack != null)
        { 
            _despawnCallBack.Invoke();
        }
        else
        {
            OnDespawn(null);
        }
    }

    public void OnInteractTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayersContainer.PLAYER_COLLISION_LAYER)
        {
            return;
        }

        if (_isPickedUp)
        {
            Debug.LogError("LogicalError");
        }

        PlayerEventsContainer.EventPuzzleItemTriggerEnter?.Invoke(this);
    }

    public void OnInteractTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != LayersContainer.PLAYER_COLLISION_LAYER)
        {
            return;
        }

        if (!_isPickedUp)
        { 
            PlayerEventsContainer.EventPuzzleItemTriggerExit?.Invoke(this);
        }
    }

    public void OnPickUp(out Quaternion initialRotation)
    {
        initialRotation = _initialRotation;

        _rigidBody2D.isKinematic = true;
        _collider2D.enabled = false;
        //_rigidBody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        _isPickedUp = true;
        _interactZone.DisableTriggerZone();

        _deltaPosUpdateLoop = StoreDeltaPosUpdate();
        StartCoroutine(_deltaPosUpdateLoop);
    }

    public void OnRelease()
    {
        _isPickedUp = false;
        _interactZone.EnableTriggerZone();
        transform.position = new Vector3(transform.position.x, transform.position.y, _initialDepthPos);
        transform.rotation = _initialRotation;
        transform.parent = null;
    }

    public void TurnOnPhysics()
    { 
        _rigidBody2D.isKinematic = false;
        _collider2D.enabled = true;
        //_rigidBody2D.constraints = RigidbodyConstraints2D.None;

        StartCoroutine(ApplyForceOnFixedUpdate(_currenVelocity * _releaseForceAmount));
        StopCoroutine(_deltaPosUpdateLoop);
    }

    private IEnumerator StoreDeltaPosUpdate()
    {
        Vector2 previousPos = transform.position;
        while (true)
        {
            _currenVelocity = ((Vector2) transform.position - previousPos) / Time.deltaTime;
            previousPos = transform.position;
            yield return null;
        }
    }

    private IEnumerator ApplyForceOnFixedUpdate(Vector2 relativeForce)
    {
        yield return new WaitForFixedUpdate();
        _rigidBody2D.AddRelativeForce(relativeForce);
    }

    public Transform GetTransform()
    {
        return transform;
    }
}