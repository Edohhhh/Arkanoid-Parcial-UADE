using UnityEngine;

public class PowerUp : MonoBehaviour, ICustomUpdate
{
    public float speed = 2f;

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
        
        transform.position += Vector3.down * speed * Time.deltaTime;

        
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }

       
        GameObject paddle = GameObject.Find("Paddle");
        if (paddle != null)
        {
            Vector3 paddlePos = paddle.transform.position;
            float paddleWidth = 2f;
            float paddleHeight = 0.5f;
            float radius = 0.3f;

            bool withinX = transform.position.x + radius > paddlePos.x - paddleWidth / 2 &&
                           transform.position.x - radius < paddlePos.x + paddleWidth / 2;
            bool withinY = transform.position.y - radius < paddlePos.y + paddleHeight / 2 &&
                           transform.position.y + radius > paddlePos.y - paddleHeight / 2;

            if (withinX && withinY)
            {
                ActivateMultiball(paddlePos);
                Destroy(gameObject);
            }
        }
    }

    private void ActivateMultiball(Vector3 origin)
    {
        GameObject existingBall = GameObject.Find("Ball");
        if (existingBall != null)
        {
            for (int i = 0; i < 2; i++)
            {
                Vector3 offset = new Vector3(i == 0 ? -0.3f : 0.3f, 0.5f, 0);
                GameObject newBall = Instantiate(existingBall, origin + offset, Quaternion.identity);
                Ball ballScript = newBall.GetComponent<Ball>();
                ballScript.enabled = true;
                ballScript.ForceLaunch();
            }
        }
    }
}
