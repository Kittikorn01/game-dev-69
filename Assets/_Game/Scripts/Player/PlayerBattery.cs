using System;
using UnityEngine;

public class PlayerBattery : MonoBehaviour
{
    [SerializeField, Min(1f)] private float maxBattery = 100f;
    [SerializeField, Min(0f)] private float drainPerSecond = 5f;

    // sends Normalized (0-1) for the battery bar
    public event Action<float> BatteryChanged;

    public float Current { get; private set; }
    public float Max => maxBattery;
    public float Normalized => Current / maxBattery;
    public bool IsEmpty => Current <= 0f;

    private void Awake()
    {
        Current = maxBattery;
    }

    public void Drain(float deltaTime)
    {
        SetCurrent(Current - drainPerSecond * deltaTime);
    }

    public void Add(float amount)
    {
        if (amount <= 0f) return;

        SetCurrent(Current + amount);
    }

    private void SetCurrent(float value)
    {
        float previous = Current;
        Current = Mathf.Clamp(value, 0f, maxBattery);
        if (Mathf.Approximately(previous, Current)) return;

        // drain runs every frame, so log only when the whole number changes
        if (Mathf.CeilToInt(previous) != Mathf.CeilToInt(Current))
        {
            Debug.Log($"Battery: {Mathf.CeilToInt(Current)}/{maxBattery}");
        }

        BatteryChanged?.Invoke(Normalized);
    }
}
