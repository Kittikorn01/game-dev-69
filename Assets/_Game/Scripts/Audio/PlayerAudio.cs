using UnityEngine;

// Plays player sound effects (shooting, flashlight on/off).
// Listens to PlayerInputReader events, so it does not modify the Lead's scripts.
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;

    [Header("Shoot")]
    [SerializeField] private AudioClip shootClip;
    [SerializeField, Range(0f, 1f)] private float shootVolume = 0.8f;
    // keep this equal to PlayerShooter.fireCooldown so no sound plays when no bullet is fired
    [SerializeField] private float fireCooldown = 0.2f;
    [SerializeField] private Vector2 shootPitchRange = new Vector2(0.95f, 1.05f);

    [Header("Flashlight")]
    [SerializeField] private AudioClip flashlightOnClip;
    [SerializeField] private AudioClip flashlightOffClip;
    [SerializeField, Range(0f, 1f)] private float flashlightVolume = 0.7f;

    private AudioSource audioSource;
    private float nextFireTime;
    private bool flashlightOn; // PlayerFlashlight starts with the light off

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        inputReader.AttackPressed += OnAttack;
        inputReader.FlashlightPressed += OnFlashlight;
    }

    private void OnDisable()
    {
        inputReader.AttackPressed -= OnAttack;
        inputReader.FlashlightPressed -= OnFlashlight;
    }

    private void OnAttack()
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireCooldown;

        if (shootClip == null) return;
        audioSource.pitch = Random.Range(shootPitchRange.x, shootPitchRange.y);
        audioSource.PlayOneShot(shootClip, shootVolume);
    }

    private void OnFlashlight()
    {
        flashlightOn = !flashlightOn;

        AudioClip clip = flashlightOn ? flashlightOnClip : flashlightOffClip;
        if (clip == null) clip = flashlightOnClip; // use one click for both if there is no off sound

        if (clip == null) return;
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(clip, flashlightVolume);
    }
}
