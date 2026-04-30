using System.Collections.Generic;
using Game.Battle.Entity;
using Global.Data;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    public Vector2 hotspot = Vector2.zero;

    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D transCursor;
    private bool isHold = false;
    private List<IDrag> currentEntities = new ();
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        GameController.Controller.Main.LeftClick.started += Hold;
        GameController.Controller.Main.LeftClick.canceled += Release;
        GameController.Controller.Main.Esc.started += Release;
    }
    /// <summary>
    /// 鼠标按下时切换动画并且发出音效
    /// </summary>
    /// <param name="context"></param>
    void Hold(InputAction.CallbackContext context)
    {
        isHold = true;
        //
        Cursor.SetCursor(transCursor, hotspot, CursorMode.Auto);
        Vector2 screenPos = GameController.Controller.Main.MousePos.ReadValue<Vector2>();
        TryClick(screenPos);
        
    }
    /// <summary>
    /// 鼠标按下时切换动画
    /// </summary>
    /// <param name="context"></param>
    void Release(InputAction.CallbackContext context)
    {
        isHold = false;
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        currentEntities.Clear();
    }
    private void TryClick(Vector2 screenPos)
    {
        if (Camera.main == null)
            return;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
        foreach (var hit in hits)
        {
            var clickObj = hit.collider.GetComponent<IClick>();
            if (clickObj != null)
            {
                
                clickObj.OnClick();
            }
            var dragObj = hit.collider.GetComponent<IDrag>();
            if (dragObj != null)
            {
                currentEntities.Add(dragObj);
            }
        }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; 

    }
    void Update()
    {
        if (isHold && currentEntities.Count>0)
        {
            foreach(var entity in currentEntities)
            {
                entity.OnDrag();
            }
        }
    }
}

