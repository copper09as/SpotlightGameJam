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
    public class Entity : MonoBehaviour,IObjectByCreate
    {
        [SerializeField]private EntityScriptData scriptData;//储存与随时修改脚本数据
        [SerializeField]private Animator animator;
        [SerializeField] private int dataId;//用于读取数据
        public int entityId;//存在实体表里面
        [SerializeField]public CommonEntityData CommonEntityData;//实体通用数据
        [NonSerialized] public LuaTable dataTable;//保存lua初始化的数据
        public Rigidbody2D rb;
        public TextMeshProUGUI text;
        public EntityManager entityManager;
        public Collider2D col;
        string IObjectByCreate.Name 
        { get => "Entity";
            set => value = "Entity"; }
        private void Start()
        {
            Init(dataId);
        }
        #region 脚本方法
        public void Init(int id)//,EntityManager entityManager)
        {
            dataTable = LuaManager.Instance._luaEnv.NewTable();

            //this.entityManager = entityManager;
            GameController.Controller.Main.Space.started += OnSpace;
            CommonEntityData = 
                GameConfig.Instance.CommonEDC.CommonEntityList.Find(i => i.id == id);
            Debug.Log(CommonEntityData.EffectId);
            scriptData = 
                GameConfig.Instance.EntitySDC.entityScriptList.Find(i => i.id == CommonEntityData.EffectId);
            
            foreach (var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
        }

        private void OnSpacePress()
        {
            foreach (var i in scriptData.OnSpacePressPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
        }

        void Update()
        {
            if (GameController.isSpacePressed)
            {
                OnSpacePress();
            }
            foreach (var i in scriptData.UpdatePath)
            {
                LuaManager.Instance.CallFunction(i, i, this,Time.deltaTime);
            }

        }
        private void OnSpace(InputAction.CallbackContext context)
        {
            foreach (var i in scriptData.OnSpacePath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
        }
        public void OnClick()
        {
            foreach (var i in scriptData.OnMouseDownPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
        }
        private void OnDisable()
        {

            if (dataTable != null)
            {
                dataTable.Dispose();
                dataTable = null;
            }
            GameController.Controller.Main.Space.started-= OnSpace;
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
                LuaManager.Instance.CallFunction(i, i, this, otherEntity, contactNormal.y);
            }
        }

        public void Dead()
        {
            foreach (var i in scriptData.DeadPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
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
                LuaManager.Instance.CallFunction(i, i, this, otherEntity, contactNormal.y);
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {

        }

        public void OnDrag()
        {
            foreach (var i in scriptData.OnDragPath)
            {
                LuaManager.Instance.CallFunction(i, i, this,Time.deltaTime);
            }
        }

        #endregion
        #region 向lua公开的方法
        /// <summary>
        /// 若有动画则设置动画
        /// </summary>
        /// <param name="aniName"></param>
        public void SetAnimation(string aniName)
        {
            if (animator == null)
                return;

            bool hasTrigger = false;
            foreach (var param in animator.parameters)
            {
                if (param.type == AnimatorControllerParameterType.Trigger && param.name == aniName)
                {
                    hasTrigger = true;
                    break;
                }
            }
            if (!hasTrigger)
                return;

            animator.SetTrigger(aniName);
        }
        //[SerializeField]private bool isGrounded = false;
       // public bool GroundCheck()
        //{
           // return isGrounded;
        //}
        #endregion


    }
}