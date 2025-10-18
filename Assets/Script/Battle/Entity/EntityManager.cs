using System;
using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using Unity.VisualScripting;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EntityManager:MonoBehaviour
{
    private readonly Dictionary<int, Entity> entityMap = new Dictionary<int, Entity>();
    private int nextId = 0;
    [SerializeField] private bool autoInit;
    private void Start()
    {
        if(autoInit)
        {
            CreateEntity("Assets/Prefab/Battle/Character/Capsule.prefab",null);
            foreach (var entity in FindObjectsOfType<Entity>(true))
            {
                entity.Init(this);
            }
        }

    }
    //获取
    public Entity GetEntity(int id)
    {
        Entity entity;
        if(entityMap.TryGetValue(id,out entity))
        {
            return entity;
        }
        return null;
    }
    public Entity InstantiateEnityty(GameObject prefab,Transform parent)
    {
        var obj = Instantiate(prefab, parent);
        var entity = obj.GetComponent<Entity>();
        Register(entity);
        return entity;
    }
    //建造
    public Entity CreateEntity(string path,Transform parent)
    {
        GameObject ui = ResManager.LoadDataByAsset<GameObject>(path);
        return InstantiateEnityty(ui, parent);
    }
    //摧毁
    public void DestroyEntity(int id) 
    {
        var entity = entityMap[id];
        Unregister(entity);
        Destroy(entity);
    }
    //注册
    public void Register(Entity entity)
    {
        if (entity.entityId == 0)
        {
            entity.entityId = nextId++;
        }
        entityMap[entity.entityId] = entity;
    }
    // 注销
    private void Unregister(Entity entity)
    {
        entityMap.Remove(entity.entityId);
    }
    //遍历
    public List<Entity> GetAllEntities()
    {
        return new List<Entity>(entityMap.Values);
    }
    public void CallNextFrame(Action action)
    {
        StartCoroutine(CallNextFrameCoroutine(action));
    }

    private IEnumerator CallNextFrameCoroutine(Action action)
    {
        yield return null;
        action?.Invoke();
    }
}
