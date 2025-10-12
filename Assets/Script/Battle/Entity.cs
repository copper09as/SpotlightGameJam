using System;
using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.Entity;
using Global.ObjectCreate;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
namespace Game.Battle.Entity
{
    public class Entity : MonoBehaviour,IObjectByCreate
    {
        private EntityScriptData scriptData = new();
        private Animator animator;
        [SerializeField] private int id;
        public CharacterEntityData CharacterData;
        public Rigidbody2D rb;
        public TextMeshProUGUI text;
        public Collider2D col;
        string IObjectByCreate.Name 
        { get => "Entity";
            set => value = "Entity"; }
        private void Awake()
        {
            id = 10000;
            GameController.Controller.Main.Space.started += OnSpace;
            CharacterData = GameConfig.Instance.CharacterDT.ToEntityData();
            foreach (var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
            scriptData = GameConfig.Instance.EntitySDC.entityScriptList.Find(i => i.id == id);
        }
        #region 脚本方法
        public void Init(EntityScriptData scriptData)
        {
            this.scriptData = scriptData;
            foreach(var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
        }
        void Update()
        {
            foreach (var i in scriptData.UpdatePath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
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
            foreach (var i in scriptData.OnDestroyPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
            CharacterData.signId = -1;
        }
        #endregion
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
        [SerializeField]private bool isGrounded = false;

        public bool GroundCheck()
        {
            return isGrounded;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            foreach (var i in scriptData.OnCollisionPath)
            {
                LuaManager.Instance.CallFunction(i, i, this, collision.gameObject.GetComponent<Entity>());
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
            }
            foreach (var i in scriptData.OnCollisionPath)
            {
                LuaManager.Instance.CallFunction(i, i, this,collision.gameObject.GetComponent<Entity>());
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = false;
            }
        }
    }
}