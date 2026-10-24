using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 10;

    private int currentHp;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    private void Awake()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(int amount)
    {
        // Ignore hits after death so Died fires only once
        if (amount <= 0 || currentHp <= 0) return;

        currentHp = Mathf.Max(currentHp - amount, 0);
        HealthChanged?.Invoke(currentHp, maxHp);
        if (currentHp == 0) Died?.Invoke();
    }
}
