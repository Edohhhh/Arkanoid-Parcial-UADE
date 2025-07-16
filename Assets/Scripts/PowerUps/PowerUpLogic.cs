using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpLogic : ICustomUpdate
{
    public Transform transform;
    public float fallSpeed = 2f;
    private IPool originPool;

    public PowerUpEffectSO effect;
    private Transform paddle;

    public void Initialize(Transform tf, IPool pool, PowerUpEffectSO assignedEffect)
    {
        transform = tf;
        originPool = pool;
        effect = assignedEffect;
        CustomUpdateManager.Register(this);
    }

    public void SetPaddle(Transform paddleTransform)
    {
        this.paddle = paddleTransform;
    }

    public void CustomUpdate()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < -6f)
        {
            Despawn();
            return;
        }

        if (paddle != null && Vector3.Distance(transform.position, paddle.position) < 1f)
        {
            effect?.ApplyEffect();
            Despawn();
        }
    }

    private void Despawn()
    {
        CustomUpdateManager.Unregister(this);
        originPool.ReturnToPool(transform.gameObject);
    }
}
