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
        StartCoroutine(LoadEndingAfterDelay());
    }

    private IEnumerator LoadEndingAfterDelay()
    {
        // GameManager sets timeScale to 0 on win, so wait in real time
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(endingScene);
    }

    // test only: in Play mode, use the ⋮ menu on this component
    [ContextMenu("Test/Win Now")]
    private void TestWin() => OnWon();
}
