using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera _renderingCamera;

    private void Awake()
    {
        QueriesContainer.FuncTransformDirectionFromCameraSpace += TransformDirectionFromCameraSpace;
    }

    private void OnDestroy()
    {
        QueriesContainer.FuncTransformDirectionFromCameraSpace -= TransformDirectionFromCameraSpace;
    }

    private Vector3 TransformDirectionFromCameraSpace(Vector3 input)
    {
        input = _renderingCamera.transform.TransformDirection(input);
        input.y = 0;
        return input;
    }
}
