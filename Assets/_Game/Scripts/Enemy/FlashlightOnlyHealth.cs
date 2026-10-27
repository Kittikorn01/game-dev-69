using UnityEngine;

// Blocks ordinary damage such as Bullet entirely.
// LightDamageReaction uses Health.TakeLightDamage, so flashlight ticks still reduce HP.
// Keep this as the only Health component on the enemy.
[DisallowMultipleComponent]
public class FlashlightOnlyHealth : Health
{
    public override void TakeDamage(int amount)
    {
        // Immune to ordinary damage, even while illuminated.
        RaiseDamageBlocked();
    }
}
