using System.Collections;
using System.Collections.Generic;
using Game.Battle.Entity;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class UIFactory
{
    public static void CreatUIFromPrefab(Transform uiParent,int id,string path)
    {
        GameObject ui =  ResManager.LoadDataByAsset<GameObject>(path);
        var obj = UnityEngine.Object.Instantiate(ui,uiParent);
        var entity = obj.GetComponent<Entity>();
        entity.Init(id);
    }
}
