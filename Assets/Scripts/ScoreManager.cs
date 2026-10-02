using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Score Values")]
    [SerializeField] private int enemyScoreValue = 100;
    [SerializeField] private int debrisScoreValue = 25;

    private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddEnemyScore()
    {
        AddScore(enemyScoreValue);
    }

    public void AddDebrisScore()
    {
        AddScore(debrisScoreValue);
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"SCORE: {currentScore}";
        }
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }
}