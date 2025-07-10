using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController
{
    private readonly Transform _nivelParent;

    public LevelController(Transform nivelContainer)
    {
        _nivelParent = nivelContainer;
    }

    public void LoadLevel(int levelNumber)
    {
        // Borrar hijos anteriores
        foreach (Transform child in _nivelParent)
            Object.Destroy(child.gameObject);

        GameObject newLevel = GameManager.Instance.GetAddressableInstance($"Nivel{levelNumber}");
        if (newLevel == null)
        {
            Debug.LogError($"LevelController: No se pudo cargar Nivel{levelNumber}");
            return;
        }

        GameObject.Instantiate(newLevel, _nivelParent);
    }
}