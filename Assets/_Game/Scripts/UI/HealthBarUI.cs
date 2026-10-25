using UnityEngine;

// Drives the HPBar width from the player's current HP.
// Listens to Health.HealthChanged, so it only updates when HP actually changes.
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private Health health;

    private RectTransform barRect;
    private float fullWidth;

    private void Awake()
    {
        barRect = (RectTransform)transform;

        // Remember the designed width, then anchor the bar to its left edge so it
        // drains right-to-left. The pivot shift is compensated so the bar does
        // not move on screen.
        fullWidth = barRect.rect.width;

        Vector2 oldPivot = barRect.pivot;
        Vector2 newPivot = new Vector2(0f, oldPivot.y);
        Vector2 size = barRect.rect.size;
        Vector2 deltaPivot = newPivot - oldPivot;
        barRect.anchoredPosition += new Vector2(deltaPivot.x * size.x, deltaPivot.y * size.y);
        barRect.pivot = newPivot;
    }

    private void OnEnable()
    {
        if (health == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) health = player.GetComponent<Health>();
        }

        if (health == null)
        {
            Debug.LogWarning("HealthBarUI: no Health set and no object tagged 'Player' found.", this);
            return;
        }

        health.HealthChanged += UpdateBar;
        UpdateBar(health.CurrentHp, health.MaxHp);
    }

    private void OnDisable()
    {
        if (health != null) health.HealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        float ratio = max > 0 ? (float)current / max : 0f;
        barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * ratio);
    }
}
