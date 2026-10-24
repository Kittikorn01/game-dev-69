using UnityEngine;

// The existing Bullet uses IDamageable: block that path entirely.
// The existing LightDamageReaction holds a Health reference and intentionally
// calls Health.TakeDamage directly, so flashlight ticks still reduce HP.
// Keep this as the only Health component on the enemy.
[DisallowMultipleComponent]
public class FlashlightOnlyHealth : Health, IDamageable
{
    // Health.TakeDamage is not virtual. Reimplement the interface as well as
    // hiding the method so both Bullet and typed callers are rejected.
    public new void TakeDamage(int amount)
    {
        // Immune to ordinary damage, even while illuminated.
    }

    void IDamageable.TakeDamage(int amount)
    {
        TakeDamage(amount);
    }
}
