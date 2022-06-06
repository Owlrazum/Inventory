using UnityEngine;

[RequireComponent(typeof(IPuzzleHealth))]
public class PuzzleHealth : MonoBehaviour
{
    [SerializeField]
    private int _initialHealth;

    private IPuzzleHealth _callBackComponent;

    private int _currentHealth;

    private void Awake()
    {
        TryGetComponent(out _callBackComponent);
        _callBackComponent.AssignSpawnCallBack(OnSpawn);
    }

    private void OnSpawn()
    {
        _currentHealth = _initialHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0)
        {
            _callBackComponent.OnHealthLost();
        }
    }

    public void Obliterate()
    {
        _currentHealth = 0;
        _callBackComponent.OnHealthLost();
    }
}