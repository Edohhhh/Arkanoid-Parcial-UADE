using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogicBootstrapper
{
    public static void Bootstrap(GameObject paddleGO, GameObject ballGO, GameObject[] powerUps = null)
    {
        // Paddle
        var paddleLogic = new PaddleLogic
        {
            transform = paddleGO.transform,
            speed = 10f
        };
        CustomUpdateManager.Register(paddleLogic);

        // Ball
        var ballLogic = new BallLogic
        {
            transform = ballGO.transform,
            paddle = paddleGO.transform,
            isMainBall = true
        };
        CustomUpdateManager.Register(ballLogic);

        // PowerUps
        if (powerUps != null)
        {
            foreach (var pu in powerUps)
            {
                var powerUpLogic = new PowerUpLogic
                {
                    transform = pu.transform,
                };
                CustomUpdateManager.Register(powerUpLogic);
            }
        }
    }
}