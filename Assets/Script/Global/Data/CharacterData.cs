using System;
using System.Collections.Generic;

namespace Global.Data
{
    /// <summary>
    /// 对于需要移动的实体的数据
    /// </summary>
    [Serializable]
    public class CharacterEntityData
    {
        public float JumpHeight;
        public float speed;
        public float Gravity;
        public int signId = -1;
        public CharacterEntityData Copy()
        {
            return new CharacterEntityData
            {
                JumpHeight = this.JumpHeight,
                speed = this.speed,
                Gravity = this.Gravity,
                signId = -1
            };
        }
    }
    [Serializable]
    /// <summary>
    /// 所有实体通用的数据
    /// </summary>
    public class CommonEntityData
    {
        public int id;
        public int EffectId;
        public float ScaleX;
        public float ScaleY;
        public CharacterEntityData CharacterData;
        public CommonEntityData Copy()
        {
            var data = new CommonEntityData
            {
                EffectId = this.EffectId,
                ScaleX = this.ScaleX,
                ScaleY = this.ScaleY,
                CharacterData = this.CharacterData.Copy()
            };
            return data;
        }
    }
    [Serializable]
    /// <summary>
    /// 通用数据库
    /// </summary>
    public class CommonEntityDataCollection
    { 
        public List<CommonEntityData> CommonEntityList = new List<CommonEntityData>();
    }

}
