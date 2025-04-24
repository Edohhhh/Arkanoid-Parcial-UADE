using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public NumericDisplay livesDisplay;

    private int currentLives = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        // Intentamos encontrar el NumericDisplay si no está asignado
        if (livesDisplay == null)
        {
            livesDisplay = FindObjectOfType<NumericDisplay>();
        }

        livesDisplay?.UpdateDisplay(currentLives); // Evitamos crash si sigue siendo null
    }

    public void LoseLife(int amount = 1)
    {
        currentLives -= amount;

        // Intentamos recuperar el display si se rompió la referencia tras recargar
        if (livesDisplay == null)
        {
            livesDisplay = FindObjectOfType<NumericDisplay>();
        }

        livesDisplay?.UpdateDisplay(currentLives);

        if (currentLives <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    // Método opcional para forzar actualización si se re-registra un display
    public void ForceRefresh()
    {
        if (livesDisplay != null)
        {
            livesDisplay.UpdateDisplay(currentLives);
        }
    }
}
