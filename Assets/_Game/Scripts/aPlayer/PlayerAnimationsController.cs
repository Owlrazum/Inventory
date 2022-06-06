using UnityEngine;

public class PlayerAnimationsController : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    private void Awake()
    {
    }

    private void OnDestroy()
    {
    }


    private void LateUpdate()
    {
        _animator.SetBool("IsRun", PlayerQueriesContainer.QueryWalkedThisFrame());
    }

    private void Log(string msg)
    {
        Debug.Log("PlayerAnimationsController: " + msg);
    }
}