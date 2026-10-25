using System;
using UnityEngine;

// An enemy prefab with a pick chance relative to the others in the same list.
[Serializable]
public class WeightedEnemy
{
    // drag prefabs from Prefabs/Enemy/Ready
    [SerializeField] private EnemyBase prefab;
    [SerializeField, Min(0)] private int weight = 1;

    public EnemyBase Prefab => prefab;
    public int Weight => weight;
}
