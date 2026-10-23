using UnityEngine;

[RequireComponent(typeof(EnemyBase))]
public class LightFreezeReaction : MonoBehaviour, ILightReactive
{
    private EnemyBase enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    public void OnLightEnter()
    {
        enemy.SetStunned(true);
        Debug.Log($"{name} stunned");
    }

    public void OnLightExit()
    {
        enemy.SetStunned(false);
        Debug.Log($"{name} unstunned");
    }
}
