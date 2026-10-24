using UnityEngine;

// Blocks the player while locked, then loads the next level when the player
// touches the door after GameManager reports every generator is turned on.
// Needs two Collider2D: a solid one set as "Blocker", and a slightly larger
// one with "Is Trigger" checked to detect the player.
public class ExitDoor : MonoBehaviour
{
    // solid collider, disabled once the door unlocks
    [SerializeField] private Collider2D blocker;
    // optional: shown/hidden when the door unlocks, e.g. an open-door sprite
    [SerializeField] private GameObject unlockedVisual;

    private int playerColliders;
    private bool exitRequested;

    private void Start()
    {
        if (unlockedVisual != null) unlockedVisual.SetActive(false);
        if (blocker == null) Debug.LogWarning($"[ExitDoor] Blocker not set, door can be walked through: {name}");
        if (GameManager.Instance == null) Debug.LogWarning($"[ExitDoor] No GameManager in scene: {name}");
    }

    // polled instead of using AllGeneratorsActivated: generators register in Start,
    // so the count is only final from the first Update
    private void Update()
    {
        bool unlocked = GameManager.Instance != null && GameManager.Instance.AllGeneratorsOn;

        if (blocker != null && blocker.enabled == unlocked) blocker.enabled = !unlocked;
        if (unlockedVisual != null && unlockedVisual.activeSelf != unlocked) unlockedVisual.SetActive(unlocked);

        // also covers a player already standing at the door when it unlocks
        if (unlocked && playerColliders > 0 && !exitRequested)
        {
            exitRequested = true;
            GameManager.Instance.PlayerReachedExit();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsPlayer(other)) playerColliders++;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (IsPlayer(other)) playerColliders = Mathf.Max(0, playerColliders - 1);
    }

    private static bool IsPlayer(Collider2D other)
    {
        if (other.CompareTag("Player")) return true;
        return other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player");
    }
}
