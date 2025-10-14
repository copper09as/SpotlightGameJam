using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.Data.Entity
{
    [Serializable]
    public class EntityScriptData
    {
        public int id;
        public List<string> InitPath = new List<string>();//被初始化脚本路径
        public List<string> UpdatePath = new List<string>();//Update脚本路径
        public List<string> OnDestroyPath = new List<string>();//被摧毁脚本路径
        public List<string> OnMouseDownPath = new List<string>();//被点击脚本路径
        public List<string> OnCollisionPath = new List<string>();//在接触到时触发脚本路径
        public List<string> OnSpacePath = new List<string>();//空格按下时
        public List<string> OnDragPath = new List<string>();//拖拽时触发
        public List<string> DeadPath = new List<string>();//被其他脚本调用死亡效果
        public List<string> OnSpacePressPath = new List<string>();//被其他脚本调用死亡效果
    }
    [Serializable]
    public class EntityScriptDataCollection
    {
        public List<EntityScriptData> entityScriptList = new List<EntityScriptData>();
    }

}
