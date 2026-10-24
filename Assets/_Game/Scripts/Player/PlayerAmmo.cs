using System;
using UnityEngine;

public class PlayerAmmo : MonoBehaviour
{
    [SerializeField, Min(0)] private int startAmmo = 20;

    public event Action<int> AmmoChanged;

    public int Current { get; private set; }

    private void Awake()
    {
        Current = startAmmo;
    }

    public bool TryConsume()
    {
        if (Current <= 0)
        {
            Debug.Log("Ammo: empty");
            return false;
        }

        SetCurrent(Current - 1);
        return true;
    }

    // no cap, ammo pickups can stack without limit
    public void Add(int amount)
    {
        if (amount <= 0) return;

        SetCurrent(Current + amount);
    }

    private void SetCurrent(int value)
    {
        Current = value;
        Debug.Log($"Ammo: {Current}");
        AmmoChanged?.Invoke(Current);
    }
}
