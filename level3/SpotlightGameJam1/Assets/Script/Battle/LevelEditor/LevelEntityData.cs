using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelEntityData
{
    public int Id;
    public int EffectId;
    
    // 网格位置信息（中心点）
    public Vector2Int gridPosition;
    public Vector2Int gridSize;
    
    // 获取左下角起始位置
    private Vector2Int GetBottomLeft()
    {
        int halfX = gridSize.x / 2;
        int halfY = gridSize.y / 2;
        return new Vector2Int(gridPosition.x - halfX, gridPosition.y - halfY);
    }
    
    // 获取所有被占据的网格坐标
    public List<Vector2Int> GetOccupiedGrids()
    {
        List<Vector2Int> occupied = new List<Vector2Int>();
        Vector2Int bottomLeft = GetBottomLeft();
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                occupied.Add(new Vector2Int(
                    bottomLeft.x + x, 
                    bottomLeft.y + y
                ));
            }
        }
        
        return occupied;
    }
    
    // 检查是否占据某个网格
    public bool OccupiesGrid(Vector2Int gridPos)
    {
        Vector2Int bottomLeft = GetBottomLeft();
        return gridPos.x >= bottomLeft.x &&
               gridPos.x < bottomLeft.x + gridSize.x &&
               gridPos.y >= bottomLeft.y &&
               gridPos.y < bottomLeft.y + gridSize.y;
    }
}