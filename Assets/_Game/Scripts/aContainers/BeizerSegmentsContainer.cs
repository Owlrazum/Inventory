using UnityEngine;

public class BeizerSegmentsContainer : MonoBehaviour
{
    [SerializeField]
    private BeizerSegment _wallRunBeizerSegment;

    private void Awake()
    {
        QueriesContainer.FuncWallRunBeizerSegment += GetWallRunBeizerSegment;
    }

    private void OnDestroy()
    { 
        QueriesContainer.FuncWallRunBeizerSegment -= GetWallRunBeizerSegment;
    }

    private BeizerSegment GetWallRunBeizerSegment()
    {
        return _wallRunBeizerSegment;
    }
}