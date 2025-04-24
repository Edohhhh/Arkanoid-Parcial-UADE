using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class Ball : MonoBehaviour, ICustomUpdate
{
    public float speed = 8f;
    public Transform paddle;
    private bool launched = false;
    private Vector3 direction;

    public bool isMainBall = false;

    private void OnEnable()
    {
        CustomUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CustomUpdateManager.Unregister(this);
    }

    public void CustomUpdate()
    {
        if (!launched)
        {
            // Mantener bola pegada a la paleta
            transform.position = paddle.position + new Vector3(0, 0.5f, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                float randomX = Random.Range(-1f, 1f);
                direction = new Vector3(randomX, 1, 0).normalized;
                launched = true;
            }
        }
        else
        {
            
            transform.position += direction * speed * Time.deltaTime;

            
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

            
            ColliderAABB[] colliders = Object.FindObjectsByType<ColliderAABB>(FindObjectsSortMode.None);
            foreach (ColliderAABB col in colliders)
            {
                Vector3 colPos = col.transform.position;
                Vector3 colScale = col.transform.localScale;

                if (transform.position.x > colPos.x - colScale.x / 2f &&
                    transform.position.x < colPos.x + colScale.x / 2f &&
                    transform.position.y > colPos.y - colScale.y / 2f &&
                    transform.position.y < colPos.y + colScale.y / 2f)
                {
                    if (col.type == ColliderAABB.ColliderType.Wall)
                    {
                        direction.x *= -1;
                    }
                    else if (col.type == ColliderAABB.ColliderType.Ceiling)
                    {
                        direction.y = -Mathf.Abs(direction.y);
                    }
                }
            }

            // Calculo de fisicas rebote pelota en paleta
            if (CheckCollisionWithPaddle())
            {
                float relativeX = transform.position.x - paddle.position.x;
                float paddleWidth = 2f;
                float maxAngle = 60f;

                float normalized = Mathf.Clamp(relativeX / (paddleWidth / 2f), -1f, 1f);
                float angle = normalized * maxAngle;

                float angleRad = angle * Mathf.Deg2Rad;

                direction = new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad), 0).normalized;
            }

            // Caída por debajo del límite
            if (transform.position.y < -5f)
            {
                if (isMainBall)
                {
                    launched = false;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }

        // Colisiones con bricks
        Brick[] bricks = Object.FindObjectsByType<Brick>(FindObjectsSortMode.None);
        foreach (Brick brick in bricks)
        {
            if (brick.CheckCollision(transform.position, 0.25f))
            {
                direction.y *= -1;
                brick.TakeHit();
                break;
            }
        }
    }

    public void ForceLaunch()
    {
        if (!launched)
        {
            float randomX = Random.Range(-1f, 1f);
            direction = new Vector3(randomX, 1, 0).normalized;
            launched = true;
        }
    }

    private bool CheckCollisionWithPaddle()
    {
        Vector3 ballPos = transform.position;
        Vector3 paddlePos = paddle.position;

        float paddleWidth = 2f;
        float paddleHeight = 0.5f;
        float ballRadius = 0.25f;

        bool withinX = ballPos.x + ballRadius > paddlePos.x - paddleWidth / 2 &&
                       ballPos.x - ballRadius < paddlePos.x + paddleWidth / 2;
        bool withinY = ballPos.y - ballRadius < paddlePos.y + paddleHeight / 2 &&
                       ballPos.y + ballRadius > paddlePos.y - paddleHeight / 2;

        return withinX && withinY;
    }
}
