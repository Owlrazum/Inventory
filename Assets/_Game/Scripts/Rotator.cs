using System.Collections;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private float speedDeg = 30;

    private bool _isActive;

    private void Awake()
    {
        _isActive = true;
    }

    private IEnumerator _rotateSequence;

    public void OnLevelSegmentEnable()
    {
        if (!_isActive)
        {
            return;
        }

        _rotateSequence = RotateSequence();
        StartCoroutine(_rotateSequence);
    }

    public void OnLevelSegmentDisable()
    {
        if (!_isActive)
        {
            return;
        }
        
        StopCoroutine(_rotateSequence);
    }

    private IEnumerator RotateSequence()
    {
        while (true)
        { 
            transform.Rotate(Vector3.right * speedDeg * Time.deltaTime);
            yield return null;
        }
    }
}
