using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;
using UnityEngine.InputSystem;

public class EditorSceneController : MonoBehaviour
{
    [Header("References")]
    public EditorRepository editorRepository;
    [SerializeField] private EntityPreview entityPreview;
    [SerializeField] private LevelEditor levelEditor;
    [SerializeField] private float cellSize = 0.3f;
    private Entity currentEntity;
    private LevelEntityData levelEntityData;
    private GameObject currentEntityPrefab;
    private bool isHoldingEntity = false;

    void Start()
    {
        if (entityPreview != null && editorRepository != null)
        {
            entityPreview.LoadEntities(editorRepository);
            entityPreview.OnEntitySelected += OnEntitySelected;
        }
        levelEditor.Init(cellSize);

    }

    void Update()
    {
        if (isHoldingEntity && currentEntityPrefab != null)
        {
            Vector3 worldMousePos = GameController.GetWorldMousePos();
            Vector3Int gridPos = WorldToGrid(worldMousePos);
            Vector3 snappedPos = GridToWorld(gridPos);

            currentEntityPrefab.transform.position = snappedPos;

            // 更新预览
            if (levelEntityData != null)
            {
                Vector2Int entitySize = levelEntityData.gridSize;
                levelEditor.UpdateEntityPreview(gridPos, entitySize);
            }


            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlaceCurrentEntity(gridPos);
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }
        }
    }

    private void OnEntitySelected(GameObject entityPrefab, int effectId, int height, int width)
    {
        // 取消之前的放置
        if (isHoldingEntity)
        {
            CancelPlacement();
        }

        if (entityPrefab != null)
        {
            // 实例化生成
            currentEntityPrefab = Instantiate(entityPrefab);
            currentEntity = currentEntityPrefab.GetComponent<Entity>();
            if (currentEntity == null)
            {
                currentEntity = currentEntityPrefab.GetComponentInChildren<Entity>();
            }
            isHoldingEntity = true;
            levelEntityData = new LevelEntityData
            {
                EffectId = effectId,
                gridSize = new Vector2Int(width, height)
            };
            Debug.Log($"生成实体: {entityPrefab.name}, 跟随鼠标中...");
        }
        else
        {
            currentEntity = null;
            isHoldingEntity = false;
            Debug.Log("未找到预制体");
        }
    }

    private void PlaceCurrentEntity(Vector3Int gridPos)
    {
        if (currentEntity == null || levelEditor == null || levelEntityData == null) return;
        levelEntityData.gridPosition = new Vector2Int(gridPos.x, gridPos.y);

        // 调用LevelEditor的放置方法
        if (levelEditor.PlaceEntity(levelEntityData, gridPos, currentEntity))
        {
            isHoldingEntity = false;
            currentEntity = null;
            currentEntityPrefab = null;
            levelEntityData = null;
        }

        // 放置成功后，保留实体但不再跟随鼠标（或者继续跟随以放置多个）
        // 如果要继续放置多个：
        // currentEntityPrefab = Instantiate(currentEntityPrefab);
        // currentEntity = currentEntityPrefab.GetComponent<Entity>();

        // 如果只放置一个：

    }

    private void CancelPlacement()
    {
        if (currentEntityPrefab != null)
        {
            Destroy(currentEntityPrefab);
        }

        currentEntity = null;
        currentEntityPrefab = null;
        isHoldingEntity = false;
    }

    private Vector3Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / cellSize);
        int y = Mathf.FloorToInt(worldPos.y / cellSize);
        return new Vector3Int(x, y, 0);
    }

    private Vector3 GridToWorld(Vector3Int gridPos)
    {
        float x = gridPos.x * cellSize + cellSize * 0.5f;
        float y = gridPos.y * cellSize + cellSize * 0.5f;
        return new Vector3(x, y, 0);
    }
}