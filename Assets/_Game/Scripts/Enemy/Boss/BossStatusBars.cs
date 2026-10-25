using UnityEngine;

// Feeds the boss HP and armor into the bars above its head.
// Armor bar drains while the flashlight wears the armor down, then refills dimmed until it is back.
[RequireComponent(typeof(Health), typeof(BossArmor))]
public class BossStatusBars : MonoBehaviour
{
    [SerializeField] private WorldBar hpBar;
    [SerializeField] private WorldBar armorBar;
    [SerializeField] private Color armorColor = new Color(0.3f, 0.8f, 1f);
    [SerializeField] private Color armorRegenColor = new Color(0.25f, 0.35f, 0.45f);

    private Health health;
    private BossArmor armor;

    private void Awake()
    {
        health = GetComponent<Health>();
        armor = GetComponent<BossArmor>();

        if (hpBar == null || armorBar == null)
        {
            Debug.LogWarning($"[BossStatusBars] HP Bar or Armor Bar not set: {name}", this);
            enabled = false;
        }
    }

    private void OnEnable()
    {
        health.HealthChanged += HandleHealthChanged;
        health.Died += HandleDied;
    }

    private void OnDisable()
    {
        health.HealthChanged -= HandleHealthChanged;
        health.Died -= HandleDied;
    }

    // Start, not Awake: WorldBar reads its full width in its own Awake
    private void Start()
    {
        hpBar.SetValue(1f);
    }

    private void Update()
    {
        if (armor.IsArmored)
        {
            armorBar.SetValue(1f - armor.LightProgress01);
            armorBar.SetColor(armorColor);
        }
        else
        {
            armorBar.SetValue(armor.RegenProgress01);
            armorBar.SetColor(armorRegenColor);
        }
    }

    private void HandleHealthChanged(int currentHp, int maxHp)
    {
        hpBar.SetValue((float)currentHp / maxHp);
    }

    private void HandleDied()
    {
        hpBar.gameObject.SetActive(false);
        armorBar.gameObject.SetActive(false);
        enabled = false;
    }
}
