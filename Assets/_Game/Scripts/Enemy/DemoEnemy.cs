using UnityEngine;

public class DemoEnemy : EnemyBase
{
    protected override void Move()
    {
        // Demo stands still
    }

    protected override void Attack()
    {
        Debug.Log($"{name} attacks player for {damage}");
        player.GetComponent<IDamageable>()?.TakeDamage(damage);
    }

    protected override void OnDied()
    {
        Debug.Log($"{name} died");
        base.OnDied();
    }

    // Right-click the component in Play mode to test death through Health
    [ContextMenu("Test Kill")]
    private void TestKill()
    {
        health.TakeDamage(health.MaxHp);
    }
}
