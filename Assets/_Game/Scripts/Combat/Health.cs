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

    // Ordinary damage (bullets, attacks); subclasses may block it
    public virtual void TakeDamage(int amount)
    {
        ApplyDamage(amount);
    }

    // Flashlight damage; not virtual so light always gets through
    public void TakeLightDamage(int amount)
    {
        ApplyDamage(amount);
    }

    protected void ApplyDamage(int amount)
    {
        // Ignore hits after death so Died fires only once
        if (amount <= 0 || currentHp <= 0) return;

        currentHp = Mathf.Max(currentHp - amount, 0);
        HealthChanged?.Invoke(currentHp, maxHp);
        if (currentHp == 0) Died?.Invoke();
    }

    // Restores a saved value (e.g. carried over from the previous level); never kills
    public void SetCurrentHp(int value)
    {
        currentHp = Mathf.Clamp(value, 1, maxHp);
        HealthChanged?.Invoke(currentHp, maxHp);
    }

    public void Heal(int amount)
    {
        // Dead stay dead; full HP is a no-op so listeners never see a fake change
        if (amount <= 0 || currentHp <= 0 || currentHp >= maxHp) return;

        currentHp = Mathf.Min(currentHp + amount, maxHp);
        HealthChanged?.Invoke(currentHp, maxHp);
    }
}
