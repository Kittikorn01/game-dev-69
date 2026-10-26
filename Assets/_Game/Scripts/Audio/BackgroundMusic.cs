using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Background music that keeps playing across scenes (e.g. menu -> intro, level -> level),
// then removes itself when a scene outside keepInScenes loads.
// Put it in each scene that starts a song, with the clip set on the AudioSource (Loop + Play On Awake).
// Same clip as the one already playing: the song just continues. Different clip: it replaces the old song.
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusic : MonoBehaviour
{
    // scene names (as in File > Build Profiles) where this song keeps playing
    [SerializeField] private string[] keepInScenes = { "MainMenu", "Intro" };
    // how long the old song takes to fade out when it is replaced or its scenes end
    [SerializeField, Min(0f)] private float fadeOutDuration = 1.5f;

    private static BackgroundMusic instance;
    private AudioSource audioSource;
    private bool fading;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance != null)
        {
            // the same song is already playing from the previous scene: keep it going
            if (instance.audioSource.clip == audioSource.clip)
            {
                Destroy(gameObject);
                return;
            }
            instance.FadeOutAndDestroy();
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (System.Array.IndexOf(keepInScenes, scene.name) < 0) FadeOutAndDestroy();
    }

    // lets the old song fade out in the new scene instead of cutting off
    private void FadeOutAndDestroy()
    {
        if (fading) return;
        fading = true;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (instance == this) instance = null;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        // unscaled, so it still fades if the new scene starts paused
        for (float t = 0f; t < fadeOutDuration; t += Time.unscaledDeltaTime)
        {
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeOutDuration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
