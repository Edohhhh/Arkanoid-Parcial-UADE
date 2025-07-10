using System.Collections;
using UnityEngine;

public class LoaderController
{
    private readonly ObjectPool _ballPool;
    private readonly Vector3 _spawnPosition;
    private readonly Vector3 _initialDirection;

    public LoaderController(ObjectPool ballPool, Vector3 spawnPosition, Vector3 initialDirection)
    {
        _ballPool = ballPool;
        _spawnPosition = spawnPosition;
        _initialDirection = initialDirection;

        GameManager.Instance.SubscribeOnAssetsLoaded(OnAssetsLoaded);
    }

    private void OnAssetsLoaded()
    {
        // Obtener y configurar la pelota principal
        GameObject ballGO = _ballPool.GetFromPool(_spawnPosition);
        if (ballGO == null)
        {
            Debug.LogError("LoaderController: no pudo obtener bola del pool");
            return;
        }

        ballGO.SetActive(true);

        BallLogic ballLogic = new BallLogic
        {
            transform = ballGO.transform,
            paddle = GameManager.Instance.paddleTransform,
            isMainBall = true
        };

        ballLogic.SetBricks(GameManager.Instance.GetBrickList());

        CustomUpdateManager.Register(ballLogic);

        // Obtener y posicionar la paleta desde addressables
        GameObject paddleGO = GameManager.Instance.GetAddressableInstance("Paleta");
        if (paddleGO == null)
        {
            Debug.LogError("LoaderController: no se pudo cargar la paleta");
            return;
        }

        paddleGO.transform.position = GameManager.Instance.paddleTransform.position;
        paddleGO.SetActive(true);

        PaddleLogic paddleLogic = new PaddleLogic
        {
            transform = paddleGO.transform
        };

        CustomUpdateManager.Register(paddleLogic);
    }
}