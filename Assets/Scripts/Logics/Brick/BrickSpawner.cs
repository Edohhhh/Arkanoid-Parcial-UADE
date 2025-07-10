using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [Header("Pool de ladrillos")]
    public ObjectPool brickPool;

    [Header("Tamaño de la grilla")]
    [Range(1, 20)] public int rows = 5;
    [Range(1, 20)] public int columns = 10;

    [Header("Espaciado entre ladrillos")]
    public float spacing = 1.1f;

    [Header("Punto inicial de la grilla")]
    public Transform startPoint;

    [Header("Configuración")]
    public ObjectPool powerUpPool;
    public GameObject powerUpPrefab;

    public List<BrickInstance> brickInstances = new();

    public void SpawnBricks()
    {
        // Validación inicial
        rows = Mathf.Max(1, rows);
        columns = Mathf.Max(1, columns);

        if (brickPool == null || startPoint == null)
        {
            Debug.LogError("❌ Faltan referencias en BrickSpawner (brickPool o startPoint)");
            return;
        }

        Debug.Log($"🧱 Generando grilla de {rows} filas × {columns} columnas");

        brickInstances.Clear();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 pos = startPoint.position + new Vector3(col * spacing, -row * spacing, 0);
                GameObject brickGO = brickPool.GetFromPool(pos);

                if (brickGO == null)
                {
                    Debug.LogWarning($"⚠️ Ladrillo null en ({row},{col})");
                    continue;
                }

                // 🔧 Corrección clave: aseguramos posición y activación
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

                brickInstances.Add(brick);
                GameManager.Instance.bricksLeft++;
            }
        }
    }
}