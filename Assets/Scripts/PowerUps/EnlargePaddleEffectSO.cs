using UnityEngine;
using System.Collections;

[CreateAssetMenu(menuName = "PowerUps/Enlarge Paddle")]
public class EnlargePaddleEffectSO : PowerUpEffectSO
{
    public float scaleMultiplier = 1.5f;
    public float duration = 5f;

    public override void ApplyEffect()
    {
        Transform paddle = GameManager.Instance.paddleTransform;

        if (paddle == null)
        {
            Debug.LogWarning("🚫 Paddle transform no encontrado.");
            return;
        }

        GameManager.Instance.StartCoroutine(EnlargeTemporarily(paddle));
    }

    private IEnumerator EnlargeTemporarily(Transform paddle)
    {
        Vector3 originalScale = paddle.localScale;
        Vector3 enlargedScale = new Vector3(originalScale.x * scaleMultiplier, originalScale.y, originalScale.z);

        paddle.localScale = enlargedScale;

        yield return new WaitForSeconds(duration);

        // Volver a su tamaño original
        if (paddle != null)
            paddle.localScale = originalScale;
    }
}