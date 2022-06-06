using System.Collections;

using UnityEngine;

using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _renderingCamera;

    [SerializeField]
    private CinemachineVirtualCamera _vcam;

    private CinemachineFramingTransposer _framingTransposer;

    private Transform _cameraLookPoint;

    private BoxCollider2D _boundingBox;

    private IEnumerator _lookingCoroutine;

    private void Awake()
    {
        _framingTransposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();

        QueriesContainer.FuncTransformDirectionFromCameraSpace += TransformDirectionFromCameraSpace;
    }

    private void OnDestroy()
    {
        QueriesContainer.FuncTransformDirectionFromCameraSpace -= TransformDirectionFromCameraSpace;
    }

    private void Start()
    {
        _cameraLookPoint = PlayerQueriesContainer.QueryTransform();
        _vcam.Follow = _cameraLookPoint;
    }

    private Vector3 TransformDirectionFromCameraSpace(Vector3 input)
    {
        input = _renderingCamera.transform.TransformDirection(input);
        input.y = 0;
        return input;
    }
}
