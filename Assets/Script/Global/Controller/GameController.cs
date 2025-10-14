using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameController
{
    public static OriginGame Controller = new OriginGame();

    // 跳跃时间控制变量
    public static bool isSpacePressed = false;
    private static float spacePressStartTime = 0f;

    static GameController()
    {
        Controller.Main.LeftClick.Enable();
        Controller.Main.Move.Enable();
        Controller.Main.MousePos.Enable();
        Controller.Main.Space.Enable();
        Controller.Main.Esc.Enable();
        // 注册输入事件
        Controller.Main.Space.performed += ctx => OnSpacePressed();
        Controller.Main.Space.canceled += ctx => OnSpaceReleased();
    }
    public static float MoveX()
    {
        return Controller.Main.Move.ReadValue<float>();
    }
    public static Vector3 GetWorldMousePos()
    {
        Vector2 screenPos = Controller.Main.MousePos.ReadValue<Vector2>();
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
    public static Vector3 GetScreenPos()=> Controller.Main.MousePos.ReadValue<Vector2>();

    //按下空格
    private static void OnSpacePressed()
    {
        isSpacePressed = true;
        spacePressStartTime = Time.time;
    }

    //松开空格
    private static void OnSpaceReleased()
    {
        isSpacePressed = false;
    }

    //获取当前按住空格的持续时间
    public static float GetJumpChargeTime()
    {
        if (isSpacePressed)
        {
            // 当前按压时间
            return Time.time - spacePressStartTime;
        }
        else
        {
            // 如果空格未按下，返回0或者上一次按压时间
            return 0f;
        }
    }

}
