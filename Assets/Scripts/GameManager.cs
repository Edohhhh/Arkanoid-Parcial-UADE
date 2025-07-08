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
    public ObjectPool ballPool;


    [Header("Configuración de grilla")]
    public int rows = 5;
    public int columns = 10;
    public float spacing = 1.1f;

    [Header("Config. de dificultad por fila")]
    public List<BrickRowConfig> rowConfigs = new();

    [Header("PowerUp Drop Config")]
    public PowerUpDropTable dropTable;

    public int bricksLeft = 0;
    private PaddleLogic paddle;
    private BallLogic ball;
    private List<BrickInstance> bricks = new();
    public NumericDisplay scoreDisplay;

    [Header("Puntajes")]
    private int score = 0;
    [SerializeField] private int lives = 3;
    [SerializeField] private NumericDisplay livesDisplay;
    [SerializeField] private NumericDisplay bricksDisplay;
    private int paddleHits = 0;
    [SerializeField] private NumericDisplay paddleHitsDisplay;
    [Header("Pausa")]
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    private PauseLogic pauseLogic;

    [Header("Parallax")]
    [SerializeField] private List<Transform> parallaxLayers;
    [SerializeField] private List<float> parallaxSpeeds;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CustomUpdateManager.Register(new DelayedMusicStart());
    }

    private void Start()
    {
        InitializeGame();
        scoreDisplay?.UpdateDisplay(score);
        livesDisplay?.UpdateDisplay(lives);
        bricksDisplay?.UpdateDisplay(bricksLeft);
        paddleHitsDisplay?.UpdateDisplay(paddleHits);
        pauseLogic = new PauseLogic(this);
        CustomUpdateManager.Register(pauseLogic);
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
        RegisterParallaxLayers();
    }

    public void SpawnBricks()
    {
        bricks.Clear();
        bricksLeft = 0;

        for (int row = 0; row < rows; row++)
        {
            int hits = 1;
            Color color = Color.white;

            if (row < rowConfigs.Count)
            {
                hits = rowConfigs[row].hits;
                color = rowConfigs[row].color;
            }

            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = bricksStartPoint.position + new Vector3(col * spacing, -row * spacing, 0);
                GameObject brickGO = brickPool.GetFromPool(pos);
                brickGO.transform.position = pos;
                brickGO.SetActive(true);

                var atlas = brickGO.GetComponent<SetAtlasTile>();

                bool hasPower = Random.Range(0f, 1f) < 0.2f;

                BrickInstance brick = new BrickInstance(
                    brickGO,
                    hits,
                    hasPower,
                    powerUpPool,
                    powerUpPrefab,
                    atlas
                );

                if (atlas != null)
                {
                    atlas.atlasColumn = hits - 1;
                    atlas.ApplyTile();
                }

                bricks.Add(brick);
                bricksLeft++;
            }
        }
    }
    private class DelayedMusicStart : ICustomUpdate
    {
        private bool started = false;

        public void CustomUpdate()
        {
            if (!started && UIAudioManager.Instance != null)
            {
                UIAudioManager.Instance.PlayBackgroundMusic();
                started = true;
                CustomUpdateManager.Unregister(this);
            }
        }
    }

    void RegisterParallaxLayers()
    {
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            if (parallaxLayers[i] != null)
            {
                var logic = new ParallaxLayerLogic(parallaxLayers[i], paddleTransform, parallaxSpeeds[i]);
                CustomUpdateManager.Register(logic);
            }
            else
            {
                Debug.LogWarning($"🌀 No se asignó capa en el índice {i} de parallaxLayers.");
            }
        }
    }
    public void TogglePause()
{
    isPaused = !isPaused;
    pauseMenuUI.SetActive(isPaused);
    Time.timeScale = isPaused ? 0f : 1f;
}

public void ResumeGame()
{
    isPaused = false;
    pauseMenuUI.SetActive(false);
    Time.timeScale = 1f;
}

public void ReturnToMainMenu()
{
    Time.timeScale = 1f;
    SceneManager.LoadScene("MainMenu");
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
        bricksDisplay?.UpdateDisplay(bricksLeft);
    }

    public void AddPoints(int amount)
    {
        score += amount;
        scoreDisplay?.UpdateDisplay(score);
    }

    public void AddPaddleHit()
    {
        paddleHits++;
        paddleHitsDisplay?.UpdateDisplay(paddleHits);
    }

    public void LoseLife()
    {
        UIAudioManager.Instance.PlayLifeLost();
        lives--;
        livesDisplay?.UpdateDisplay(lives);

        if (lives <= 0)
        {
            UIAudioManager.Instance.PlayGameOver();
            Debug.Log("👾 Game Over!");
            // Podés recargar la escena o mostrar pantalla de Game Over
            // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public BallLogic GetMainBallLogic()
    {
        return ball;
    }

    public List<BrickInstance> GetBrickList()
    {
        return bricks;
    }
}
