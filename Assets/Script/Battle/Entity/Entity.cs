using System;
using System.Collections.Generic;
using Global.Data;
using Global.Data.Entity;
using Global.ObjectCreate;
using UnityEngine;
using XLua;

namespace Game.Battle.Entity
{
    [Serializable]
    public class EntityStringPair
    {
        [SerializeField] public string key;
        [SerializeField] public List<Entity> entities; // MonoBehaviour
    }


    public class Entity : MonoBehaviour, IObjectByCreate
    {
        public EntityScriptData scriptData;//储存与随时修改脚本数据
        [SerializeField] public Animator animator;
        [SerializeField] public int dataId;//用于读取数据

        [NonSerialized] public int entityId;//存在实体表里面
        [SerializeField] public CommonEntityData CommonEntityData;//实体通用数据
        [SerializeField] public List<EntityStringPair> entityPairs;
        [NonSerialized] public LuaTable dataTable;//保存lua初始化的数据
        [SerializeField] public SpriteRenderer sr;
        [SerializeField] private GameObject headDamageEffectPrefab; // 修改名字
        [NonSerialized] public Rigidbody2D rb;
        [NonSerialized] public EntityManager entityManager;
        [NonSerialized] public Collider2D col;


        private bool isStop = false;
        public bool entityStop = false;
        string IObjectByCreate.Name
        {
            get => "Entity";
            set => value = "Entity";
        }
        private void Start()
        {
            EventBus.Subscribe<Global.Events.OnOpenSettingUi>(Stop);
            EventBus.Subscribe<Global.Events.OnCloseSettingUi>(CancelStop);
        }
        #region 脚本方法
        public void Init(EntityManager entityManager)
        {
            int id = dataId;
            this.entityManager = entityManager;

            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (col == null) col = GetComponent<Collider2D>();
            if (sr == null) sr = GetComponent<SpriteRenderer>();


            dataTable = LuaManager.Instance._luaEnv.NewTable();
            CommonEntityData =
                GameConfig.Instance.CommonEDC.CommonEntityList.Find(i => i.id == id);
            try
            {
                scriptData =
             GameConfig.Instance.EntitySDC.entityScriptList.Find(i => i.id == CommonEntityData.EffectId);
            }
            catch (Exception ex)
            {
                NotificationManager.Instance.ShowNotification(ex.Message, "实体名字为：" + name + "Id为：" + dataId.ToString());
                SceneChangeManager.Instance.LoadScene("StartScene");
                throw ex;
            }

            foreach (var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }
        void Update()
        {
            if (isStop || entityStop)
            {
                return;
            }
            foreach (var i in scriptData.UpdatePath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, Time.deltaTime, IsStuckInWall());
            }
        }
        public void OnClick()
        {
            if (isStop || entityStop)
            {
                return;
            }
            foreach (var i in scriptData.OnMouseDownPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }
        private void OnDestroy()
        {
            if (isStop || entityStop)
            {
                return;
            }
            if (dataTable != null)
            {
                dataTable.Dispose();
                dataTable = null;
            }
            EventBus.Unsubscribe<Global.Events.OnOpenSettingUi>(Stop);
            EventBus.Unsubscribe<Global.Events.OnCloseSettingUi>(CancelStop);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (isStop || entityStop)
            {
                return;
            }
            var otherEntity = collision.gameObject.GetComponent<Entity>();
            if (otherEntity == null) return;

            Vector2 contactNormal = Vector2.zero;

            if (collision.contacts.Length > 0)
            {
                contactNormal = collision.contacts[0].normal;
            }
            else
            {
                return;
            }

            foreach (var i in scriptData.OnCollisionPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity, contactNormal.x, contactNormal.y);
            }
        }

        public void Dead(Entity entity)
        {
            if (isStop || entityStop)
            {
                return;
            }
            foreach (var i in scriptData.DeadPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, entity);
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (isStop || entityStop)
            {
                return;
            }
            var otherEntity = collision.gameObject.GetComponent<Entity>();
            if (otherEntity == null) return;

            Vector2 contactNormal = Vector2.zero;

            if (collision.contacts.Length > 0)
            {
                contactNormal = collision.contacts[0].normal;
            }
            else
            {
                return;
            }
            foreach (var i in scriptData.OnCollisionPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity, contactNormal.y);
            }
        }
        private bool IsStuckInWall()
        {
            float checkDist = 0.05f;
            LayerMask wallMask = LayerMask.GetMask("Ground");

            Vector2[] dirs = {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right
    };
            int blocked = 0;
            foreach (var dir in dirs)
            {
                var hit = Physics2D.Raycast(transform.position, dir, checkDist, wallMask);
                if (hit.collider != null)
                {
                    var entity = hit.transform.GetComponent<Entity>();
                    if (entity == null)
                    {
                        blocked++;
                        continue;
                    }
                    object canBlockObj = entity.dataTable.Get<object>("canBlock");
                    if (canBlockObj != null && (bool)canBlockObj)
                    {
                        blocked++;
                    }
                  
                }
            }
            return blocked >= 3;
        }
        public void OnDrag()
        {
            if (isStop || entityStop)
            {
                return;
            }
            foreach (var i in scriptData.OnDragPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, Time.deltaTime);
            }
        }
        private void OnDisable()
        {
            if (isStop || entityStop)
            {
                return;
            }
            foreach (var i in scriptData.OnDisablePath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (isStop || entityStop)
            {
                return;
            }
            var otherEntity = collision.gameObject.GetComponent<Entity>();
            if (otherEntity == null) return;

            Vector2 contactNormal = Vector2.zero;

            if (collision.contacts.Length > 0)
            {
                contactNormal = collision.contacts[0].normal;
            }

            foreach (var i in scriptData.OnEntityExitPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity, contactNormal.x, contactNormal.y);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (isStop || entityStop)
            {
                return;
            }
            var otherEntity = collider.gameObject.GetComponent<Entity>();
            if (otherEntity == null) return;

            foreach (var i in scriptData.OnTriggerPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity);
            }
        }

        #endregion
        public List<Entity> GetEntities(string key) => entityPairs.Find(i => i.key == key).entities;
        private void Stop(Global.Events.OnOpenSettingUi eve) => isStop = true;
        private void CancelStop(Global.Events.OnCloseSettingUi eve) => isStop = false;
        public void PlayDamageObj()
        {
            if (headDamageEffectPrefab == null) return;
            if (this.GetComponent<Collider2D>() == null) return;

            Collider2D col = this.GetComponent<Collider2D>();

            Vector3 spawnPos = new Vector3(
                col.bounds.center.x,
                col.bounds.min.y,
                this.transform.position.z
            );

            GameObject particleObj = Instantiate(headDamageEffectPrefab, spawnPos, Quaternion.identity);

            ParticleSystem ps = particleObj.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(particleObj, 1.4f);
            }
        }
    }
}