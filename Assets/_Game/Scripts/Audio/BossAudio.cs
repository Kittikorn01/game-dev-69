using UnityEngine;

// Plays boss sound effects by listening to the Lead's boss events.
// Put it on the Boss root (same object as Boss / BossHealth / BossArmor / BossSummoner).
// Every clip is optional: leave a slot empty and that sound is skipped.
[RequireComponent(typeof(AudioSource))]
public class BossAudio : MonoBehaviour
{
    [Header("Clips")]
    // played once when the boss appears (scene start)
    [SerializeField] private AudioClip appearClip;
    // bullet hits while the armor is up (no damage)
    [SerializeField] private AudioClip blockedClip;
    // took real damage
    [SerializeField] private AudioClip hurtClip;
    // flashlight broke the armor
    [SerializeField] private AudioClip armorBreakClip;
    // armor came back
    [SerializeField] private AudioClip armorRestoreClip;
    // a minion was summoned
    [SerializeField] private AudioClip summonClip;
    [SerializeField] private AudioClip deathClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float volume = 0.9f;
    // hits can come very fast, so limit how often blocked/hurt sounds repeat
    [SerializeField, Min(0f)] private float hitSoundCooldown = 0.08f;

    private AudioSource audioSource;
    private Health health;
    private BossHealth bossHealth;
    private BossArmor armor;
    private BossSummoner summoner;
    private int lastHp;
    private float nextHitSoundTime;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        health = GetComponent<Health>();
        bossHealth = GetComponent<BossHealth>();
        armor = GetComponent<BossArmor>();
        summoner = GetComponent<BossSummoner>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.HealthChanged += OnHealthChanged;
            health.Died += OnDied;
        }
        if (bossHealth != null) bossHealth.DamageBlocked += OnDamageBlocked;
        if (armor != null)
        {
            armor.ArmorBroken += OnArmorBroken;
            armor.ArmorRestored += OnArmorRestored;
        }
        if (summoner != null) summoner.Summoned += OnSummoned;
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.HealthChanged -= OnHealthChanged;
            health.Died -= OnDied;
        }
        if (bossHealth != null) bossHealth.DamageBlocked -= OnDamageBlocked;
        if (armor != null)
        {
            armor.ArmorBroken -= OnArmorBroken;
            armor.ArmorRestored -= OnArmorRestored;
        }
        if (summoner != null) summoner.Summoned -= OnSummoned;
    }

    private void Start()
    {
        if (health != null) lastHp = health.CurrentHp;
        Play(appearClip);
    }

    private void OnHealthChanged(int current, int max)
    {
        // only a drop is a hit; death has its own sound
        if (current < lastHp && current > 0) PlayHit(hurtClip);
        lastHp = current;
    }

    private void OnDamageBlocked() => PlayHit(blockedClip);
    private void OnArmorBroken() => Play(armorBreakClip);
    private void OnArmorRestored() => Play(armorRestoreClip);
    private void OnSummoned() => Play(summonClip);
    private void OnDied() => Play(deathClip);

    private void PlayHit(AudioClip clip)
    {
        if (Time.time < nextHitSoundTime) return;
        nextHitSoundTime = Time.time + hitSoundCooldown;
        Play(clip);
    }

    private void Play(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip, volume);
    }
}
