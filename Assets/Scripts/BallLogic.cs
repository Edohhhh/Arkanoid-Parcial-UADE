using System.Collections.Generic;
using UnityEngine;

public class BallLogic : ICustomUpdate
{
    public Transform transform;
    public Transform paddle;
    public bool isMainBall = false;
    public float speed = 8f;

    private Vector3 direction;
    private bool launched = false;

    private bool isExplosive = false;
    private float explosiveTimer = 0f;

    private const float ballRadius = 0.25f;

    public List<BrickInstance> bricks = new();

    public void SetBricks(List<BrickInstance> brickList)
    {
        this.bricks = brickList;
    }

    public void ActivateExplosiveMode(float duration)
    {
        isExplosive = true;
        explosiveTimer = duration;
    }

    public void CustomUpdate()
    {
        if (!launched)
        {
            transform.position = paddle.position + new Vector3(0, 0.5f, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float randomX = Random.Range(-1f, 1f);
                direction = new Vector3(randomX, 1, 0).normalized;
                direction.y = Mathf.Abs(direction.y);
                launched = true;
            }
        }
        else
        {
            float moveStep = speed * Time.deltaTime;
            int subSteps = 4;
            float stepSize = moveStep / subSteps;

            if (isExplosive)
            {
                explosiveTimer -= Time.deltaTime;
                if (explosiveTimer <= 0f)
                    isExplosive = false;
            }

            for (int i = 0; i < subSteps; i++)
            {
                transform.position += direction * stepSize;
                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

                if (CheckCollisionWithBricks()) break;
                if (CheckCollisionWithWalls()) break;
                if (CheckCollisionWithPaddle())
                {
                    BounceFromPaddle();
                    GameManager.Instance.AddPaddleHit();
                    break;
                }
            }

            if (transform.position.y < -5f)
            {
                if (isMainBall)
                {
                    GameManager.Instance.LoseLife();
                    launched = false;
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }
    }

    private bool CheckCollisionWithWalls()
    {
        ColliderAABB[] colliders = Object.FindObjectsByType<ColliderAABB>(FindObjectsSortMode.None);
        foreach (ColliderAABB col in colliders)
        {
            if (col.TryGetComponent<Renderer>(out Renderer renderer))
            {
                Bounds bounds = renderer.bounds;
                if (bounds.Contains(transform.position))
                {
                    if (col.type == ColliderAABB.ColliderType.Wall)
                        direction.x *= -1;
                    else if (col.type == ColliderAABB.ColliderType.Ceiling)
                        direction.y = -Mathf.Abs(direction.y);
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckCollisionWithBricks()
    {
        if (bricks == null || bricks.Count == 0)
        {
            Debug.LogWarning("⚠️ bricks está vacío en BallLogic");
            return false;
        }

        foreach (var brick in bricks)
        {
            if (!brick.IsActive()) continue;

            if (brick.CheckCollision(transform.position, ballRadius))
            {
                if (isExplosive)
                {
                    brick.DestroyCompletely();
                    GameManager.Instance.AddPoints(1);
                    GameManager.Instance.BrickDestroyed();
                    brick.gameObject.SetActive(false);
                    // No rebota si es explosivo
                }
                else
                {
                    bool wasDestroyed = brick.TakeHit();

                    if (!wasDestroyed)
                    {
                        direction.y *= -1;
                    }
                }

                return true;
            }
        }

        return false;
    }

    private bool CheckCollisionWithPaddle()
    {
        Vector3 ballPos = transform.position;
        Vector3 paddlePos = paddle.position;

        float paddleWidth = 3.5f;
        float paddleHeight = 0.5f;

        bool withinX = ballPos.x + ballRadius > paddlePos.x - paddleWidth / 2 &&
                       ballPos.x - ballRadius < paddlePos.x + paddleWidth / 2;
        bool withinY = ballPos.y - ballRadius < paddlePos.y + paddleHeight / 2 &&
                       ballPos.y + ballRadius > paddlePos.y - paddleHeight / 2;

        return withinX && withinY;
    }

    private void BounceFromPaddle()
    {
        float relativeX = transform.position.x - paddle.position.x;
        float paddleWidth = 3.5f;

        float maxAngle = 60f;
        float normalized = Mathf.Clamp(relativeX / (paddleWidth / 2f), -1f, 1f);
        float angle = normalized * maxAngle;
        float angleRad = angle * Mathf.Deg2Rad;

        direction = new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0).normalized;
        direction.y = Mathf.Abs(direction.y);
    }
}