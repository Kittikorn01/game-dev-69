using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Fades the scene's global light up once all generators are on, so the player can find the exit.
// Each level is its own scene, so the next level starts back at its own darkness.
public class GeneratorLighting : MonoBehaviour
{
    // found on this GameObject when empty
    [SerializeField] private Light2D globalLight;
    [SerializeField, Min(0f)] private float targetIntensity = 0.6f;
    [SerializeField, Min(0f)] private float fadeDuration = 2f;

    private void Awake()
    {
        if (globalLight == null) globalLight = GetComponent<Light2D>();
        if (globalLight == null) Debug.LogWarning($"[GeneratorLighting] Light2D not set: {name}", this);
    }

    // Start, not OnEnable: GameManager.Instance is only set after every Awake
    private void Start()
    {
        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated += BrightenScene;
        else Debug.LogWarning($"[GeneratorLighting] No GameManager in scene: {name}", this);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.AllGeneratorsActivated -= BrightenScene;
    }

    private void BrightenScene()
    {
        if (globalLight != null) StartCoroutine(FadeIntensity());
    }

    private IEnumerator FadeIntensity()
    {
        float startIntensity = globalLight.intensity;
        for (float t = 0f; t < fadeDuration; t += Time.deltaTime)
        {
            globalLight.intensity = Mathf.Lerp(startIntensity, targetIntensity, t / fadeDuration);
            yield return null;
        }
        globalLight.intensity = targetIntensity;
    }
}
