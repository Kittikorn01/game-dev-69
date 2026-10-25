using UnityEngine;
using TMPro;

// Shows the player's current ammo count on a TMP label.
// Listens to PlayerAmmo.AmmoChanged, which fires each time a shot is consumed.
public class AmmoTextUI : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to find the object tagged Player.")]
    [SerializeField] private PlayerAmmo ammo;
    [Tooltip("Optional. Leave empty to use the TMP_Text on this object.")]
    [SerializeField] private TMP_Text label;

    private void Awake()
    {
        if (label == null) label = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        if (ammo == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) ammo = player.GetComponent<PlayerAmmo>();
        }

        if (ammo == null)
        {
            Debug.LogWarning("AmmoTextUI: no PlayerAmmo set and no object tagged 'Player' found.", this);
            return;
        }

        ammo.AmmoChanged += UpdateLabel;
        UpdateLabel(ammo.Current);
    }

    private void OnDisable()
    {
        if (ammo != null) ammo.AmmoChanged -= UpdateLabel;
    }

    // Awake order isn't guaranteed across objects, so OnEnable can run before
    // PlayerAmmo.Awake has set Current. Start always runs after every Awake, so
    // the first value shown on screen is correct (not a stale 0).
    private void Start()
    {
        if (ammo != null) UpdateLabel(ammo.Current);
    }

    private void UpdateLabel(int current)
    {
        if (label != null) label.text = current.ToString();
    }
}
