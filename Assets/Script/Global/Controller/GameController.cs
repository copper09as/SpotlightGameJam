using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public static class GameController
{
    public static OriginGame Controller = new OriginGame();

    static GameController()
    {
        Controller.Main.LeftClick.Enable();
        Controller.Main.Move.Enable();
        Controller.Main.MousePos.Enable();
        Controller.Main.Space.Enable();
    }
    public static float MoveX()
    {
        return GameController.Controller.Main.Move.ReadValue<float>();
    }
    public static Vector3 GetWorldMousePos()
    {
        Vector2 screenPos = Controller.Main.MousePos.ReadValue<Vector2>();
        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        worldPos.z = 0f;
        return worldPos;
    }
}
