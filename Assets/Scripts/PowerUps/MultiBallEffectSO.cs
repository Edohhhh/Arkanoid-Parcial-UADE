using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/MultiBall")]
public class MultiBallEffectSO : PowerUpEffectSO
{
    public int extraBalls = 2;

    public override void ApplyEffect()
    {
        var mainBall = GameManager.Instance.GetMainBallLogic();
        if (mainBall == null) return;

        for (int i = 0; i < 2; i++)
        {
            // Offset de izquierda y derecha respecto a la pelota principal
            Vector3 offset = new Vector3((i == 0 ? -0.5f : 0.5f), 0, 0);
            Vector3 spawnPos = mainBall.transform.position + offset;

            // Usa el método correcto del GameManager para instanciar y configurar una pelota
            BallLogic newBall = GameManager.Instance.CreateNewBall(spawnPos);
        }
    }
}