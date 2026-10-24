using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerFlashlight : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerBattery battery;
    [SerializeField] private Light2D flashlight;
    // ambient glow around the player, dimmed while the flashlight is on
    [SerializeField] private Light2D ambientLight;
    [SerializeField, Range(0f, 1f)] private float ambientDimMultiplier = 0.3f;

    private float ambientIntensity;

    private void Awake()
    {
        ambientIntensity = ambientLight.intensity;
    }

    private void Start()
    {
        SetFlashlight(false);
    }

    private void OnEnable()
    {
        inputReader.FlashlightPressed += Toggle;
    }

    private void OnDisable()
    {
        inputReader.FlashlightPressed -= Toggle;
    }

    private void Update()
    {
        if (!flashlight.enabled) return;

        battery.Drain(Time.deltaTime);
        if (battery.IsEmpty) SetFlashlight(false);
    }

    private void Toggle()
    {
        if (!flashlight.enabled && battery.IsEmpty) return;

        SetFlashlight(!flashlight.enabled);
    }

    private void SetFlashlight(bool isOn)
    {
        flashlight.enabled = isOn;
        ambientLight.intensity = isOn ? ambientIntensity * ambientDimMultiplier : ambientIntensity;
    }
}
