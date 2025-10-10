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
    }
}
