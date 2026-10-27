using UnityEngine;
using UnityEngine.UI;

// Screen HUD for collecting generators.
// Shows "Press E" when the player is near an un-collected generator, then swaps
// to a radial fill while they hold to collect it. Unfinished progress stays
// shown under the prompt.
// It reads only the generator's PUBLIC state (Progress, IsActivated) and does
// its own distance check, so Generator.cs stays untouched.
public class InteractPromptUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private Transform player;

    [Tooltip("The 'Press E' object. Shown when near a generator, before holding.")]
    [SerializeField] private GameObject pressEPrompt;

    [Tooltip("Radial Image shown while collecting. Its fillAmount is set to the progress.")]
    [SerializeField] private Image progressCircle;

    [Tooltip("Optional. Shown and hidden instead of the circle, e.g. a parent that also holds a ring behind it. Leave empty to toggle the circle itself.")]
    [SerializeField] private GameObject progressRoot;

    [Tooltip("How close (in world units) counts as 'in range'. Tune to match the generator trigger.")]
    [SerializeField, Min(0.1f)] private float interactRange = 2f;

    private Generator[] generators;

    private void Start()
    {
        generators = Object.FindObjectsByType<Generator>(FindObjectsSortMode.None);

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (player == null)
        {
            Debug.LogWarning("InteractPromptUI: no Player set and no object tagged 'Player' found.", this);
        }

        Hide();
    }

    private void Update()
    {
        Generator target = GetNearestInRange();

        if (target == null)
        {
            Hide();
            return;
        }

        // the prompt hides only while E is held; progress kept from an earlier hold
        // stays visible next to it so the player knows to come back and finish
        bool holding = target.IsHolding;
        if (pressEPrompt != null) pressEPrompt.SetActive(!holding);
        SetCircleVisible(holding || target.Progress > 0f);
        if (progressCircle != null) progressCircle.fillAmount = target.Progress;
    }

    private void SetCircleVisible(bool visible)
    {
        GameObject circle = progressRoot != null ? progressRoot
            : progressCircle != null ? progressCircle.gameObject : null;
        if (circle != null && circle.activeSelf != visible) circle.SetActive(visible);
    }

    // Nearest generator that is in range and still off (null if none).
    private Generator GetNearestInRange()
    {
        if (player == null || generators == null) return null;

        Generator nearest = null;
        float best = interactRange * interactRange;

        foreach (Generator g in generators)
        {
            if (g == null || g.IsActivated) continue;

            float sqrDist = ((Vector2)(g.transform.position - player.position)).sqrMagnitude;
            if (sqrDist <= best)
            {
                best = sqrDist;
                nearest = g;
            }
        }

        return nearest;
    }

    private void Hide()
    {
        if (pressEPrompt != null) pressEPrompt.SetActive(false);
        SetCircleVisible(false);
    }
}
