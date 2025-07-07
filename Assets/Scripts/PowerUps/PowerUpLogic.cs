using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpLogic : ICustomUpdate
{
    public Transform transform;
    public float fallSpeed = 2f;
    private ObjectPool originPool;

    public PowerUpEffectSO effect;

    public void Initialize(Transform tf, ObjectPool pool, PowerUpEffectSO assignedEffect)
    {
        transform = tf;
        originPool = pool;
        effect = assignedEffect;
        CustomUpdateManager.Register(this);
    }

    public void CustomUpdate()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < -6f)
        {
            Despawn();
        }

        if (GameManager.Instance.paddleTransform != null)
        {
            if (Vector3.Distance(transform.position, GameManager.Instance.paddleTransform.position) < 1f)
            {
                effect?.ApplyEffect();
                Despawn();
            }
        }
    }

    private void Despawn()
    {
        CustomUpdateManager.Unregister(this);
        originPool.ReturnToPool(transform.gameObject);
    }
}