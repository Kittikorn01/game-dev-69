using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Fades a full-screen black image in or out.
// Uses unscaled time so it still works when the game is paused.
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField, Min(0f)] private float duration = 1.5f;
    // start black and fade to clear when the scene opens
    [SerializeField] private bool fadeInOnStart;

    private CanvasGroup canvasGroup;
    private bool loading;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        SetAlpha(fadeInOnStart ? 1f : 0f);
    }

    private void Start()
    {
        if (fadeInOnStart) StartCoroutine(Fade(1f, 0f, duration));
    }

    // for callers that run their own sequence, e.g. yield return fader.FadeTo(1f, 0.5f)
    public IEnumerator FadeTo(float alpha, float seconds)
    {
        // stops the scene-start fade so two fades don't fight over the alpha
        StopAllCoroutines();
        return Fade(canvasGroup.alpha, alpha, seconds);
    }

    public void FadeOutAndLoad(string sceneName)
    {
        if (loading) return;
        loading = true;
        StopAllCoroutines();
        StartCoroutine(FadeOutThenLoad(sceneName));
    }

    private IEnumerator FadeOutThenLoad(string sceneName)
    {
        yield return Fade(canvasGroup.alpha, 1f, duration);
        // unpause only once the screen is black, so a paused game doesn't move behind the fade
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float from, float to, float seconds)
    {
        for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)
        {
            SetAlpha(Mathf.Lerp(from, to, t / seconds));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
        // block clicks while visible, so buttons under it can't be pressed mid-fade
        canvasGroup.blocksRaycasts = alpha > 0f;
    }
}
