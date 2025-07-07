using UnityEngine;

[CreateAssetMenu(menuName = "PowerUps/ExplodeBricks")]
public class ExplodeBricksEffectSO : PowerUpEffectSO
{
    [Header("Configuraci�n")]
    public float explosiveDuration = 5f;

    public override void ApplyEffect()
    {
        BallLogic ball = GameManager.Instance.GetMainBallLogic();
        if (ball != null)
        {
            ball.ActivateExplosiveMode(explosiveDuration);
        }
    }
}
