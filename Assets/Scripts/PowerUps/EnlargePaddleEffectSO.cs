using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PowerUps/EnlargePaddle")]
public class EnlargePaddleEffectSO : PowerUpEffectSO
{
    public float duration = 5f;
    public float scaleMultiplier = 1.5f;

    public override void ApplyEffect()
    {
        GameManager.Instance.StartCoroutine(Enlarge());
    }

    private IEnumerator Enlarge()
    {
        Transform paddle = GameManager.Instance.paddleTransform;
        Vector3 original = paddle.localScale;
        paddle.localScale = new Vector3(original.x * scaleMultiplier, original.y, original.z);
        yield return new WaitForSeconds(duration);
        paddle.localScale = original;
    }
}
