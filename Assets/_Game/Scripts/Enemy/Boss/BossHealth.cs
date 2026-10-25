using System;
using UnityEngine;

// Ordinary damage only gets through while the armor is broken.
[DisallowMultipleComponent]
[RequireComponent(typeof(BossArmor))]
public class BossHealth : Health
{
    private BossArmor armor;

    // a hit landed but the armor stopped it
    public event Action DamageBlocked;

    // Start, not Awake: defining Awake here would hide Health's private Awake that sets HP
    private void Start()
    {
        armor = GetComponent<BossArmor>();
    }

    public override void TakeDamage(int amount)
    {
        if (armor == null || armor.IsArmored)
        {
            if (amount > 0 && CurrentHp > 0) DamageBlocked?.Invoke();
            return;
        }
        base.TakeDamage(amount);
    }
}
