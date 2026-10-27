using UnityEngine;
using UnityEngine.Rendering.Universal;

// Breathes red while the exit is locked, then holds a steady light green once every generator is on.
public class ExitDoorLight : MonoBehaviour
{
    // found on this GameObject when empty
    [SerializeField] private Light2D doorLight;
    [SerializeField] private Color lockedColor = new Color(1f, 0.1f, 0.1f);
    [SerializeField] private Color unlockedColor = new Color(0.55f, 1f, 0.55f);
    [SerializeField, Min(0f)] private float minLockedIntensity = 0.2f;
    [SerializeField, Min(0f)] private float maxLockedIntensity = 1.5f;
    [SerializeField, Min(0f)] private float unlockedIntensity = 1.2f;
    // full breath cycles per second
    [SerializeField, Min(0.01f)] private float breathSpeed = 0.8f;

    private void Awake()
    {
        if (doorLight == null) doorLight = GetComponent<Light2D>();
        if (doorLight == null)
        {
            Debug.LogWarning($"[ExitDoorLight] Light2D not set: {name}", this);
            enabled = false;
        }
    }

    // polled like ExitDoor: generators register in Start, so the count is only final from the first Update
    private void Update()
    {
        bool unlocked = GameManager.Instance != null && GameManager.Instance.AllGeneratorsOn;

        if (unlocked)
        {
            doorLight.color = unlockedColor;
            doorLight.intensity = unlockedIntensity;
            return;
        }

        float t = (Mathf.Cos(Time.time * breathSpeed * 2f * Mathf.PI) + 1f) / 2f;
        doorLight.color = lockedColor;
        doorLight.intensity = Mathf.Lerp(minLockedIntensity, maxLockedIntensity, t);
    }
}
