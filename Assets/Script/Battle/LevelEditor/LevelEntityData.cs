using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEntityData
{
    public int Id;
    public int EffectId;
    
    // 网格位置信息
    public Vector2Int gridPosition;  // 实体左下角/中心所在的网格坐标
    public Vector2Int gridSize;      // 实体占据的网格数量（宽x高）
    
    // 获取所有被占据的网格坐标
    public List<Vector2Int> GetOccupiedGrids()
    {
        List<Vector2Int> occupied = new List<Vector2Int>();
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                occupied.Add(new Vector2Int(
                    gridPosition.x + x, 
                    gridPosition.y + y
                ));
            }
        }
        
        return occupied;
    }
    
    // 检查是否占据某个网格
    public bool OccupiesGrid(Vector2Int gridPos)
    {
        return gridPos.x >= gridPosition.x &&
               gridPos.x < gridPosition.x + gridSize.x &&
               gridPos.y >= gridPosition.y &&
               gridPos.y < gridPosition.y + gridSize.y;
    }
}