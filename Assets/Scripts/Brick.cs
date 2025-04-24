using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Brick : MonoBehaviour
{
    public int hitsToBreak = 3;

    public Material mat3;
    public Material mat2;
    public Material mat1;

    public float width = 1f;
    public float height = 0.5f;

    public bool hasPowerUp = false;
    public GameObject powerUpPrefab;

    public float dropChance = 30f; // porcentaje (0 a 100)

    private Renderer rend;

    public ObjectPool powerUpPool;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        //UpdateMaterial();
    }

    public bool CheckCollision(Vector3 ballPos, float ballRadius)
    {
        Vector3 pos = transform.position;

        bool withinX = ballPos.x + ballRadius > pos.x - width / 2 &&
                       ballPos.x - ballRadius < pos.x + width / 2;

        bool withinY = ballPos.y + ballRadius > pos.y - height / 2 &&
                       ballPos.y + ballRadius < pos.y + height / 2;

        return withinX && withinY;
    }

    public void TakeHit()
    {
        hitsToBreak--;

        //UpdateMaterial();

        if (hitsToBreak <= 0)
        {
            // Solo intenta tirar powerup si tiene asignado un prefab
            if (hasPowerUp && powerUpPrefab != null)
            {
                float chance = Random.Range(0f, 100f);
                if (chance <= dropChance)
                {
                    if (powerUpPool != null)
                    {
                        GameObject p = powerUpPool.GetFromPool(transform.position);
                        PowerUp pScript = p.GetComponent<PowerUp>();
                        pScript.pool = powerUpPool;
                    }
                    else
                    {
                        Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
                    }
                }
            }

            Destroy(gameObject);
        }
    }

    //private void UpdateMaterial()
    //{
    //    if (hitsToBreak == 3 && mat3 != null)
    //        rend.material = mat3;
    //    else if (hitsToBreak == 2 && mat2 != null)
    //        rend.material = mat2;
    //    else if (hitsToBreak == 1 && mat1 != null)
    //        rend.material = mat1;
    //}
}
