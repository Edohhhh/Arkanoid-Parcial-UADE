using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickLogic
{
    public Transform transform;
    public int hitsToBreak = 3;
    public bool hasPowerUp = false;
    public GameObject powerUpPrefab;
    public float dropChance = 30f;
    public SetAtlasTile atlasTile;

    public void ConfigureAtlas()
    {
        if (atlasTile != null)
        {
            atlasTile.atlasColumn = hitsToBreak - 1;
            atlasTile.ApplyTile();
        }
    }

    public bool CheckCollision(Vector3 ballPos, float ballRadius)
    {
        if (!transform.TryGetComponent<Renderer>(out Renderer renderer))
            return false;

        Bounds bounds = renderer.bounds;
        bounds.Expand(ballRadius * 2f);
        return bounds.Contains(ballPos);
    }

    public bool TakeHit()
    {
        if (hitsToBreak <= 0)
            return false;

        hitsToBreak--;

        if (atlasTile != null && hitsToBreak > 0)
        {
            atlasTile.atlasColumn = Mathf.Clamp(hitsToBreak - 1, 0, 2);
            atlasTile.ApplyTile();
        }

        if (hitsToBreak <= 0)
        {
            transform.gameObject.SetActive(false);

            GameManager.Instance.AddPoints(1);
            GameManager.Instance.BrickDestroyed();

            if (hasPowerUp && powerUpPrefab != null)
            {
                float chance = Random.Range(0f, 100f);
                if (chance <= dropChance)
                {
                    GameManager.Instance.TrySpawnPowerUp(transform.position);
                }
            }

            return true;
        }

        return false;
    }
}
