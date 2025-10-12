using System.Collections;
using System.Collections.Generic;
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
    }
    public static float MoveX()
    {
        return GameController.Controller.Main.Move.ReadValue<float>();
    }
}
