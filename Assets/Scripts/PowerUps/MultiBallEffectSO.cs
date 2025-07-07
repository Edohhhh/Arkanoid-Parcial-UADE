using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/MultiBall")]
public class MultiBallEffectSO : PowerUpEffectSO
{
    [Header("Configuración")]
    public int ballCount = 2;
    public float offsetX = 0.5f;

    public override void ApplyEffect()
    {
        Transform paddle = GameManager.Instance.paddleTransform;
        ObjectPool ballPool = GameManager.Instance.ballPool;

        List<BrickInstance> bricks = GameManager.Instance.GetBrickList(); // Esto debe existir en GameManager

        for (int i = 0; i < ballCount; i++)
        {
            float xOffset = ((i - (ballCount - 1) / 2f) * offsetX);
            Vector3 spawnPos = paddle.position + new Vector3(xOffset, 0.5f, 0);

            GameObject newBall = ballPool.GetFromPool(spawnPos);

            BallLogic logic = new BallLogic
            {
                transform = newBall.transform,
                paddle = paddle,
                isMainBall = false,
                bricks = bricks
            };

            CustomUpdateManager.Register(logic);
        }
    }
}