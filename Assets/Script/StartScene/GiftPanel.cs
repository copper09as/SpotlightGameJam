using System;
using System.Collections;
using System.Collections.Generic;
using Global.Data.BattleConfig;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GiftPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField input;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button closeBtn;
    void Start()
    {
        closeBtn.onClick.AddListener(Close);
        confirmBtn.onClick.AddListener(Confirm);
    }
    private void OnEnable()
    {
        input.text = string.Empty;
    }

    private void Confirm()
    {
        if(input.text == "BlackField")
        {
            BattleConfig.Instance.UnLockLevel();
            Close();
        }
        else if(input.text == "Clear")
        {
            BattleConfig.Instance.ClearLevel();
            Close();
        }
        else if(input.text == "DeadMode")
        {
            BattleConfig.Instance.DeadMode = true;
            Close();
            BattleConfig.Instance.ClearLevel();
            NotificationManager.Instance.ShowNotification("开启死亡模式！死亡后会损失所有存档！", "开启死亡模式！");
        }

        else if (input.text == "CloseDeadMode")
        {
            BattleConfig.Instance.DeadMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("关闭死亡模式！", "关闭死亡模式！");
        }
        else if (input.text == "DragMode")
        {
            BattleConfig.Instance.DragMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("开启拖拽模式！所有物体都可以被拖拽！", "开启拖拽模式！");
        }
        else if (input.text == "CloseDragMode")
        {
            BattleConfig.Instance.DragMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("关闭拖拽模式！", "关闭拖拽模式！");
        }
        else if (input.text == "Alive!")
        {
            BattleConfig.Instance.EverythingMoveMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("所有物体活了起来！", "开启活力模式！");
        }
        else if (input.text == "CloseAlive")
        {
            BattleConfig.Instance.EverythingMoveMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("关闭活力模式！", "关闭活力模式！");
        }
        else if (input.text == "Poison")
        {
            BattleConfig.Instance.PoisionMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("开启毒素模式！所有的物体都有毒。。。", "开启毒素模式！");
        }
        else if (input.text == "ClosePoison")
        {
            BattleConfig.Instance.PoisionMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("关闭毒素模式!", "关闭毒素模式！");
        }
        else if (input.text == "DontDead")
        {
            BattleConfig.Instance.DontDeadMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("开启角色无敌模式!", "开启无敌模式！");
        }
        else if (input.text == "CloseDontDead")
        {
            BattleConfig.Instance.PoisionMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("关闭角色无敌模式!", "关闭角色无敌模式！");
        }
        else
        {
            input.text = "输入错误！";
        }


    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

 
  
}
