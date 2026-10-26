using System.Collections;
using UnityEngine;

// Fades a CanvasGroup in unscaled time, so it still works while the game is paused.
// Use from a coroutine: yield return CanvasGroupFade.FadeIn(group, 0.5f);
public static class CanvasGroupFade
{
    public static IEnumerator FadeIn(CanvasGroup group, float seconds)
    {
        for (float t = 0f; t < seconds; t += Time.unscaledDeltaTime)
        {
            group.alpha = t / seconds;
            yield return null;
        }
        group.alpha = 1f;
    }
}
