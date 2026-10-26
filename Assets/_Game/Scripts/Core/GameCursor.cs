using UnityEngine;

public class GameCursor : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Vector2 hotspot = new Vector2(16f, 16f);

    private void OnEnable()
    {
        if (cursorTexture == null)
        {
            Debug.LogWarning("GameCursor: no cursor texture set.", this);
            return;
        }

        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }

    private void OnDisable()
    {
        // Restore the OS cursor when leaving the scene (e.g. back to the main menu)
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}
