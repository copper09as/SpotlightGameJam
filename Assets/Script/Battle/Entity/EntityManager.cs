using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Battle.Entity;
using Global.Data.BattleConfig;

public class EntityManager
{
    private readonly Dictionary<int, Entity> entityMap = new Dictionary<int, Entity>();
    private int nextId = 0;
    //获取
    public Entity GetEntity(int id)
    {
        Entity entity;
        if (entityMap.TryGetValue(id, out entity))
        {
            return entity;
        }
        return null;
    }
    public List<Entity> GetEntitiesByDataId(int dataId)
    {
        return GetAllEntities().Where(i => i.dataId == dataId).ToList();
    }
    public void Lose(float delay)
    {
        foreach (var entity in GetAllEntities())
        {
            entity.entityStop = true;
        }
        if (BattleConfig.Instance.DeadMode)
        {
            BattleConfig.Instance.ClearLevel();
            SceneChangeManager.Instance.LoadSceneWithDelay("StartScene", delay);
            BattleConfig.Instance.DeadMode = false;
            NotificationManager.Instance.ShowNotification("死亡模式失败，存档已清空，并且死亡模式已自动关闭，请重新挑战！", "死亡模式失败，请重新挑战！");
        }
        else
        {
            SceneChangeManager.Instance.ReloadSceneWithDelay(delay);
        }
        
    }
    public void Win(float delay)
    {
        foreach (var entity in GetAllEntities())
        {
            entity.entityStop = true;
        }
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/OpenDoor.wav");
        BattleConfig.Instance.Win(delay);
    }
    public List<Entity> FindEntityByDataTable(string key)
    {
        var result = new List<Entity>();

        foreach (var entity in GetAllEntities())
        {
            if (entity == null || entity.dataTable == null)
                continue;

            bool value;
            try
            {
                value = entity.dataTable.Get<bool>(key);
            }
            catch
            {
                continue;
            }

            if (value)
            {
                result.Add(entity);
            }
        }

        return result;
    }


    /*public Entity InstantiateEnityty(GameObject prefab,Transform parent)
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
    }*/
    //摧毁
    /*public void DestroyEntity(int id) 
    {
        var entity = entityMap[id];
        Unregister(entity);
        Destroy(entity);
    }*/
    //注册
    public void Register(Entity entity)
    {
        if (entity.entityId == 0)
        {
            entity.entityId = nextId++;
        }
        entityMap[entity.entityId] = entity;
        entity.Init(this);
        if(BattleConfig.Instance.DragMode)
        {
            if(!entity.scriptData.OnDragPath.Contains("Mouse/FollowMouseY")
                && !entity.scriptData.OnDragPath.Contains("Mouse/FollowMouseX")
                &&!entity.scriptData.OnDragPath.Contains("Mouse/FollowMouse"))
            {
                entity.scriptData.OnDragPath.Add("Mouse/FollowMouse");
            }
            
        }
        if(BattleConfig.Instance.EverythingMoveMode)
        {
            if (!entity.scriptData.UpdatePath.Contains("Character/CharacterMove"))
            {
                entity.scriptData.UpdatePath.Add("Position/EverythingMove");
            } 
           
        }
        if(BattleConfig.Instance.PoisionMode)
        {
          entity.scriptData.OnCollisionPath.Add("Damage/DamageEntity");
        }
        if(BattleConfig.Instance.DontDeadMode)
        {
            if(entity.scriptData.InitPath.Contains("Character/CharacterInit"))
            {
                entity.scriptData.DeadPath.Clear();
            }
        }
    }
    // 注销
    public void Unregister(Entity entity)
    {
        entityMap.Remove(entity.entityId);
    }
    //遍历

    public List<Entity> GetAllEntities()
    {
        return new List<Entity>(entityMap.Values);
    }

}
