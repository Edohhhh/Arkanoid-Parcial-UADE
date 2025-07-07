using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Referencias de escena")]
    public Transform paddleTransform;
    public Transform ballTransform;
    public Transform bricksStartPoint;
    public ObjectPool brickPool;
    public ObjectPool powerUpPool;
    public GameObject powerUpPrefab;

    [Header("Configuraci�n")]
    public int rows = 5;
    public int columns = 10;
    public float spacing = 1.1f;

    public int bricksLeft = 0;
    private PaddleLogic paddle;
    private BallLogic ball;
    private List<BrickInstance> bricks = new();

    [Header("PowerUp Drop Config")]
    public PowerUpDropTable dropTable;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        paddle = new PaddleLogic();
        paddle.transform = paddleTransform;
        CustomUpdateManager.Register(paddle);

        ball = new BallLogic();
        ball.transform = ballTransform;
        ball.paddle = paddleTransform;
        ball.isMainBall = true;
        CustomUpdateManager.Register(ball);

        SpawnBricks();
        ball.SetBricks(bricks);
    }

    public void SpawnBricks()
    {
        bricks.Clear();
        bricksLeft = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = bricksStartPoint.position + new Vector3(col * spacing, -row * spacing, 0);
                GameObject brickGO = brickPool.GetFromPool(pos);
                brickGO.transform.position = pos;
                brickGO.SetActive(true);

                var atlas = brickGO.GetComponent<SetAtlasTile>();
                int hits = Random.Range(1, 4);
                bool hasPower = Random.Range(0f, 1f) < 0.2f;

                BrickInstance brick = new BrickInstance(
                    brickGO,
                    hits,
                    hasPower,
                    powerUpPool,
                    powerUpPrefab,
                    atlas
                );

                bricks.Add(brick);
                bricksLeft++;
            }
        }
    }

    public void TrySpawnPowerUp(Vector3 position)
    {
        PowerUpEffectSO effect = dropTable.GetRandomPowerUp();
        if (effect == null) return;

        GameObject p = powerUpPool.GetFromPool(position);
        var logic = new PowerUpLogic();
        logic.Initialize(p.transform, powerUpPool, effect);
    }

    public void BrickDestroyed()
    {
        bricksLeft--;
    }

    public void AddPoints(int amount)
    {
        // Implementar puntaje si es necesario
    }

    public void AddPaddleHit()
    {
        // Efectos o sonido
    }

    public void LoseLife()
    {
        // Vidas - reinicio - Game Over
    }

    public List<BrickInstance> GetBrickList()
    {
        return bricks;
    }
}
