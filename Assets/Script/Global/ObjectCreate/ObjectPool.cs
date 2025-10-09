using System;
using System.Collections.Generic;
using UnityEngine;
namespace Global.ObjectCreate
{
    public class ObjectPool:MonoBehaviour
    {
        private Dictionary<string, Stack<GameObject>> poolDic = new();
        [SerializeField] private int maxCapacity;
        /// <summary>
        /// 从池中获取对象
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="name">唯一标识</param>
        /// <param name="parent">父节点</param>
        /// <returns></returns>
        public GameObject Get(string path,string name,Transform parent)
        {
            GameObject ob = null;
            if (!poolDic.TryGetValue(name,out Stack<GameObject> stack))
            {
                stack = new();
                poolDic[name] = stack;
            }
            if (stack.Count > 0)
            {
              ob = stack.Pop();
            }
            else
            {
                GameObject data = ResManager.LoadDataByAsset<GameObject>(path);
                ob = Instantiate(data, parent);
            }
            ob.SetActive(true);
            return ob;
        }
        /// <summary>
        /// 释放对象
        /// </summary>
        /// <param name="ob">唯一标识</param>
        /// <exception cref="Exception"></exception>
        public void Release(GameObject ob)
        {
            if(ob == null)
            {
                throw new Exception("目标物体为空");
            }
            IObjectByCreate objectByCreate = ob.GetComponent<IObjectByCreate>();
            if (objectByCreate == null)
            {
                Destroy(ob);
                Debug.LogWarning("目标物体不拥有创建接口");
                return;
            }
            Stack<GameObject> stack = poolDic[objectByCreate.Name];
            if (stack.Count >= maxCapacity)
            {
                Destroy(ob);
            }
            else
            {
                ob.SetActive(false);
                stack.Push(ob);
            }
        }
        /// <summary>
        /// 清空指定池子
        /// </summary>
        /// <param name="name">唯一标识</param>
        public void Clear(string name)
        {
            if (poolDic.TryGetValue(name, out Stack<GameObject> stack))
            {
                while (stack.Count > 0)
                {
                    GameObject obj = stack.Pop();
                    Destroy(obj);
                }
            }
        }
        /// <summary>
        /// 清空对象池字典
        /// </summary>
        public void ClearAll()
        {
            foreach (var kv in poolDic)
            {
                while (kv.Value.Count > 0)
                {
                    GameObject obj = kv.Value.Pop();
                    Destroy(obj);
                }
            }
            poolDic.Clear();
        }
    }
}