using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EntityManager:MonoBehaviour
{
    public static EntityManager Instance;
    [SerializeField] private Transform canva;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        CreateEntity("Assets/Prefab/Battle/Character/Capsule.prefab", canva);
    }
    private void OnDestroy()
    {
        if(Instance = this)
        Instance = null;
    }
    private readonly Dictionary<int, Entity> entityMap = new Dictionary<int, Entity>();
    private int nextId = 0;
    //��ȡ
    public Entity GetEntity(int id)
    {
        Entity entity;
        if(entityMap.TryGetValue(id,out entity))
        {
            return entity;
        }
        return null;
    }
    //����
    public Entity CreateEntity(string path,Transform parent)
    {
        GameObject ui = ResManager.LoadDataByAsset<GameObject>(path);
        var obj = Object.Instantiate(ui,parent);
        var entity = obj.GetComponent<Entity>();
        Register(entity);
        return entity;
    }
    //�ݻ�
    public void DestroyEntity(int id) 
    {
        var entity = entityMap[id];
        Unregister(entity);
        Object.Destroy(entity);
    }
    //ע��
    private void Register(Entity entity)
    {
        if (entity.entityId == 0)
        {
            entity.entityId = nextId++;
        }
        entityMap[entity.entityId] = entity;
    }
    // ע��
    private void Unregister(Entity entity)
    {
        entityMap.Remove(entity.entityId);
    }
    //����
    public IEnumerable<Entity> GetBuildingValues()
    {
        return entityMap.Values;
    }
}
