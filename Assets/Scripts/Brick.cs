using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Brick : MonoBehaviour
{
    public int hitsToBreak = 3;

    public float width = 1f;
    public float height = 0.5f;

    public bool hasPowerUp = false;
    public GameObject powerUpPrefab;

    public float dropChance = 30f; 

    public ObjectPool powerUpPool;

    public SetAtlasTile atlasTile;

    private void Start()
    {
        if (atlasTile != null)
        {
            atlasTile.atlasColumn = hitsToBreak - 1;
            atlasTile.ApplyTile();
        }
    }

    public bool CheckCollision(Vector3 ballPos, float ballRadius)
    {
        if (!TryGetComponent<Renderer>(out Renderer renderer))
            return false;

        Bounds bounds = renderer.bounds;

        
        bounds.Expand(ballRadius * 2f);

        return bounds.Contains(ballPos);
    }

    public void TakeHit()
    {
        hitsToBreak--;

        if (atlasTile != null)
        {
            atlasTile.atlasColumn = hitsToBreak - 1;
            atlasTile.ApplyTile();
        }

        if (hitsToBreak <= 0)
        {
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddPoints(1);
            if (hasPowerUp && powerUpPrefab != null)
            {
                float chance = Random.Range(0f, 100f);
                if (chance <= dropChance)
                {
                    if (powerUpPool != null)
                    {
                        GameObject p = powerUpPool.GetFromPool(transform.position);
                        p.transform.rotation = Quaternion.Euler(0f, 0f, 90f); 
                        PowerUp pScript = p.GetComponent<PowerUp>();
                        pScript.pool = powerUpPool;
                    }
                    else
                    {
                        Instantiate(powerUpPrefab, transform.position, Quaternion.Euler(90f, 0f, 0f)); 
                    }
                }
            }

            Destroy(gameObject);
        }
    }
}
