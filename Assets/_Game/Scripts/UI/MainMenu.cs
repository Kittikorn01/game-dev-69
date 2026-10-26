using UnityEngine;
using UnityEngine.SceneManagement;

// Hook these methods to the buttons' OnClick() in the Inspector.
public class MainMenu : MonoBehaviour
{
    // must match the scene file name and be added in File > Build Profiles
    [SerializeField] private string firstLevelScene = "Level1";
    // optional, fades to black before loading; loads right away when empty
    [SerializeField] private ScreenFader screenFader;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        // the previous game may have ended paused (GameManager sets timeScale to 0)
        Time.timeScale = 1f;
        ShowMain();
    }

    public void StartGame()
    {
        PlayerRunData.Clear();
        if (screenFader != null) screenFader.FadeOutAndLoad(firstLevelScene);
        else SceneManager.LoadScene(firstLevelScene);
    }

    public void OpenCredits()
    {
        if (mainPanel != null) mainPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(true);
    }

    public void ShowMain()
    {
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (mainPanel != null) mainPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Debug.Log("MainMenu: quit");
#if UNITY_EDITOR
        // Application.Quit does nothing in the editor, so stop Play mode instead
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
