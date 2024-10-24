using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float gameTime = 60f; // Total time for the game in seconds
    private int totalObjectsToSpawn; // Total number of objects in the game

    [Header("UI Elements")]
    public GameObject gameOverUI; // Reference to the Game Over UI
    public GameObject winUI; // Reference to the Win UI

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
        // Wait for the pool to be initialized before starting the game
        StartCoroutine(WaitForPoolInitialization());
    }

    private IEnumerator WaitForPoolInitialization()
    {
        // Wait until the object pool has been populated
        while (ObjectPoolManager.Instance.GetTotalObjectCount() == 0)
        {
            yield return null;  // Wait for one frame and recheck
        }

        // Now the pool is populated, get the total object count
        totalObjectsToSpawn = ObjectPoolManager.Instance.GetTotalObjectCount();

        timeRemaining = gameTime;
        StartCoroutine(GameTimer());

        // Deactivate the Game Over and Win UIs at the start
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
    }

    IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && !isGameOver)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            OnTimeChanged.Invoke(timeRemaining);  // Let the event update the timer UI
        }

        if (!isGameOver) GameOver();  // Game ends if time runs out and the game isn't over yet
    }

    public void AddScore(int amount)
    {
        if (isGameOver)
            return;

        score += amount;
        OnScoreChanged.Invoke(score);  // Let the event update the score UI

        // Check if the player has won the game
        if (score >= totalObjectsToSpawn)
        {
            WinGame();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        OnGameOver.Invoke();

        // Activate Game Over UI
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        // Additional game over logic (e.g., stop player movement)
        Debug.Log("Game Over.");
    }

    void WinGame()
    {
        isGameOver = true;
        OnGameOver.Invoke();

        // Activate Win UI
        if (winUI != null)
            winUI.SetActive(true);

        Debug.Log("You Win!");
    }
}
