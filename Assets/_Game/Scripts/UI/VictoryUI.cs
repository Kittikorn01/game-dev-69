using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// When GameManager reports the boss is defeated, optionally shows a "YOU WIN" panel
// for a moment, then loads the ending video scene.
// Put this on an object that stays active (e.g. a Canvas), not on the panel itself.
public class VictoryUI : MonoBehaviour
{
    [SerializeField] private GameObject victoryPanel;
    // must be added in File > Build Profiles
    [SerializeField] private string endingScene = "Ending";
    // seconds to show the panel before loading the ending (real time, the game is paused on win)
    [SerializeField, Min(0f)] private float delay = 2f;

    [Header("Sound (optional)")]
    // played once when the screen appears
    [SerializeField] private AudioClip showSound;
    [SerializeField, Range(0f, 1f)] private float showVolume = 1f;

    private GameManager gameManager;

    private void Start()
    {
        if (victoryPanel != null) victoryPanel.SetActive(false);

        // GameManager sets Instance in Awake, so it is ready by Start
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            Debug.LogWarning("VictoryUI: no GameManager in this scene", this);
            return;
        }
        gameManager.Won += OnWon;
    }

    private void OnDestroy()
    {
        if (gameManager != null) gameManager.Won -= OnWon;
    }

    private void OnWon()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        PlayShowSound();
        StartCoroutine(LoadEndingAfterDelay());
    }

    private IEnumerator LoadEndingAfterDelay()
    {
        // GameManager sets timeScale to 0 on win, so wait in real time
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(endingScene);
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

    // test only: in Play mode, use the ⋮ menu on this component
    [ContextMenu("Test/Win Now")]
    private void TestWin() => OnWon();
}
