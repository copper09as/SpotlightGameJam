using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;

public class EditorRepository : MonoBehaviour
{
    public List<GameObject> EntityPrefab;
    
    // 根据ID获取预制体
    public GameObject GetEntityPrefab(int id)
    {
        if (id >= 0 && id < EntityPrefab.Count)
        {
            return EntityPrefab[id];
        }
        return null;
    }
    
    // 获取预制体数量
    public int GetEntityCount()
    {
        return EntityPrefab.Count;
    }
    
    // 获取预制体的Sprite（如果有SpriteRenderer组件）
    public Sprite GetEntitySprite(int id)
    {
        GameObject prefab = GetEntityPrefab(id);
        if (prefab != null)
        {
            SpriteRenderer sr = prefab.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                return sr.sprite;
            }
        }
        return null;
    }
    
    // 获取预制体的大小信息（如果实体脚本有Size属性）
    public Vector2Int GetEntitySize(int id)
    {
        GameObject prefab = GetEntityPrefab(id);
        if (prefab != null)
        {
            // 假设你的实体脚本叫Entity或类似的名字
            var entityComponent = prefab.GetComponent<Entity>(); // 替换为实际的实体脚本
            if (entityComponent != null)
            {
                // 返回实体大小，可能需要你自己定义
                return new Vector2Int(1, 1);
            }
        }
        return new Vector2Int(1, 1);
    }
}