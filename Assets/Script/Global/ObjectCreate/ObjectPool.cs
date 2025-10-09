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
        /// �ӳ��л�ȡ����
        /// </summary>
        /// <param name="path">·��</param>
        /// <param name="name">Ψһ��ʶ</param>
        /// <param name="parent">���ڵ�</param>
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
        /// �ͷŶ���
        /// </summary>
        /// <param name="ob">Ψһ��ʶ</param>
        /// <exception cref="Exception"></exception>
        public void Release(GameObject ob)
        {
            if(ob == null)
            {
                throw new Exception("Ŀ������Ϊ��");
            }
            IObjectByCreate objectByCreate = ob.GetComponent<IObjectByCreate>();
            if (objectByCreate == null)
            {
                Destroy(ob);
                Debug.LogWarning("Ŀ�����岻ӵ�д����ӿ�");
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
        /// ���ָ������
        /// </summary>
        /// <param name="name">Ψһ��ʶ</param>
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
        /// ��ն�����ֵ�
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