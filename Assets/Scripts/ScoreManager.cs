using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public NumericDisplay scoreDisplay;

    private int score = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (scoreDisplay == null)
            scoreDisplay = FindObjectOfType<NumericDisplay>();

        scoreDisplay?.UpdateDisplay(score);
    }

    public void AddPoints(int amount)
    {
        score += amount;

        if (scoreDisplay == null)
            scoreDisplay = FindObjectOfType<NumericDisplay>();

        scoreDisplay?.UpdateDisplay(score);
    }

    public void ResetScore()
    {
        score = 0;
        scoreDisplay?.UpdateDisplay(score);
    }
}
