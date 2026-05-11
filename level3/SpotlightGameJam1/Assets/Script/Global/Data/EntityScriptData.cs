using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global.Data.Entity
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class EntityScriptData
    {
        public int id;
        public List<string> InitPath = new List<string>();          // 初始化脚本路径
        public List<string> UpdatePath = new List<string>();        // Update脚本路径
        public List<string> OnMouseDownPath = new List<string>();   // 点击脚本路径
        public List<string> OnCollisionPath = new List<string>();   // 碰撞脚本路径
        public List<string> OnTriggerPath = new List<string>();     // 触发器脚本路径
        public List<string> OnDragPath = new List<string>();        // 拖拽时触发
        public List<string> DeadPath = new List<string>();          // 死亡脚本路径
        public List<string> OnDisablePath = new List<string>();     // 消失时触发
        public List<string> OnEntityExitPath = new List<string>();  // 实体离开时触发

        public EntityScriptData Copy()
        {
            return new EntityScriptData
            {
                id = this.id,
                InitPath = new List<string>(this.InitPath),
                UpdatePath = new List<string>(this.UpdatePath),
                OnMouseDownPath = new List<string>(this.OnMouseDownPath),
                OnCollisionPath = new List<string>(this.OnCollisionPath),
                OnTriggerPath = new List<string>(this.OnTriggerPath),
                OnDragPath = new List<string>(this.OnDragPath),
                DeadPath = new List<string>(this.DeadPath),
                OnDisablePath = new List<string>(this.OnDisablePath),
                OnEntityExitPath = new List<string>(this.OnEntityExitPath)
            };
        }
    }

    [Serializable]
    public class EntityScriptDataCollection
    {
        public List<EntityScriptData> entityScriptList = new List<EntityScriptData>();
    }

}
