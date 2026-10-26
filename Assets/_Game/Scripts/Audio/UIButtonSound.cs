using UnityEngine;
using UnityEngine.UI;

// Adds the same click sound to every Button under this object (including hidden ones).
// Put it on a Canvas. The sound plays from a player that survives scene changes,
// so buttons like Start or Restart do not cut the click off when the next scene loads.
public class UIButtonSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;
    [SerializeField, Range(0f, 1f)] private float volume = 0.8f;

    private static AudioSource sharedSource;

    private void Awake()
    {
        foreach (Button button in GetComponentsInChildren<Button>(true))
        {
            button.onClick.AddListener(PlayClick);
        }
    }

    private void PlayClick()
    {
        if (clickClip == null) return;
        GetSharedSource().PlayOneShot(clickClip, volume);
    }

    private static AudioSource GetSharedSource()
    {
        if (sharedSource == null)
        {
            GameObject player = new GameObject("UIButtonSoundPlayer");
            DontDestroyOnLoad(player);
            sharedSource = player.AddComponent<AudioSource>();
            sharedSource.playOnAwake = false;
            sharedSource.spatialBlend = 0f;
            // keep clicking audible while the game is paused (e.g. on the game over screen)
            sharedSource.ignoreListenerPause = true;
        }
        return sharedSource;
    }
}
