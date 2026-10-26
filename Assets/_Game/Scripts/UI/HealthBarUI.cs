using UnityEngine;
using UnityEngine.UI;

// Drives a Filled Image's fillAmount from the player's current HP.
// Listens to Health.HealthChanged, so it only updates when HP actually changes.
// The bar Image must have Image Type = Filled (e.g. Horizontal) for this to show.
public class HealthBarUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private Health health;

    [Tooltip("The fill Image (Image Type = Filled). Leave empty to use the Image on this object.")]
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        if (fillImage == null) fillImage = GetComponent<Image>();
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

    private void Start()
    {
        // Awake order isn't guaranteed across objects; Start runs after every
        // Awake, so the first fill shown on screen is correct.
        if (health != null) UpdateBar(health.CurrentHp, health.MaxHp);
    }

    private void UpdateBar(int current, int max)
    {
        if (fillImage == null) return;
        fillImage.fillAmount = max > 0 ? (float)current / max : 0f;
    }
}
