using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Shows story pictures one at a time, fading to black between them, then loads the next scene.
// The next key or a left click goes forward; the skip key jumps straight to the next scene.
public class StorySlideshow : MonoBehaviour
{
    // must be added in File > Build Profiles
    [SerializeField] private string nextScene = "Level1";
    [SerializeField] private Texture[] slides;
    [SerializeField] private RawImage image;
    // optional, pictures change instantly without it
    [SerializeField] private ScreenFader fader;
    [SerializeField] private Key nextKey = Key.Space;
    [SerializeField] private Key skipKey = Key.Escape;
    [SerializeField, Min(0f)] private float slideFadeDuration = 0.5f;
    // input is ignored right after the scene opens, so a key held from the menu does nothing
    [SerializeField, Min(0f)] private float inputDelay = 0.5f;
    // optional: e.g. a "SPACE: next  ESC: skip" text, shown once input is allowed
    [SerializeField] private GameObject hint;

    private int index;
    private float startTime;
    private bool busy;

    private void Start()
    {
        startTime = Time.unscaledTime;
        if (hint != null) hint.SetActive(false);

        // without pictures the scene would wait forever, so go straight on
        if (slides == null || slides.Length == 0)
        {
            Debug.LogWarning("StorySlideshow: no slides assigned, skipping story", this);
            LoadNext();
            return;
        }

        image.texture = slides[0];
    }

    private void Update()
    {
        if (busy || Time.unscaledTime - startTime < inputDelay) return;

        if (hint != null && !hint.activeSelf) hint.SetActive(true);

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;
        if (keyboard != null && keyboard[skipKey].wasPressedThisFrame) LoadNext();
        else if ((keyboard != null && keyboard[nextKey].wasPressedThisFrame) ||
                 (mouse != null && mouse.leftButton.wasPressedThisFrame)) Next();
    }

    private void Next()
    {
        if (index + 1 >= slides.Length) LoadNext();
        else StartCoroutine(ShowSlide(index + 1));
    }

    private IEnumerator ShowSlide(int nextIndex)
    {
        busy = true;
        if (fader != null) yield return fader.FadeTo(1f, slideFadeDuration);

        index = nextIndex;
        image.texture = slides[index];

        if (fader != null) yield return fader.FadeTo(0f, slideFadeDuration);
        busy = false;
    }

    private void LoadNext()
    {
        busy = true;
        if (fader != null) fader.FadeOutAndLoad(nextScene);
        else SceneManager.LoadScene(nextScene);
    }
}
