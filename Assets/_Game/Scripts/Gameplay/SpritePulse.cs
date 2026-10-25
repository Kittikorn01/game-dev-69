using UnityEngine;

// Fades a SpriteRenderer's alpha up and down, e.g. a glowing outline.
public class SpritePulse : MonoBehaviour
{
    [SerializeField] private SpriteRenderer target;
    [SerializeField, Range(0f, 1f)] private float minAlpha = 0.2f;
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 1f;
    // full fade cycles per second
    [SerializeField, Min(0.01f)] private float speed = 1.5f;

    private float startTime;

    private void Awake()
    {
        if (target == null) target = GetComponent<SpriteRenderer>();
        if (target == null)
        {
            Debug.LogWarning($"[SpritePulse] No SpriteRenderer: {name}", this);
            enabled = false;
        }
    }

    // restart from the bright end each time the outline is shown
    private void OnEnable()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        float t = (Mathf.Cos((Time.time - startTime) * speed * 2f * Mathf.PI) + 1f) / 2f;
        Color color = target.color;
        color.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        target.color = color;
    }
}
