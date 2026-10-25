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

    // true if the beam is on and reaches the point, e.g. so spawners avoid lit spots
    public bool IsPointLit(Vector2 point)
    {
        if (!beam.enabled) return false;

        float radius = beam.pointLightOuterRadius;
        if ((point - (Vector2)transform.position).sqrMagnitude > radius * radius) return false;
        return IsInBeam(point);
    }

    // Point Light2D shines along local up, spread over pointLightOuterAngle
    private void CollectHits()
    {
        Vector2 origin = transform.position;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(origin, beam.pointLightOuterRadius, targetMask))
        {
            if (!IsInBeam(hit.bounds.center)) continue;

            foreach (ILightReactive target in hit.GetComponentsInParent<ILightReactive>())
            {
                currentHits.Add(target);
            }
        }
    }

    // Angle and wall check only: CollectHits already limits range by collider overlap
    private bool IsInBeam(Vector2 point)
    {
        Vector2 origin = transform.position;
        if (Vector2.Angle(transform.up, point - origin) > beam.pointLightOuterAngle * 0.5f) return false;
        return !Physics2D.Linecast(origin, point, obstacleMask);
    }

    // Targets destroyed while lit stay in the set until the next step
    private static bool IsAlive(ILightReactive target)
    {
        return target as Object != null;
    }
}
