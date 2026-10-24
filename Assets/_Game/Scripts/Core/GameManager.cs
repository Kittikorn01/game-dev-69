using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Won, Lost }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Health playerHealth;

    private GameState state = GameState.Playing;
    private int totalGenerators;
    private int activatedGenerators;

    public event Action<int, int> GeneratorProgressChanged;
    public event Action AllGeneratorsActivated;
    public event Action Won;
    public event Action Lost;

    public GameState State => state;
    public int TotalGenerators => totalGenerators;
    public int ActivatedGenerators => activatedGenerators;
    public bool AllGeneratorsOn => activatedGenerators >= totalGenerators;

    private void Awake()
    {
        Instance = this;
        // Reset in case the previous scene ended paused
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        if (playerHealth != null) playerHealth.Died += HandlePlayerDied;
    }

    private void OnDisable()
    {
        if (playerHealth != null) playerHealth.Died -= HandlePlayerDied;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void RegisterGenerator()
    {
        totalGenerators++;
        Debug.Log($"[GameManager] Generator registered ({activatedGenerators}/{totalGenerators})");
        GeneratorProgressChanged?.Invoke(activatedGenerators, totalGenerators);
    }

    public void ActivateGenerator()
    {
        if (state != GameState.Playing || AllGeneratorsOn) return;

        activatedGenerators++;
        Debug.Log($"[GameManager] Generator activated ({activatedGenerators}/{totalGenerators})");
        GeneratorProgressChanged?.Invoke(activatedGenerators, totalGenerators);

        if (AllGeneratorsOn)
        {
            Debug.Log("[GameManager] All generators activated, door open");
            AllGeneratorsActivated?.Invoke();
        }
    }

    public void PlayerReachedExit()
    {
        if (state != GameState.Playing) return;
        if (!AllGeneratorsOn)
        {
            Debug.Log($"[GameManager] Exit locked ({activatedGenerators}/{totalGenerators})");
            return;
        }

        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("[GameManager] No next scene in Build Settings");
            return;
        }

        Debug.Log($"[GameManager] Level complete, loading scene {nextIndex}");
        SceneManager.LoadScene(nextIndex);
    }

    public void BossDefeated()
    {
        EndGame(GameState.Won);
    }

    private void HandlePlayerDied()
    {
        EndGame(GameState.Lost);
    }

    private void EndGame(GameState result)
    {
        if (state != GameState.Playing) return;

        state = result;
        Time.timeScale = 0f;
        Debug.Log($"[GameManager] Game over: {result}");

        if (result == GameState.Won) Won?.Invoke();
        else Lost?.Invoke();
    }
}
