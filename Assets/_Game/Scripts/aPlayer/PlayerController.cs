using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _rotationSpeedDeg;

    private CharacterController _characterController;

    private WalkCommand _walkCommand;
    private Vector3 _moveDirection;
    private bool _walkedThisFrame;

    private void Awake()
    {
        TryGetComponent(out _characterController);
        _moveDirection = Vector3.zero;

        PlayerQueriesContainer.FuncWalkedThisFrame += GetWalkedThisFrame;
    }

    private void OnDestroy()
    { 
        PlayerQueriesContainer.FuncWalkedThisFrame -= GetWalkedThisFrame;
    }

    private void Start()
    {
        _walkCommand = InputDelegatesContainer.QueryWalkCommand();
    }

    private void Update()
    {
        if (_walkCommand.Horizontal == 0 && _walkCommand.Vertical == 0)
        {
            _walkedThisFrame = false;
            return;
        }

        _moveDirection.x = _walkCommand.Horizontal;
        _moveDirection.z = _walkCommand.Vertical;

        _moveDirection = QueriesContainer.QueryTransformDirectionFromCameraSpace(_moveDirection);

        _characterController.Move(_moveDirection.normalized * _moveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            Quaternion.LookRotation(_moveDirection),
            _rotationSpeedDeg * Time.deltaTime    
        );

        _walkedThisFrame = true;
    }

    private bool GetWalkedThisFrame()
    {
        return _walkedThisFrame;
    }
}