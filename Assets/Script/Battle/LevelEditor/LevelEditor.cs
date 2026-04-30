using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

public class LevelEditor : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 1f;
    
    [Header("Tilemap")]
    [SerializeField] public Tilemap tilemap;
    [SerializeField] public TileBase tile;
    [SerializeField] private List<LevelEntityData> levelEntityDataList = new();
    private EditorStateMachine stateMachine;
    void Start()    {
        stateMachine = new EditorStateMachine(this);
    }
    void Update()
    {
        // 获取鼠标世界坐标
        Vector3 worldMousePos = GameController.GetWorldMousePos();
        
        // 转换为网格坐标
        Vector3Int gridPos = tilemap.WorldToCell(worldMousePos);
        if (Mouse.current.leftButton.wasPressedThisFrame)
            stateMachine.OnTrigger(gridPos);
    }
   public void PlaceEntity(LevelEntityData newEntity,Vector3Int gridPos)
    {
        if(!CanPlace(newEntity))return;
        if(newEntity.Id==-1)
        {
            tilemap.SetTile(gridPos, tile);
        }
        levelEntityDataList.Add(newEntity);
    }
    private bool CanPlace(LevelEntityData newEntity)
    {
                // 检查每个网格是否被占用
        List<Vector2Int> occupiedGrids = newEntity.GetOccupiedGrids();
        
        foreach (var grid in occupiedGrids)
        {
            // Find查找是否有实体占据了该网格
            LevelEntityData findEntity = levelEntityDataList.Find(e => e.OccupiesGrid(grid));
            if (findEntity != null)
            {
                Debug.Log($"网格 {grid} 已被实体 {findEntity.Id} 占用");
                return false;
            }
        }
        return true;
    }
    
}