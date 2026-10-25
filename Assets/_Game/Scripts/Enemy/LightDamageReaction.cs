using UnityEngine;

[RequireComponent(typeof(Health))]
public class LightDamageReaction : MonoBehaviour, ILightReactive
{
    [SerializeField] private int damagePerTick = 1;
    [SerializeField] private float tickInterval = 0.5f;

    private Health health;
    private bool isLit;
    private float nextTickTime;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!isLit || Time.time < nextTickTime) return;

        nextTickTime = Time.time + tickInterval;
        health.TakeLightDamage(damagePerTick);
        Debug.Log($"{name} takes {damagePerTick} light damage (HP {health.CurrentHp})");
    }

    public void OnLightEnter()
    {
        isLit = true;
        // First tick waits a full interval so flicking the light gives no free damage
        nextTickTime = Time.time + tickInterval;
    }

    public void OnLightExit()
    {
        isLit = false;
    }
}
