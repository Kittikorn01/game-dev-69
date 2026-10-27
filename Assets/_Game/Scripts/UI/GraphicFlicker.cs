using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Makes a UI Graphic (Image, TMP text) flicker like a failing light: steady, then a quick burst of blinks.
public class GraphicFlicker : MonoBehaviour
{
    [SerializeField] private Graphic target;
    [SerializeField, Range(0f, 1f)] private float dimAlpha = 0.15f;
    // seconds of steady light between bursts
    [SerializeField, Min(0f)] private float minSteadyTime = 1.5f;
    [SerializeField, Min(0f)] private float maxSteadyTime = 4f;
    // blinks per burst
    [SerializeField, Min(1)] private int minBlinks = 1;
    [SerializeField, Min(1)] private int maxBlinks = 4;
    // seconds each dim/bright step lasts inside a burst
    [SerializeField, Min(0.01f)] private float minBlinkTime = 0.03f;
    [SerializeField, Min(0.01f)] private float maxBlinkTime = 0.12f;

    private float brightAlpha;

    private void Awake()
    {
        if (target == null) target = GetComponent<Graphic>();
        if (target == null)
        {
            Debug.LogWarning($"[GraphicFlicker] No Graphic: {name}", this);
            enabled = false;
            return;
        }
        brightAlpha = target.color.a;
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (target != null) SetAlpha(brightAlpha);
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSteadyTime, maxSteadyTime));

            int blinks = Random.Range(minBlinks, maxBlinks + 1);
            for (int i = 0; i < blinks; i++)
            {
                SetAlpha(dimAlpha);
                yield return new WaitForSeconds(Random.Range(minBlinkTime, maxBlinkTime));
                SetAlpha(brightAlpha);
                yield return new WaitForSeconds(Random.Range(minBlinkTime, maxBlinkTime));
            }
        }
    }

    private void SetAlpha(float alpha)
    {
        Color color = target.color;
        color.a = alpha;
        target.color = color;
    }
}
