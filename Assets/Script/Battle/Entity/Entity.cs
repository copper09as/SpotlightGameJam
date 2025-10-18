using System;
using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.Entity;
using Global.ObjectCreate;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using XLua;

namespace Game.Battle.Entity
{
    [Serializable]
    public class EntityStringPair
    {
        [SerializeField] public string key;
        [SerializeField] public List<Entity> entities; // MonoBehaviour
    }


    public class Entity : MonoBehaviour,IObjectByCreate
    {
        [SerializeField]private EntityScriptData scriptData;//储存与随时修改脚本数据
        [SerializeField]public Animator animator;
        [SerializeField] private int dataId;//用于读取数据
        [SerializeField] public SpriteRenderer sr;
        public int entityId;//存在实体表里面
        [SerializeField]public CommonEntityData CommonEntityData;//实体通用数据
        [NonSerialized] public LuaTable dataTable;//保存lua初始化的数据
        [SerializeField]public List<EntityStringPair> entityPairs;
        [NonSerialized] public Rigidbody2D rb;
        public TextMeshProUGUI text;
        [NonSerialized] public EntityManager entityManager;
        [NonSerialized] public Collider2D col;
        string IObjectByCreate.Name 
        { get => "Entity";
            set => value = "Entity"; }

        #region 脚本方法
        public void Init(EntityManager entityManager)//,EntityManager entityManager)
        {
            int id = dataId;
            this.entityManager = entityManager;
            if(rb == null) rb = GetComponent<Rigidbody2D>();
            if(col==null) col = GetComponent<Collider2D>();
            if(sr==null)sr = GetComponent<SpriteRenderer>();

             
            dataTable = LuaManager.Instance._luaEnv.NewTable();
            CommonEntityData = 
                GameConfig.Instance.CommonEDC.CommonEntityList.Find(i => i.id == id);
            scriptData = 
                GameConfig.Instance.EntitySDC.entityScriptList.Find(i => i.id == CommonEntityData.EffectId);
            
            foreach (var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }


        void Update()
        {
            foreach (var i in scriptData.UpdatePath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this,Time.deltaTime);
            }

        }
        public void OnClick()
        {
            foreach (var i in scriptData.OnMouseDownPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }
        private void OnDestroy()
        {
            if (dataTable != null)
            {
                dataTable.Dispose();
                dataTable = null;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
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
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity, contactNormal.x,contactNormal.y);
            }
        }

        public void Dead(Entity entity)
        {
            foreach (var i in scriptData.DeadPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this,entity);
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
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

        public void OnDrag()
        {
            foreach (var i in scriptData.OnDragPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this,Time.deltaTime);
            }
        }
        private void OnDisable()
        {
            foreach (var i in scriptData.OnDisablePath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this);
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
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

            foreach (var i in scriptData.OnEntityExitPath)
            {
                LuaManager.Instance.CallFunction(i, Tool.GetLuaName(i), this, otherEntity, contactNormal.x, contactNormal.y);
            }
        }

        #endregion
        public List<Entity> GetEntities(string key) => entityPairs.Find(i => i.key == key).entities;
    }
}