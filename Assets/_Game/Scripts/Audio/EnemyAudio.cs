using UnityEngine;

// Plays enemy sound effects by listening to EnemyBase / Health events.
// Put it on the enemy root (same object as BasicChaseEnemy and Health).
// Every clip is optional: leave a slot empty and that sound is skipped.
[RequireComponent(typeof(AudioSource))]
public class EnemyAudio : MonoBehaviour
{
    [Header("Clips")]
    // the enemy hits the player (e.g. a slash or a smash)
    [SerializeField] private AudioClip attackClip;
    // the enemy takes damage
    [SerializeField] private AudioClip hurtClip;
    [SerializeField] private AudioClip deathClip;
    // optional random growls while alive, so the player can hear enemies in the dark
    [SerializeField] private AudioClip[] idleClips;
    [SerializeField] private Vector2 idleInterval = new Vector2(4f, 9f);

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.8f;
    [SerializeField, Range(0f, 1f)] private float idleVolume = 0.5f;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.9f, 1.1f);
    // limits repeats when many bullets hit at once
    [SerializeField, Min(0f)] private float hurtSoundCooldown = 0.1f;

    private AudioSource audioSource;
    private EnemyBase enemy;
    private Health health;
    private int lastHp;
    private float nextHurtTime;
    private float nextIdleTime;
    private bool dead;

    // default 3D settings when the component is first added:
    // the camera sits 10 units away on Z, so full volume up to 10, silent past 22
    private void Reset()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = 10f;
        source.maxDistance = 22f;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        enemy = GetComponent<EnemyBase>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (enemy != null) enemy.Attacked += OnAttacked;
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
            health.Died += OnDied;
        }
    }

    private void OnDisable()
    {
        if (enemy != null) enemy.Attacked -= OnAttacked;
        if (health != null)
        {
            health.HealthChanged -= OnHealthChanged;
            health.Died -= OnDied;
        }
    }

    private void Start()
    {
        if (health != null) lastHp = health.CurrentHp;
        ScheduleIdle();
    }

    private void Update()
    {
        if (dead || idleClips == null || idleClips.Length == 0) return;
        if (Time.time < nextIdleTime) return;

        Play(idleClips[Random.Range(0, idleClips.Length)], idleVolume);
        ScheduleIdle();
    }

    private void ScheduleIdle()
    {
        nextIdleTime = Time.time + Random.Range(idleInterval.x, idleInterval.y);
    }

    private void OnAttacked() => Play(attackClip, volume);

    private void OnHealthChanged(int current, int max)
    {
        if (current < lastHp && current > 0 && Time.time >= nextHurtTime)
        {
            nextHurtTime = Time.time + hurtSoundCooldown;
            Play(hurtClip, volume);
        }
        lastHp = current;
    }

    private void OnDied()
    {
        dead = true;
        if (deathClip == null) return;

        // the enemy may be destroyed right away, so play from a temporary object
        // that copies this AudioSource's settings and removes itself when done
        GameObject temp = new GameObject($"{name}_DeathSound");
        temp.transform.position = transform.position;
        AudioSource source = temp.AddComponent<AudioSource>();
        source.spatialBlend = audioSource.spatialBlend;
        source.rolloffMode = audioSource.rolloffMode;
        source.minDistance = audioSource.minDistance;
        source.maxDistance = audioSource.maxDistance;
        source.outputAudioMixerGroup = audioSource.outputAudioMixerGroup;
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.PlayOneShot(deathClip, volume);
        Destroy(temp, deathClip.length / Mathf.Max(0.1f, source.pitch) + 0.1f);
    }

    private void Play(AudioClip clip, float clipVolume)
    {
        if (clip == null) return;
        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip, clipVolume);
    }
}
