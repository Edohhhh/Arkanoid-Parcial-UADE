using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/MultiBall")]
public class MultiBallEffectSO : PowerUpEffectSO
{
    public override void ApplyEffect()
    {
        Transform paddle = GameManager.Instance.paddleTransform;
        ObjectPool ballPool = GameObject.Find("BallPool").GetComponent<ObjectPool>();

        for (int i = 0; i < 2; i++)
        {
            GameObject newBall = ballPool.GetFromPool(paddle.position + new Vector3(i == 0 ? -0.5f : 0.5f, 0.5f, 0));
            BallLogic logic = new BallLogic
            {
                transform = newBall.transform,
                paddle = paddle,
                isMainBall = false
            };
            CustomUpdateManager.Register(logic);
        }
    }
}