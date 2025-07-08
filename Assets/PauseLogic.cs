using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseLogic : ICustomUpdate
{
    private GameManager gm;

    public PauseLogic(GameManager manager)
    {
        gm = manager;
    }

    public void CustomUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gm.TogglePause();
        }
    }
}