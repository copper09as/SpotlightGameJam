using System;
using System.Resources;
using Global.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public Vector2 hotspot = Vector2.zero;

    [SerializeField]private Texture2D defaultCursor;
    [SerializeField] private Texture2D transCursor; 

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        GameController.Controller.Main.LeftClick.started += Hold;
        GameController.Controller.Main.LeftClick.canceled += Release;
    }
    void Hold(InputAction.CallbackContext context)
    {
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
        Cursor.SetCursor(transCursor, hotspot, CursorMode.Auto);
    }
    void Release(InputAction.CallbackContext context)
    {
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
    }

}
