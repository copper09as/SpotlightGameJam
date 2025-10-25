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
        else
        {
            input.text = " ‰»Î¥ÌŒÛ£°";
        }


    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

 
  
}
