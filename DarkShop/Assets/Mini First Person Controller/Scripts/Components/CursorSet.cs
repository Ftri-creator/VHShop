using UnityEngine;

public class CursorSet : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTex;
    private Vector2 hotspot;

    private void Start()
    {
        hotspot = new Vector2(cursorTex.width / 1.5f, cursorTex.height / 1.5f);
        Cursor.SetCursor(cursorTex, hotspot, CursorMode.Auto);
    }
}
