using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
namespace Global.ObjectCreate
{
    public class ObjectPool:MonoBehaviour
    {
        private Dictionary<string, Stack<GameObject>> poolDic = new();
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
        public void Destroy(GameObject ob)
        {
            if(ob == null)
            {
                throw new Exception("Ŀ������Ϊ��");
            }
            IObjectByCreate objectByCreate = ob.GetComponent<IObjectByCreate>();
            Stack<GameObject> stack = poolDic[objectByCreate.Name];
            if (objectByCreate == null)
            {
                throw new Exception("Ŀ�����岻ӵ�д����ӿ�");
            }
            stack.Push(ob);
            ob.SetActive(false);
        }
    }
}