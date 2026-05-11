using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPanel : MonoBehaviour
{
    [SerializeField]
    private Button CloseBtn;
    // Start is called before the first frame update
    void Start()
    {
        CloseBtn.onClick.AddListener(Close);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
