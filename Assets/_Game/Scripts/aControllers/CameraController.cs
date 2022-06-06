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
    }

    private void OnDestroy()
    {
    }

    private void Start()
    {
        _cameraLookPoint = PlayerQueriesContainer.QueryTransform();
        _vcam.Follow = _cameraLookPoint;
    }
}
