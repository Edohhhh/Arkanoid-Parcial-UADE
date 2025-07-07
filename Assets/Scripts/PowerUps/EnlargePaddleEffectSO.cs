using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PowerUps/EnlargePaddle")]
public class EnlargePaddleEffectSO : PowerUpEffectSO
{
    public float scaleMultiplier = 2f;
    public float duration = 5f;
    public float growSpeed = 2f; // cuanto más alto, más rápido se agranda

    public override void ApplyEffect()
    {
        var paddle = GameManager.Instance.paddleTransform;
        if (paddle == null) return;

        GameManager.Instance.StartCoroutine(AnimateScale(paddle));
    }

    private IEnumerator AnimateScale(Transform paddle)
    {
        Vector3 originalScale = paddle.localScale;
        Vector3 targetScale = new Vector3(originalScale.x * scaleMultiplier, originalScale.y, originalScale.z);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * growSpeed;
            paddle.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        paddle.localScale = targetScale;

        yield return new WaitForSeconds(duration);

        // Restaurar suavemente
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * growSpeed;
            paddle.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        paddle.localScale = originalScale;
    }
}