using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickInstance
{
    public GameObject gameObject;
    public BrickLogic logic;
    public bool TakeHit() => logic.TakeHit();
    public BrickInstance(GameObject brickGO, int hits, bool hasPowerUp, ObjectPool powerUpPool, GameObject powerUpPrefab, SetAtlasTile atlas)
    {
        gameObject = brickGO;
        logic = new BrickLogic
        {
            transform = brickGO.transform,
            hitsToBreak = hits,
            hasPowerUp = hasPowerUp,
            dropChance = 30f,
            powerUpPool = powerUpPool,
            powerUpPrefab = powerUpPrefab,
            atlasTile = atlas
        };

        logic.ConfigureAtlas();
    }

    public bool IsActive()
    {
        return gameObject != null && gameObject.activeInHierarchy;
    }

    public bool CheckCollision(Vector3 pos, float radius) => logic.CheckCollision(pos, radius);


    public void DestroyCompletely()
    {
        logic.hitsToBreak = 0;
        gameObject.SetActive(false);
    }
}
