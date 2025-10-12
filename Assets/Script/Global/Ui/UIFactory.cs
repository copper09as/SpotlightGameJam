using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class UIFactory
{
    public static void CreatUIFromPrefab(Transform uiParent,int id,string path)
    {
        GameObject ui =  ResManager.LoadDataByAsset<GameObject>(path);

        ui.transform.parent = uiParent;
    }
}
