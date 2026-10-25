using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// Plays the story video, then loads the first level.
// The skip key (Space by default) skips it after a short delay, so a key held from the menu does not skip.
[RequireComponent(typeof(VideoPlayer))]
public class IntroVideo : MonoBehaviour
{
    // must be added in File > Build Profiles
    [SerializeField] private string nextScene = "Level1";
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private Key skipKey = Key.Space;
    [SerializeField, Min(0f)] private float skipDelay = 0.5f;
    // optional: e.g. a "Press SPACE to skip" text, shown once skipping is allowed
    [SerializeField] private GameObject skipHint;

    private VideoPlayer player;
    private float startTime;
    private bool loading;

    private void Awake()
    {
        player = GetComponent<VideoPlayer>();
        player.isLooping = false;
        player.loopPointReached += OnVideoFinished;
    }

    private void Start()
    {
        Time.timeScale = 1f;
        startTime = Time.unscaledTime;
        if (skipHint != null) skipHint.SetActive(false);

        // without a video the scene would wait forever, so go straight to the level
        if (player.clip == null && string.IsNullOrEmpty(player.url))
        {
            Debug.LogWarning("IntroVideo: no video assigned, skipping intro", this);
            LoadNext();
            return;
        }

        if (!player.isPlaying) player.Play();
    }

    private void OnDestroy()
    {
        if (player != null) player.loopPointReached -= OnVideoFinished;
    }

    private void Update()
    {
        if (!allowSkip || Time.unscaledTime - startTime < skipDelay) return;

        if (skipHint != null && !skipHint.activeSelf) skipHint.SetActive(true);

        if (Keyboard.current != null && Keyboard.current[skipKey].wasPressedThisFrame) Skip();
    }

    // also usable from a "Skip" button's OnClick()
    public void Skip()
    {
        LoadNext();
    }

    private void OnVideoFinished(VideoPlayer source)
    {
        LoadNext();
    }

    private void LoadNext()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(nextScene);
    }
}
