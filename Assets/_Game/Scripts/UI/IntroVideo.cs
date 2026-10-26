using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

// Plays the story clips in order, then loads the first level.
// The skip key (Space by default) skips all of them after a short delay, so a key held from the menu does not skip.
[RequireComponent(typeof(VideoPlayer))]
public class IntroVideo : MonoBehaviour
{
    // must be added in File > Build Profiles
    [SerializeField] private string nextScene = "Level1";
    // played one after another; when empty, the VideoPlayer's own clip or URL is used
    [SerializeField] private VideoClip[] clips;
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private Key skipKey = Key.Space;
    [SerializeField, Min(0f)] private float skipDelay = 0.5f;
    // optional: e.g. a "Press SPACE to skip" text, shown once skipping is allowed
    [SerializeField] private GameObject skipHint;

    private VideoPlayer player;
    private float startTime;
    private bool loading;
    private int clipIndex = -1;

    private void Awake()
    {
        player = GetComponent<VideoPlayer>();
        player.isLooping = false;
        player.loopPointReached += OnVideoFinished;
        if (clips != null && clips.Length > 0) SetNextClip();
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
        if (clips != null && SetNextClip())
        {
            player.Play();
            return;
        }
        LoadNext();
    }

    // moves to the next non-empty clip in the list, false when none are left
    private bool SetNextClip()
    {
        while (++clipIndex < clips.Length)
        {
            if (clips[clipIndex] == null) continue;
            player.clip = clips[clipIndex];
            return true;
        }
        return false;
    }

    private void LoadNext()
    {
        if (loading) return;
        loading = true;
        SceneManager.LoadScene(nextScene);
    }
}
