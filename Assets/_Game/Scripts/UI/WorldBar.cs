using UnityEngine;

// A bar made of SpriteRenderers in the world; shrinks the fill toward its left edge.
public class WorldBar : MonoBehaviour
{
    // fill sprite has a centered pivot, so it is moved as well as scaled
    [SerializeField] private SpriteRenderer fill;

    private float fullWidth;
    private float leftEdge;

    private void Awake()
    {
        if (fill == null)
        {
            Debug.LogWarning($"[WorldBar] Fill not set: {name}", this);
            enabled = false;
            return;
        }

        fullWidth = fill.transform.localScale.x;
        leftEdge = fill.transform.localPosition.x - fullWidth / 2f;
    }

    public void SetValue(float value01)
    {
        if (!enabled) return;

        float width = fullWidth * Mathf.Clamp01(value01);
        Transform fillTransform = fill.transform;

        Vector3 scale = fillTransform.localScale;
        scale.x = width;
        fillTransform.localScale = scale;

        Vector3 position = fillTransform.localPosition;
        position.x = leftEdge + width / 2f;
        fillTransform.localPosition = position;
    }

    public void SetColor(Color color)
    {
        if (!enabled) return;
        fill.color = color;
    }
}
