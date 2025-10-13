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
    private Entity currentEntity;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Cursor.SetCursor(defaultCursor, hotspot, CursorMode.Auto);
        GameController.Controller.Main.LeftClick.started += Hold;
        GameController.Controller.Main.LeftClick.canceled += Release;
    }
    /// <summary>
    /// 鼠标按下时切换动画并且发出音效
    /// </summary>
    /// <param name="context"></param>
    void Hold(InputAction.CallbackContext context)
    {
        isHold = true;
        AudioManager.Instance.PlaySFX(StringResource.LeftClickSfxPath);
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
    }
    private void TryClick(Vector2 screenPos)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            var entity = hit.collider.GetComponent<Entity>();
            Debug.Log($"点击到：{hit.collider.name}");
            if (entity != null)
            {
                currentEntity = entity;
                entity.OnClick();
            }
        }
        else
        {
            currentEntity = null;
        }
    }
    void Update()
    {
        if (isHold && currentEntity != null)
        {
            currentEntity.OnDrag();
        }
    }
}

