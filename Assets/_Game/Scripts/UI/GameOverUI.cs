using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Shows the "YOU DIED" panel when GameManager reports the player lost.
// Put this on an object that stays active (e.g. the Canvas), not on the panel itself.
public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    // scene names must be added in File > Build Profiles
    [SerializeField] private string restartScene = "Level1";
    [SerializeField] private string menuScene = "MainMenu";

    [Header("Sound (optional)")]
    // played once when the screen appears
    [SerializeField] private AudioClip showSound;
    [SerializeField, Range(0f, 1f)] private float showVolume = 1f;

    [Header("Transition")]
    // lets the death animation (0.5s) play before the screen appears
    [SerializeField, Min(0f)] private float showDelay = 1f;
    // the panel's dark background fades in first (needs a CanvasGroup on the panel)
    [SerializeField, Min(0f)] private float backgroundFadeDuration = 0.5f;
    // then the text and buttons under it
    [SerializeField] private CanvasGroup contentGroup;
    [SerializeField, Min(0f)] private float contentFadeDuration = 0.5f;
    // found in the scene when empty; scenes load right away if there is none
    [SerializeField] private ScreenFader screenFader;

    private GameManager gameManager;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (screenFader == null) screenFader = FindFirstObjectByType<ScreenFader>();

        // GameManager sets Instance in Awake, so it is ready by Start
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogWarning("GameOverUI: no GameManager in this scene", this);
            return;
        }
        gameManager.Lost += Show;
    }

    private void OnDestroy()
    {
        if (gameManager != null) gameManager.Lost -= Show;
    }

    private void Show()
    {
        StartCoroutine(ShowSequence());
    }

    private IEnumerator ShowSequence()
    {
        yield return new WaitForSecondsRealtime(showDelay);

        // content stays hidden and unclickable until the background is in
        if (contentGroup != null)
        {
            contentGroup.alpha = 0f;
            contentGroup.interactable = false;
        }
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        PlayShowSound();

        if (gameOverPanel != null && gameOverPanel.TryGetComponent(out CanvasGroup background))
            yield return CanvasGroupFade.FadeIn(background, backgroundFadeDuration);

        if (contentGroup != null)
        {
            yield return CanvasGroupFade.FadeIn(contentGroup, contentFadeDuration);
            contentGroup.interactable = true;
        }
    }

    private void PlayShowSound()
    {
        if (showSound == null) return;
        AudioSource source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        // the game is paused on this screen, so make sure the sound still plays
        source.ignoreListenerPause = true;
        source.PlayOneShot(showSound, showVolume);
    }

    public void Restart()
    {
        // GameManager pauses the game on death, so unpause before loading
        Time.timeScale = 1f;
        PlayerRunData.Clear();
        Load(restartScene);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        Load(menuScene);
    }

    private void Load(string sceneName)
    {
        if (screenFader != null) screenFader.FadeOutAndLoad(sceneName);
        else SceneManager.LoadScene(sceneName);
    }

    // test only: in Play mode, use the ⋮ menu on this component
    [ContextMenu("Test/Show Game Over")]
    private void TestShow() => Show();
}
