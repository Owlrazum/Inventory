using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _renderingCamera;

    private void Awake()
    {
        QueriesContainer.FuncTransformDirectionFromCameraSpace += TransformDirectionFromCameraSpace;
        QueriesContainer.FuncTransformScreenPosToWorldPos += TransformScreenPosToWorldPos;
    }

    private void OnDestroy()
    {
        QueriesContainer.FuncTransformScreenPosToWorldPos -= TransformScreenPosToWorldPos;
    }

    private Vector3 TransformDirectionFromCameraSpace(Vector3 input)
    {
        input = _renderingCamera.transform.TransformDirection(input);
        input.y = 0;
        return input;
    }

    private Vector3 TransformScreenPosToWorldPos(Vector3 input)
    {
        return _renderingCamera.ScreenToWorldPoint(input);
    }
}
