using System.Collections;
using System.Collections.Generic;
using Global.Data;
using Global.Data.Entity;
using Global.ObjectCreate;
using UnityEngine;
namespace Game.Battle.Entity
{
    public class Entity : MonoBehaviour,IObjectByCreate
    {
        private EntityScriptData scriptData = new();
        private Animator animator;
        public CharacterEntityData CharacterData;
        public Rigidbody2D rb;
        string IObjectByCreate.Name 
        { get => "Entity";
            set => value = "Entity"; }
        private void Awake()
        {
            CharacterData = GameConfig.Instance.CharacterDT.ToEntityData();
            foreach (var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
            this.scriptData.UpdatePath.Add("CharacterMove");
        }
        #region 脚本方法
        public void Init(EntityScriptData scriptData)
        {
            this.scriptData = scriptData;
            foreach(var i in scriptData.InitPath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
            }
            this.scriptData.UpdatePath.Add("CharacterMove");
            
        }
        void Update()
        {
            foreach (var i in scriptData.UpdatePath)
            {
                LuaManager.Instance.CallFunction(i, i, this);
                Debug.Log(i);
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
    }
}