using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class FlashlightDetector : MonoBehaviour
{
    [SerializeField] private LayerMask targetMask;
    // layers that block the beam, e.g. walls
    [SerializeField] private LayerMask obstacleMask;

    private Light2D beam;
    private readonly HashSet<ILightReactive> litTargets = new HashSet<ILightReactive>();
    private readonly HashSet<ILightReactive> currentHits = new HashSet<ILightReactive>();

    private void Awake()
    {
        beam = GetComponent<Light2D>();
    }

    private void FixedUpdate()
    {
        currentHits.Clear();
        if (beam.enabled) CollectHits();

        foreach (ILightReactive target in litTargets)
        {
            if (!currentHits.Contains(target) && IsAlive(target))
            {
                Debug.Log($"Flashlight left {((Component)target).name}");
                target.OnLightExit();
            }
        }
        foreach (ILightReactive target in currentHits)
        {
            if (!litTargets.Contains(target))
            {
                Debug.Log($"Flashlight hit {((Component)target).name}");
                target.OnLightEnter();
            }
        }

        litTargets.Clear();
        litTargets.UnionWith(currentHits);
    }

    private void OnDisable()
    {
        foreach (ILightReactive target in litTargets)
        {
            if (IsAlive(target)) target.OnLightExit();
        }
        litTargets.Clear();
    }

    // Point Light2D shines along local up, spread over pointLightOuterAngle
    private void CollectHits()
    {
        Vector2 origin = transform.position;
        float halfAngle = beam.pointLightOuterAngle * 0.5f;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, beam.pointLightOuterRadius, targetMask))
        {
            Vector2 targetPoint = hit.bounds.center;
            if (Vector2.Angle(transform.up, targetPoint - origin) > halfAngle) continue;
            if (Physics2D.Linecast(origin, targetPoint, obstacleMask)) continue;

            foreach (ILightReactive target in hit.GetComponentsInParent<ILightReactive>())
            {
                currentHits.Add(target);
            }
        }
    }

    // Targets destroyed while lit stay in the set until the next step
    private static bool IsAlive(ILightReactive target)
    {
        return target as Object != null;
    }
}
