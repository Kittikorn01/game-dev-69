using UnityEngine;

// Shows the "next level" message when the player is near the door,
// but only after every generator in the list is turned on.
// Needs a Collider2D with "Is Trigger" checked on the same GameObject.
public class ExitDoor : MonoBehaviour
{
    [SerializeField] private Generator[] requiredGenerators;
    // e.g. a world-space text "Next Level", hidden until the door is ready
    [SerializeField] private GameObject nextLevelMessage;
    // optional: shown/hidden when the door unlocks, e.g. an open-door sprite
    [SerializeField] private GameObject unlockedVisual;

    private int playerColliders;
    private bool isShowing;

    public bool IsUnlocked
    {
        get
        {
            foreach (Generator generator in requiredGenerators)
            {
                if (generator == null || !generator.IsActivated) return false;
            }
            return true;
        }
    }

    private void Start()
    {
        if (nextLevelMessage != null) nextLevelMessage.SetActive(false);
        if (unlockedVisual != null) unlockedVisual.SetActive(false);
    }

    private void Update()
    {
        bool unlocked = IsUnlocked;

        if (unlockedVisual != null && unlockedVisual.activeSelf != unlocked)
        {
            unlockedVisual.SetActive(unlocked);
        }

        bool show = unlocked && playerColliders > 0;
        if (show == isShowing) return;
        isShowing = show;

        if (nextLevelMessage != null) nextLevelMessage.SetActive(show);
        if (show) Debug.Log("Exit door: next level");
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
