using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "PowerUps/ExplodeBricks")]
public class ExplodeBricksEffectSO : PowerUpEffectSO
{
    public override void ApplyEffect()
    {
        Vector3 ballPos = GameManager.Instance.ballTransform.position;
        List<BrickInstance> bricks = GameManager.Instance.GetBrickList();

        foreach (var dir in new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        })
        {
            Vector3 target = ballPos + dir * 1.1f;
            foreach (var b in bricks)
            {
                if (!b.IsActive()) continue;
                if (Vector3.Distance(b.gameObject.transform.position, target) < 0.6f)
                {
                    b.TakeHit();
                    if (!b.IsActive())
                    {
                        GameManager.Instance.AddPoints(1);
                        GameManager.Instance.BrickDestroyed();
                    }
                }
            }
        }
    }
}