using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public float gameTime = 60f; // Total time for the game in seconds
    private int totalObjectsToSpawn; // Total number of objects in the game

    [Header("UI Elements")]
    public GameObject gameOverUI; // Reference to the Game Over UI
    public GameObject winUI; // Reference to the Win UI

    [Header("References")]
    public CapsuleController capsuleController;  // Reference to the CapsuleController
    public Button restartButtonWinUI;  // Button in the Win UI
    public Button restartButtonGameOverUI;  // Button in the Game Over UI
    private int score = 0;
    private float timeRemaining;
    private bool isGameOver = false;

    // Events for updating UI
    public UnityEvent<int> OnScoreChanged;
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent OnGameOver;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        if (OnScoreChanged == null)
            OnScoreChanged = new UnityEvent<int>();

        if (OnTimeChanged == null)
            OnTimeChanged = new UnityEvent<float>();

        if (OnGameOver == null)
            OnGameOver = new UnityEvent();
    }

    void Start()
    {
        // Hook the restart buttons to the RestartGame method
        if (restartButtonWinUI != null)
        {
            restartButtonWinUI.onClick.AddListener(RestartGame);
        }

        if (restartButtonGameOverUI != null)
        {
            restartButtonGameOverUI.onClick.AddListener(RestartGame);
        }

       
        StartCoroutine(WaitForPoolInitialization());
    }

    private IEnumerator WaitForPoolInitialization()
    {
        while (ObjectPoolManager.Instance.GetTotalObjectCount() == 0)
        {
            yield return null;
        }

        totalObjectsToSpawn = ObjectPoolManager.Instance.GetTotalObjectCount();
        timeRemaining = gameTime;
        StartCoroutine(GameTimer());

        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);
    }

    IEnumerator GameTimer()
    {
        while (timeRemaining > 0 && !isGameOver)
        {
            yield return new WaitForSeconds(1f);
            timeRemaining--;
            OnTimeChanged.Invoke(timeRemaining);
        }

        if (!isGameOver) GameOver();
    }

    public void AddScore(int amount)
    {
        if (isGameOver)
            return;

        score += amount;
        OnScoreChanged.Invoke(score);

        if (score >= totalObjectsToSpawn)
        {
            WinGame();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        OnGameOver.Invoke();

        if (gameOverUI != null)
            gameOverUI.SetActive(true);
    }

    void WinGame()
    {
        isGameOver = true;
        OnGameOver.Invoke();

        if (winUI != null)
            winUI.SetActive(true);
    }

    public void RestartGame()
    {
        if (gameOverUI != null) gameOverUI.SetActive(false);
        if (winUI != null) winUI.SetActive(false);

        isGameOver = false;
        score = 0;
        timeRemaining = gameTime;

        OnScoreChanged.Invoke(score);

        ObjectPoolManager.Instance.DeactivateAllPooledObjects();
        StartCoroutine(GameTimer());
        SpawnManager.Instance.RestartSpawning();

        // Re-enable capsule movement
        if (capsuleController != null)
        {
            capsuleController.EnableMovement();
        }

        //Debug.Log("Game restarted.");
    }
}
