using UnityEngine;
using UnityEngine.Rendering.Universal;

// Plays player sound effects (shooting, empty gun, flashlight on/off).
// Only reads the Lead's components (PlayerAmmo, flashlight Light2D), never modifies them.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private PlayerAmmo ammo;
    [SerializeField] private Light2D flashlight;

    [Header("Shoot")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.8f;
    [SerializeField] private Vector2 shootPitchRange = new Vector2(0.95f, 1.05f);
    // optional: plays when clicking with no ammo left
    [SerializeField] private AudioClip emptyClip;
    [SerializeField, Range(0f, 1f)] private float emptyVolume = 0.6f;

    [Header("Flashlight")]
    [SerializeField] private AudioClip flashlightOnClip;
    [SerializeField] private AudioClip flashlightOffClip;
    [SerializeField, Range(0f, 1f)] private float flashlightVolume = 0.7f;

    private AudioSource audioSource;
    private int lastAmmo;
    private int lastShotFrame = -1;
    private bool lastFlashlightOn;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        lastAmmo = ammo.Current;
        lastFlashlightOn = flashlight.enabled;
    }

    private void OnEnable()
    {
        ammo.AmmoChanged += OnAmmoChanged;
        inputReader.AttackPressed += OnAttackPressed;
    }

    private void OnDisable()
    {
        ammo.AmmoChanged -= OnAmmoChanged;
        inputReader.AttackPressed -= OnAttackPressed;
    }

    // ammo going down = a bullet was really fired
    private void OnAmmoChanged(int current)
    {
        if (current < lastAmmo)
        {
            lastShotFrame = Time.frameCount;
            Play(shootClip, shootVolume, Random.Range(shootPitchRange.x, shootPitchRange.y));
        }
        lastAmmo = current;
    }

    private void OnAttackPressed()
    {
        // skip if this same click just fired the last bullet
        if (ammo.Current > 0 || lastShotFrame == Time.frameCount) return;
        Play(emptyClip, emptyVolume, 1f);
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

    private void Play(AudioClip clip, float volume, float pitch)
    {
        if (clip == null) return;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume);
    }
}
