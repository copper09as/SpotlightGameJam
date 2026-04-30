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
        if (input.text == "BlackField")
        {
            BattleConfig.Instance.UnLockLevel();
            Close();
            NotificationManager.Instance.ShowNotification("�ѽ����ؿ���7600��", "�������йؿ���");
        }
        else if (input.text == "Clear")
        {
            BattleConfig.Instance.ClearLevel();
            Close();
            NotificationManager.Instance.ShowNotification("����մ浵��", "�Ѿ���մ浵��");
        }
        else if (input.text == "DeadMode")
        {
            BattleConfig.Instance.DeadMode = true;
            Close();
            BattleConfig.Instance.ClearLevel();
            NotificationManager.Instance.ShowNotification("��������ģʽ�����������ʧ���д浵��", "��������ģʽ��");
        }

        else if (input.text == "CloseDeadMode")
        {
            BattleConfig.Instance.DeadMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("�ر�����ģʽ��", "�ر�����ģʽ��");
        }
        else if (input.text == "DragMode")
        {
            BattleConfig.Instance.DragMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("������קģʽ���������嶼���Ա���ק��", "������קģʽ��");
        }
        else if (input.text == "CloseDragMode")
        {
            BattleConfig.Instance.DragMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("�ر���קģʽ��", "�ر���קģʽ��");
        }
        else if (input.text == "Alive!")
        {
            BattleConfig.Instance.EverythingMoveMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("�����������������", "��������ģʽ��");
        }
        else if (input.text == "CloseAlive")
        {
            BattleConfig.Instance.EverythingMoveMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("�رջ���ģʽ��", "�رջ���ģʽ��");
        }
        else if (input.text == "Poison")
        {
            BattleConfig.Instance.PoisionMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("��������ģʽ�����е����嶼�ж�������", "��������ģʽ��");
        }
        else if (input.text == "ClosePoison")
        {
            BattleConfig.Instance.PoisionMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("�رն���ģʽ!", "�رն���ģʽ��");
        }
        else if (input.text == "DontDead")
        {
            BattleConfig.Instance.DontDeadMode = true;
            Close();
            NotificationManager.Instance.ShowNotification("������ɫ�޵�ģʽ!", "�����޵�ģʽ��");
        }
        else if (input.text == "CloseDontDead")
        {
            BattleConfig.Instance.PoisionMode = false;
            Close();
            NotificationManager.Instance.ShowNotification("�رս�ɫ�޵�ģʽ!", "�رս�ɫ�޵�ģʽ��");
        }
        else if (input.text == "SpecialSfx")
        {
            BattleConfig.Instance.SpecialSfx = 1;
            Close();
            NotificationManager.Instance.ShowNotification("����������Ч!", "����������Ч��");
        }
        else if (input.text == "CloseSpecialSfx")
        {
            BattleConfig.Instance.SpecialSfx = 0;
            Close();
            NotificationManager.Instance.ShowNotification("�ر�������Ч!", "�ر�������Ч��");
        }
        else if(input.text == "Edit")
        {
            SceneChangeManager.Instance.LoadScene("LevelEditor");
        }
        else
        {
            input.text = "�������";
        }


    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

 
  
}
