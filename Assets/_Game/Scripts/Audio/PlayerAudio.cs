using UnityEngine;
using UnityEngine.Rendering.Universal;

// Plays player sound effects (shooting, empty gun, flashlight on/off).
// Only reads the Lead's components (PlayerAmmo, flashlight Light2D), never modifies them.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerAmmo ammo;
    [SerializeField] private PlayerBattery battery;
    [SerializeField] private Light2D flashlight;
    // optional: the player's Health, for hurt / death / heal sounds
    [SerializeField] private Health health;

    [Header("Shoot")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.8f;
    [SerializeField] private Vector2 shootPitchRange = new Vector2(0.95f, 1.05f);
    // optional: plays when clicking with no ammo left
    [SerializeField] private AudioClip emptyClip;
    [SerializeField, Range(0f, 1f)] private float emptyVolume = 0.6f;

    [Header("Pickup")]
    [SerializeField] private AudioClip ammoPickupClip;
    [SerializeField] private AudioClip batteryPickupClip;
    [SerializeField, Range(0f, 1f)] private float pickupVolume = 0.8f;

    [Header("Health")]
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    // health went up, e.g. a health pickup
    [SerializeField] private AudioClip healClip;
    [SerializeField, Range(0f, 1f)] private float healthVolume = 0.9f;

    [Header("Flashlight")]
    [SerializeField] private AudioClip flashlightOnClip;
    [SerializeField] private AudioClip flashlightOffClip;
    [SerializeField, Range(0f, 1f)] private float flashlightVolume = 0.7f;

    private AudioSource audioSource;
    private int lastAmmo;
    private int lastShotFrame = -1;
    private float lastBattery;
    private int lastHp;
    private bool lastFlashlightOn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        lastAmmo = ammo.Current;
        lastBattery = battery.Current;
        if (health != null) lastHp = health.CurrentHp;
        lastFlashlightOn = flashlight.enabled;
    }

    private void OnEnable()
    {
        ammo.AmmoChanged += OnAmmoChanged;
        battery.BatteryChanged += OnBatteryChanged;
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
            health.Died += OnDied;
        }
        inputReader.AttackPressed += OnAttackPressed;
    }

    private void OnDisable()
    {
        ammo.AmmoChanged -= OnAmmoChanged;
        battery.BatteryChanged -= OnBatteryChanged;
        if (health != null)
        {
            health.HealthChanged -= OnHealthChanged;
            health.Died -= OnDied;
        }
        inputReader.AttackPressed -= OnAttackPressed;
    }

    // ammo going down = a bullet was really fired, going up = ammo picked up
    private void OnAmmoChanged(int current)
    {
        if (current < lastAmmo)
        {
            lastShotFrame = Time.frameCount;
            Play(shootClip, shootVolume, Random.Range(shootPitchRange.x, shootPitchRange.y));
        }
        else if (current > lastAmmo)
        {
            Play(ammoPickupClip, pickupVolume, 1f);
        }
        lastAmmo = current;
    }

    // battery drains every frame, so only an increase means a pickup
    private void OnBatteryChanged(float normalized)
    {
        if (battery.Current > lastBattery)
        {
            Play(batteryPickupClip, pickupVolume, 1f);
        }
        lastBattery = battery.Current;
    }

    private void OnAttackPressed()
    {
        // skip if this same click just fired the last bullet
        if (ammo.Current > 0 || lastShotFrame == Time.frameCount) return;
        Play(emptyClip, emptyVolume, 1f);
    }

    // a drop is a hit (death has its own sound), a rise is a heal
    private void OnHealthChanged(int current, int max)
    {
        if (current < lastHp && current > 0) Play(hurtClip, healthVolume, Random.Range(0.95f, 1.05f));
        else if (current > lastHp) Play(healClip, healthVolume, 1f);
        lastHp = current;
    }

    private void OnDied()
    {
        Play(deathClip, healthVolume, 1f);
    }

    // watch the light itself, so it also clicks when the battery runs out
    private void LateUpdate()
    {
        bool isOn = flashlight.enabled;
        if (isOn == lastFlashlightOn) return;
        lastFlashlightOn = isOn;

        AudioClip clip = isOn ? flashlightOnClip : flashlightOffClip;
        if (clip == null) clip = flashlightOnClip;
        Play(clip, flashlightVolume, 1f);
    }

    // test only: in Play mode, click the ⋮ menu on this component to fake a pickup
    [ContextMenu("Test/Pickup Ammo (+5)")]
    private void TestPickupAmmo() => ammo.Add(5);

    [ContextMenu("Test/Pickup Battery (+20)")]
    private void TestPickupBattery() => battery.Add(20f);

    private void Play(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}
