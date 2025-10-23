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
    [SerializeField]private GameObject particlePrefab;
    
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
    }
    private void TryClick(Vector2 screenPos)
    {
        if (Camera.main == null)
            return;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero);
        foreach (var hit in hits)
        {
            var entity = hit.collider.GetComponent<Entity>();
            if (entity != null)
            {
                currentEntity = entity;
                entity.OnClick();
                break; // 找到第一个可交互物体就退出
            }
        }
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f; // 视你的摄像机决定

        // 实例化粒子
        GameObject go = Instantiate(particlePrefab, GameController.GetWorldMousePos(), Quaternion.identity);

        // 播放粒子系统
        var ps = go.GetComponent<ParticleSystem>();
        ps.Play();

        // 自动销毁（等粒子播完）
        Destroy(go, 0.5f);

    }
    void Update()
    {
        if (isHold && currentEntity != null)
        {
            currentEntity.OnDrag();
        }
    }
}

