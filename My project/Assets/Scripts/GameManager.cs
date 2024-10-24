using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro; // Add this namespace

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float gameTime = 60f; // Total time for the game in seconds

    private int score = 0;
    private float timeRemaining;
    private bool isGameOver = false;

    // Events for updating UI
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent OnGameOver;

    void Awake()
    {
        // Singleton pattern to ensure only one GameManager exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Initialize events
        if (OnScoreChanged == null)
            OnScoreChanged = new UnityEvent<int>();

        if (OnTimeChanged == null)
            OnTimeChanged = new UnityEvent<float>();

        if (OnGameOver == null)
            OnGameOver = new UnityEvent();
    }

    void Start()
    {
        timeRemaining = gameTime;
        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && !isGameOver)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            OnTimeChanged.Invoke(timeRemaining);  // Let the event update the timer UI
        }

        GameOver();
    }

    public void AddScore(int amount)
    {
        if (isGameOver)
            return;

        score += amount;
        OnScoreChanged.Invoke(score);  // Let the event update the score UI
    }

    void GameOver()
    {
        isGameOver = true;
        OnGameOver.Invoke();
        // Additional game over logic (e.g., show Game Over screen)
    }
}
