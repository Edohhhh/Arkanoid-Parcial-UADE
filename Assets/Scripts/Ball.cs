using UnityEngine;

public class Ball : MonoBehaviour, ICustomUpdate
{
    public float speed = 8f;
    public Transform paddle;
    private bool launched = false;
    private Vector3 direction;

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

            
            if (Mathf.Abs(transform.position.x) > 8f) direction.x *= -1;
            if (transform.position.y > 5f) direction.y *= -1;

          
            if (CheckCollisionWithPaddle())
            {
                direction.y = Mathf.Abs(direction.y); 
             
            }

            
            if (transform.position.y < -5f)
            {
                launched = false;
            }
        }

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

