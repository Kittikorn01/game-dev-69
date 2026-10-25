using System;
using UnityEngine;

[Serializable]
public class LootDropEntry
{
    [SerializeField] private GameObject prefab;
    [SerializeField, Range(0f, 100f)] private float chance;

    public GameObject Prefab => prefab;
    public float Chance => chance;
}
