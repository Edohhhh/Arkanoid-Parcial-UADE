using UnityEngine;

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

    private Renderer rend;

    public ObjectPool powerUpPool;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateMaterial();
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

        if (hitsToBreak <= 0)
        {
            if (hasPowerUp && powerUpPrefab != null)
            {
                if (powerUpPool != null)
                {
                    GameObject p = powerUpPool.GetFromPool(transform.position);
                    PowerUp pScript = p.GetComponent<PowerUp>();
                    pScript.pool = powerUpPool;
                }
                else
                {
                    GameObject p = Instantiate(powerUpPrefab, transform.position, Quaternion.identity);
                }
            }

            Destroy(gameObject);
        }


    }

    private void UpdateMaterial()
    {
        if (hitsToBreak == 3 && mat3 != null)
            rend.material = mat3;
        else if (hitsToBreak == 2 && mat2 != null)
            rend.material = mat2;
        else if (hitsToBreak == 1 && mat1 != null)
            rend.material = mat1;
    }
}

