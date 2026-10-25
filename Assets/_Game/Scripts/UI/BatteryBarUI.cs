using UnityEngine;

// Drives the flashlight battery bar width from the player's current battery.
// Listens to PlayerBattery.BatteryChanged, which fires while the flashlight drains it.
public class BatteryBarUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private PlayerBattery battery;

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
        if (battery == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) battery = player.GetComponent<PlayerBattery>();
        }

        if (battery == null)
        {
            Debug.LogWarning("BatteryBarUI: no PlayerBattery set and no object tagged 'Player' found.", this);
            return;
        }

        battery.BatteryChanged += UpdateBar;
        UpdateBar(battery.Normalized);
    }

    private void OnDisable()
    {
        if (battery != null) battery.BatteryChanged -= UpdateBar;
    }

    private void UpdateBar(float normalized)
    {
        barRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, fullWidth * Mathf.Clamp01(normalized));
    }
}
