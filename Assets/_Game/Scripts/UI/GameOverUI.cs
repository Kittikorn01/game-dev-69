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

    private GameManager gameManager;

    private void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

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
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        PlayShowSound();
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
        SceneManager.LoadScene(restartScene);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }

    // test only: in Play mode, use the ⋮ menu on this component
    [ContextMenu("Test/Show Game Over")]
    private void TestShow() => Show();
}
