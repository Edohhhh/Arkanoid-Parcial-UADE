using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;


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
    private const int maxLevel = 10;
    [SerializeField] private List<RowConfigPerLevel> levelRowConfigs;

    [Header("Configuración de grilla")]
    public int rows = 5;
    public int columns = 10;
    public float spacing = 1.1f;

    [Header("Config. de dificultad por fila")]
    public RowConfigPerLevel currentRowConfig;

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

    [Header("Addressables Integrado")]
    [SerializeField] private List<AssetReference> assetReferences;
    [SerializeField] private List<AssetReference> levelReferences;
    [SerializeField] private bool useRemoteAssets = true;
    [SerializeField] private string localURL = "http://localhost:3000/";
    [SerializeField] private string cloudURL = "https://myserver.com/";

    private Dictionary<string, GameObject> loadedAssets = new();
    public Action OnAddressablesLoaded;

    [Header("Niveles Addressables")]
    [SerializeField] private Transform levelRoot;
    [SerializeField] private int startingLevel = 1;

    [Header("Prefabs Addressables")]
    [SerializeField] private string paddleAddressKey = "Paleta";
    [SerializeField] private string levelAddressPrefix = "Nivel";
    [SerializeField] private string powerUpAddressKey = "PowerUp";
    [SerializeField] private RowConfigPerLevel debugRowConfig;

    private int currentLevel = 0;
    private LevelController levelController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        CustomUpdateManager.Register(new DelayedMusicStart());

        if (useRemoteAssets)
            Addressables.ResourceManager.InternalIdTransformFunc += ChangeAssetUrlToPrivateServer;

        StartCoroutine(LoadAssetsCoroutine());
        SubscribeOnAssetsLoaded(OnAssetsLoaded);
    }

    private void Start()
    {
        pauseLogic = new PauseLogic(this);
        CustomUpdateManager.Register(pauseLogic);
    }


    private IEnumerator LoadAssetsCoroutine()
    {
        var allRefs = assetReferences.Concat(levelReferences).ToList();
        int total = allRefs.Count;
        int loaded = 0;

        foreach (var reference in allRefs)
        {
            var handle = reference.LoadAssetAsync<GameObject>();
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                string key = handle.Result.name;
                loadedAssets[key] = handle.Result;
                loaded++;
            }
            else
            {
                Debug.LogError($"AssetsManager (GameManager): Error cargando '{reference.RuntimeKey}': {handle.OperationException}");
            }
        }

        if (loaded == total)
            OnAddressablesLoaded?.Invoke();
    }

    private readonly Dictionary<string, float> decorParallaxData = new()
{
    { "Nube(Clone)", 0.1f },
    { "Nube2(Clone)", 0.05f },
    { "Montaña(Clone)", 0.02f },
    { "Logo(Clone)", 0.01f },
    { "Background(Clone)", 0.00f }
};


    private void OnAssetsLoaded()
    {
        levelController = new LevelController(levelRoot);
        GameObject level = GetAddressableInstance(levelAddressPrefix + startingLevel);

        if (level != null)
        {
            GameObject levelInstance = Instantiate(level, levelRoot);
            currentLevel = startingLevel;
            ForceAtlasApplyInChildren(levelInstance);
        }

        // ✅ Usar el RowConfigPerLevel directamente
        if (startingLevel - 1 < levelRowConfigs.Count && levelRowConfigs[startingLevel - 1] != null)
        {
            currentRowConfig = levelRowConfigs[startingLevel - 1];
            Debug.Log($"✅ currentRowConfig apunta a RowConfig_Level{startingLevel}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró RowConfig para nivel inicial {startingLevel}");
            currentRowConfig = null;
        }

        GameObject paddleGO = GetAddressableInstance(paddleAddressKey);
        if (paddleGO != null)
        {
            paddleGO.transform.position = paddleTransform.position;
            paddleGO.SetActive(true);
            ForceAtlasApplyInChildren(paddleGO);

            paddle = new PaddleLogic();
            paddle.transform = paddleGO.transform;
            CustomUpdateManager.Register(paddle);

            GameObject ballGO = ballPool.GetFromPool(paddleGO.transform.position + Vector3.up * 0.6f);
            ballGO.SetActive(true);
            ForceAtlasApplyInChildren(ballGO);

            ball = new BallLogic();
            ball.transform = ballGO.transform;
            ball.paddle = paddleGO.transform;
            ball.isMainBall = true;
            CustomUpdateManager.Register(ball);

            SpawnBricks();
            ball.SetBricks(bricks);

            scoreDisplay?.UpdateDisplay(score);
            livesDisplay?.UpdateDisplay(lives);
            bricksDisplay?.UpdateDisplay(bricksLeft);
            paddleHitsDisplay?.UpdateDisplay(paddleHits);
            GameManager.Instance.paddleTransform = paddleGO.transform;

            RegisterParallaxLayers();

            Dictionary<string, float> decorParallaxData = new()
        {
            { "Nube", 0.05f },
            { "Nube2", 0.05f },
            { "Map", 0.00f },
            { "Logo", 0.1f },
            { "Montaña", 0.02f }
        };

            foreach (var kvp in decorParallaxData)
            {
                GameObject go = GetAddressableInstance(kvp.Key);
                if (go != null)
                {
                    go.SetActive(true);
                    ForceAtlasApplyInChildren(go);

                    var logic = new ParallaxLayerLogic(go.transform, paddleGO.transform, kvp.Value);
                    CustomUpdateManager.Register(logic);
                }
                else
                {
                    Debug.LogWarning($"⚠️ No se pudo instanciar '{kvp.Key}' desde Addressables.");
                }
            }
        }
    }


    private string ChangeAssetUrlToPrivateServer(IResourceLocation location)
    {
        string url = location.InternalId;
        if (url.StartsWith(localURL, StringComparison.OrdinalIgnoreCase))
            url = url.Replace(localURL, cloudURL);
        return url;
    }

    public void SubscribeOnAssetsLoaded(Action callback)
    {
        OnAddressablesLoaded += callback;
    }

    public GameObject GetAddressableInstance(string assetName)
    {
        if (loadedAssets.TryGetValue(assetName, out var prefab))
            return Instantiate(prefab);

        // 🔁 Intentamos sin "(Clone)" si falló
        string cleanName = assetName.Replace("(Clone)", "");
        if (loadedAssets.TryGetValue(cleanName, out prefab))
            return Instantiate(prefab);

        Debug.LogError($"AssetsManager (GameManager): Asset '{assetName}' no encontrado.");
        return null;
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

        Debug.Log($"🔍 Probando drop: chance={dropTable.dropChance}, entries={dropTable.powerUps.Count}");

        if (effect == null)
        {
            Debug.Log("❌ No se eligió ningún powerup.");
            return;
        }

        Debug.Log($"⚡ Intentando generar PowerUp: {effect.name}");

        GameObject p = powerUpPool.GetFromPool(position);
        if (p == null)
        {
            Debug.LogError("❌ No se pudo obtener el powerup desde el pool.");
            return;
        }

        p.SetActive(true);

        PowerUpLogic logic = new PowerUpLogic();
        logic.Initialize(p.transform, powerUpPool, effect);
        logic.SetPaddle(paddle.transform); // <- esto es clave para detectar colisión con paddle
    }

    public void BrickDestroyed()
    {
        bricksLeft--;
        bricksDisplay?.UpdateDisplay(bricksLeft);

        if (bricksLeft <= 0)
        {
            Debug.Log("🏁 Nivel completado!");

            

            if (currentLevel > maxLevel)
            {
                Debug.Log("🎉 Todos los niveles completados. Escena de victoria.");
                SceneManager.LoadScene("Victory"); // Cambiá "Victory" por el nombre real de tu escena de victoria
            }
            else
            {
                LoadNextLevel();
            }
        }
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
            Debug.Log("💾 Game Over!");
        }
    }

    private void ForceAtlasApplyInChildren(GameObject root)
    {
        foreach (var atlas in root.GetComponentsInChildren<SetAtlasTile>(true))
        {
            atlas.ApplyTile();
        }
    }

    public BallLogic GetMainBallLogic() => ball;
    public List<BrickInstance> GetBrickList() => bricks;

    public void SpawnBricks()
    {
        bricks.Clear();
        bricksLeft = 0;

        for (int row = 0; row < rows; row++)
        {
            int hits = 1;
            Color color = Color.white;

            if (row < currentRowConfig.rowConfigs.Count)
            {
                hits = currentRowConfig.rowConfigs[row].hits;
                color = currentRowConfig.rowConfigs[row].color;
            }

            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = bricksStartPoint.position + new Vector3(col * spacing, -row * spacing, 0);
                GameObject brickGO = brickPool.GetFromPool(pos);
                brickGO.transform.position = pos;
                brickGO.SetActive(true);

                var atlas = brickGO.GetComponent<SetAtlasTile>();

                bool hasPower = UnityEngine.Random.Range(0f, 1f) < 0.2f;

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

    public BallLogic CreateNewBall(Vector3 position, bool followPaddle = true)
    {
        GameObject ballGO = ballPool.GetFromPool(position);
        if (ballGO == null)
        {
            Debug.LogError("❌ No se pudo obtener una pelota del pool.");
            return null;
        }

        ballGO.SetActive(true);
        ForceAtlasApplyInChildren(ballGO);

        BallLogic newBall = new BallLogic();
        newBall.transform = ballGO.transform;
        newBall.paddle = paddle.transform;
        newBall.isMainBall = false;
        newBall.SetBricks(bricks);

        CustomUpdateManager.Register(newBall);
        return newBall;
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

    private void RegisterParallaxLayers()
    {
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            if (parallaxLayers[i] != null)
            {
                var logic = new ParallaxLayerLogic(parallaxLayers[i], paddleTransform, parallaxSpeeds[i]);
                CustomUpdateManager.Register(logic);
            }
        }
    }
    public void LoadNextLevel()
    {
        currentLevel++;

        if (currentLevel > 10)
        {
            SceneManager.LoadScene("Victory");
            return;
        }

        if (currentLevel - 1 < levelRowConfigs.Count && levelRowConfigs[currentLevel - 1] != null)
        {
            currentRowConfig = levelRowConfigs[currentLevel - 1];
            Debug.Log($"✅ currentRowConfig apunta a RowConfig_Level{currentLevel}");
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró RowConfig para nivel {currentLevel}");
            currentRowConfig = null;
        }

        CustomUpdateManager.ClearAll();

        // 🔁 Re-registrar Paddle y Ball existentes
        if (paddle != null)
        {
            CustomUpdateManager.Register(paddle);
        }

        if (ball != null)
        {
            // Reposicionar la bola en el paddle
            ball.transform.position = paddle.transform.position + Vector3.up * 0.6f;
            ball.paddle = paddle.transform;
            ball.isMainBall = true;
            ball.SetBricks(bricks); // los bricks se regenerarán abajo
            CustomUpdateManager.Register(ball);
        }

        // 🔄 Desinstanciar nivel anterior
        foreach (Transform child in levelRoot)
        {
            Destroy(child.gameObject);
        }

        // ⬇️ Instanciar nuevo nivel Addressable
        GameObject level = GetAddressableInstance(levelAddressPrefix + currentLevel);
        if (level != null)
        {
            GameObject levelInstance = Instantiate(level, levelRoot);
            ForceAtlasApplyInChildren(levelInstance);
        }
        else
        {
            Debug.LogError($"❌ Nivel {currentLevel} no se pudo cargar.");
        }

        // 🧱 Regenerar bricks
        SpawnBricks();
        if (ball != null)
        {
            ball.SetBricks(bricks); // actualizar bricks en lógica de bola
        }

        // UI
        scoreDisplay?.UpdateDisplay(score);
        bricksDisplay?.UpdateDisplay(bricksLeft);
        paddleHitsDisplay?.UpdateDisplay(paddleHits);

        // Pausa, música y parallax
        pauseLogic = new PauseLogic(this);
        CustomUpdateManager.Register(pauseLogic);
        RegisterParallaxLayers();
    }

}
