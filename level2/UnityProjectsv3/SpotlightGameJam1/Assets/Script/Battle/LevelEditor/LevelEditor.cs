using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using Game.Battle.Entity;

public class LevelEditor : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 0.5f;
    [SerializeField] private Vector2Int gridAreaSize = new Vector2Int(20, 20);  // 网格区域大小
    
    [Header("Tilemap")]
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public TileBase tile;
    
    [Header("Visual Feedback")]
    [SerializeField] private Tilemap previewTilemap;
    [SerializeField] private TileBase previewTile;
    [SerializeField] private TileBase invalidTile;
    [SerializeField] private TileBase gridLineTile;          // 网格线Tile（白色半透明）
    
    [SerializeField] private List<LevelEntityData> levelEntityDataList = new();
    private Vector3Int lastGridPos;
    private bool lastPlacementValid = true;
    
    public void Init(float cellSize)
    {
        this.cellSize = cellSize;
        
        // 初始化预览Tilemap
        if (previewTilemap != null)
        {
            Grid grid = previewTilemap.GetComponent<Grid>();
            if (grid == null)
                grid = previewTilemap.gameObject.AddComponent<Grid>();
            grid.cellSize = new Vector3(cellSize, cellSize, 1);
        }
        
        // 同步主Tilemap
        if (tilemap != null)
        {
            Grid mainGrid = tilemap.GetComponent<Grid>();
            if (mainGrid == null)
                mainGrid = tilemap.gameObject.AddComponent<Grid>();
            mainGrid.cellSize = new Vector3(cellSize, cellSize, 1);
        }
        
        RefreshAllPreviews();
    }

    
    // 刷新所有预览显示
    private void RefreshAllPreviews()
    {
        if (previewTilemap == null) return;
        
        previewTilemap.ClearAllTiles();
        
        // 先显示所有有效网格（淡淡的基础色）
        // 再覆盖已占据的网格（绿色）
        foreach (var entity in levelEntityDataList)
        {
            List<Vector2Int> occupiedGrids = entity.GetOccupiedGrids();
            foreach (var grid in occupiedGrids)
            {
                Vector3Int tilePos = new Vector3Int(grid.x, grid.y, 0);
                previewTilemap.SetTile(tilePos, previewTile);
            }
        }
    }
    
    // 更新鼠标预览（显示当前要放置的实体占据网格）
    public void UpdateEntityPreview(Vector3Int gridPos, Vector2Int entitySize)
    {
        // 清除之前的动态预览，但保留已放置实体的预览
        RefreshAllPreviews();
        
        LevelEntityData tempEntity = new LevelEntityData
        {
            Id = -1,
            gridPosition = new Vector2Int(gridPos.x, gridPos.y),
            gridSize = entitySize
        };
        
        bool canPlace = CanPlace(tempEntity);
        TileBase currentPreviewTile = canPlace ? previewTile : invalidTile;
        
        List<Vector2Int> occupiedGrids = tempEntity.GetOccupiedGrids();
        
        foreach (var grid in occupiedGrids)
        {
            Vector3Int tilePos = new Vector3Int(grid.x, grid.y, 0);
            previewTilemap.SetTile(tilePos, currentPreviewTile);
        }
    }
    
    void ClearPreview()
    {
        if (previewTilemap != null)
        {
            previewTilemap.ClearAllTiles();
        }
    }
    
    public bool PlaceEntity(LevelEntityData newEntity, Vector3Int gridPos, Entity entity)
    {
        if(!CanPlace(newEntity)) return false;
        
        levelEntityDataList.Add(newEntity);
        
        if (entity != null)
        {
            entity.dataId = newEntity.EffectId;
            entity.transform.position = new Vector3(
                gridPos.x * cellSize + cellSize * 0.5f,
                gridPos.y * cellSize + cellSize * 0.5f,
                0
            );
        }
        
        RefreshAllPreviews();
        return true;
    }
    
    public void RemoveEntity(Vector2Int gridPos)
    {
        LevelEntityData entity = levelEntityDataList.Find(e => e.OccupiesGrid(gridPos));
        if (entity != null)
        {
            levelEntityDataList.Remove(entity);
            RefreshAllPreviews();
        }
    }
    
    private bool CanPlace(LevelEntityData newEntity)
    {
        List<Vector2Int> occupiedGrids = newEntity.GetOccupiedGrids();
        
        foreach (var grid in occupiedGrids)
        {
            LevelEntityData findEntity = levelEntityDataList.Find(e => e.OccupiesGrid(grid));
            if (findEntity != null)
            {
                return false;
            }
        }
        return true;
    }
}