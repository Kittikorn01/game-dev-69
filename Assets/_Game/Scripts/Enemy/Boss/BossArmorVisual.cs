using UnityEngine;

// Shows the armor overlay while armored, flashes it when a hit is blocked
// and blinks it while the flashlight is wearing it down.
[RequireComponent(typeof(BossArmor), typeof(BossHealth))]
public class BossArmorVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer armorOverlay;
    [SerializeField] private Color blockedFlashColor = Color.white;
    [SerializeField, Min(0.01f)] private float blockedFlashDuration = 0.1f;
    [SerializeField] private Color litBlinkColor = Color.cyan;
    [SerializeField, Min(0.01f)] private float litBlinkInterval = 0.2f;

    private BossArmor armor;
    private BossHealth health;
    private Color armorColor;
    private float blockedFlashEndTime;

    private void Awake()
    {
        armor = GetComponent<BossArmor>();
        health = GetComponent<BossHealth>();

        if (armorOverlay != null) armorColor = armorOverlay.color;
        else Debug.LogWarning($"[BossArmorVisual] Armor Overlay not set: {name}", this);
    }

    private void OnEnable()
    {
        armor.ArmorBroken += HandleArmorBroken;
        armor.ArmorRestored += HandleArmorRestored;
        health.DamageBlocked += HandleDamageBlocked;
    }

    private void OnDisable()
    {
        armor.ArmorBroken -= HandleArmorBroken;
        armor.ArmorRestored -= HandleArmorRestored;
        health.DamageBlocked -= HandleDamageBlocked;
    }

    private void Update()
    {
        if (armorOverlay == null || !armor.IsArmored) return;
        armorOverlay.color = GetCurrentColor();
    }

    // a blocked hit wins over the light blink so every shot gets feedback
    private Color GetCurrentColor()
    {
        if (Time.time < blockedFlashEndTime) return blockedFlashColor;
        if (armor.IsLit && Mathf.FloorToInt(Time.time / litBlinkInterval) % 2 == 0) return litBlinkColor;
        return armorColor;
    }

    private void HandleArmorBroken()
    {
        if (armorOverlay != null) armorOverlay.enabled = false;
    }

    private void HandleArmorRestored()
    {
        if (armorOverlay == null) return;
        armorOverlay.color = armorColor;
        armorOverlay.enabled = true;
    }

    private void HandleDamageBlocked()
    {
        blockedFlashEndTime = Time.time + blockedFlashDuration;
    }
}
