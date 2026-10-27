using System.Collections;
using UnityEngine;

// Boss fight music: when the boss appears (this object becomes active), the level's
// BackgroundMusic fades down and the boss song fades in. When the boss dies, the boss song fades out.
// Put it on its own child object of the Boss (so it does not share the BossAudio AudioSource). The boss is turned on by BossEncounter,
// so the music starts exactly when the fight starts.
[RequireComponent(typeof(AudioSource))]
public class BossMusic : MonoBehaviour
{
    [SerializeField] private AudioClip bossSong;
    [SerializeField, Range(0f, 1f)] private float volume = 0.6f;
    [SerializeField, Min(0f)] private float fadeInDuration = 1.5f;
    [SerializeField, Min(0f)] private float fadeOutDuration = 2f;
    // how quiet the level music gets during the fight (0 = silent)
    [SerializeField, Range(0f, 1f)] private float levelMusicVolumeDuringFight = 0f;

    private AudioSource audioSource;
    private Health health;
    private AudioSource levelMusic;
    private float levelMusicStartVolume;

    // a song should not be 3D or start by itself before the fight
    private void Reset()
    {
        AudioSource source = GetComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 0f;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        // keep playing on the win screen, where the game is paused
        audioSource.ignoreListenerPause = true;
        // works on the Boss root or on a child object of it
        health = GetComponentInParent<Health>();
    }

    private void OnEnable()
    {
        if (health != null) health.Died += OnBossDied;
        if (bossSong == null) return;

        BackgroundMusic background = FindFirstObjectByType<BackgroundMusic>();
        if (background != null)
        {
            levelMusic = background.GetComponent<AudioSource>();
            levelMusicStartVolume = levelMusic.volume;
            StartCoroutine(Fade(levelMusic, levelMusicStartVolume * levelMusicVolumeDuringFight, fadeInDuration));
        }

        audioSource.clip = bossSong;
        audioSource.volume = 0f;
        audioSource.Play();
        StartCoroutine(Fade(audioSource, volume, fadeInDuration));
    }

    private void OnDisable()
    {
        if (health != null) health.Died -= OnBossDied;
    }

    private void OnBossDied()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(audioSource, 0f, fadeOutDuration));
    }

    // unscaled time, so fades still finish while the game is paused
    private static IEnumerator Fade(AudioSource source, float target, float duration)
    {
        if (source == null) yield break;
        float start = source.volume;
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            if (source == null) yield break;
            source.volume = Mathf.Lerp(start, target, t / duration);
            yield return null;
        }
        if (source != null) source.volume = target;
    }
}
