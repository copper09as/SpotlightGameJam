using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button btn;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private GameObject frame;
    public void Init(bool isLock,string sceneName,UnityAction action)
    {
        btn.interactable = isLock;
        nameTxt.text = sceneName;
        btn.onClick.AddListener(action);
        frame.SetActive(false);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("Assets/Audio/Sfx/MouseFlow (2).wav");
        if(btn.interactable && frame!=null)
        {
            frame.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (btn.interactable && frame != null)
        {
            frame.SetActive(false);
        }
    }
}
