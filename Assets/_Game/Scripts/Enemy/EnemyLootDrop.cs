using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Health))]
public class EnemyLootDrop : MonoBehaviour
{
    [Tooltip("Chances are percentages. One roll per death, so at most one item drops.")]
    [SerializeField] private LootDropEntry[] entries;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.Died += OnDied;
    }

    private void OnDisable()
    {
        health.Died -= OnDied;
    }

    private void OnValidate()
    {
        if (entries == null) return;
        float total = 0f;
        foreach (LootDropEntry entry in entries) total += entry.Chance;
        if (total > 100f) Debug.LogWarning($"{name}: loot chances add up to {total}%, entries past 100% never drop", this);
    }

    private void OnDied()
    {
        GameObject prefab = PickPrefab();
        if (prefab != null) Instantiate(prefab, transform.position, Quaternion.identity);
    }

    // Walks the entries as consecutive slices of 0-100; the leftover slice drops nothing
    private GameObject PickPrefab()
    {
        if (entries == null) return null;
        float roll = Random.Range(0f, 100f);
        foreach (LootDropEntry entry in entries)
        {
            if (roll < entry.Chance) return entry.Prefab;
            roll -= entry.Chance;
        }
        return null;
    }
}
