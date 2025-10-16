using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 控制UI激活或失活的流程
/// </summary>

using UnityEngine.InputSystem;

public class EntityUIManager : MonoBehaviour
{
    public static EntityUIManager Instance;
    [SerializeField] private GameObject menu1;
    [SerializeField] private GameObject menu2;

    private bool isMenu1Active = false;
    private bool isMenu2Active = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
        
    }
    private void Start()
    {

        GameController.Controller.Main.Esc.started += OnEscPressed;
        menu1.gameObject.SetActive(false);
        menu2.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Instance = null;
        GameController.Controller.Main.Esc.started -= OnEscPressed;
    }
    private void OnEscPressed(InputAction.CallbackContext ctx)
    {
        if (!isMenu1Active && !isMenu2Active)
        {
            ShowMenu1();
        }
        else if (isMenu1Active)
        {
            HideMenu1();
        }
        else if (isMenu2Active)
        {
            HideMenu2();
            ShowMenu1();
        }
    }
    //切换菜单2
    public void SwitchToMenu2()
    {
        HideMenu1();
        ShowMenu2();
    }
    
    private void ShowMenu1()
    {
        menu1.SetActive(true);
        isMenu1Active = true;
    }

    private void HideMenu1()
    {
        menu1.SetActive(false);
        isMenu1Active = false;
    }

    private void ShowMenu2()
    {
        menu2.SetActive(true);
        isMenu2Active = true;
    }

    private void HideMenu2()
    {
        menu2.SetActive(false);
        isMenu2Active = false;
    }
}
