using UnityEngine;
using TMPro; // Add this namespace

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI scoreText; // Use TextMeshProUGUI
    public TextMeshProUGUI timerText; // Use TextMeshProUGUI

    void OnEnable()
    {
        
        GameManager.Instance.OnScoreChanged.AddListener(UpdateScore);
        GameManager.Instance.OnTimeChanged.AddListener(UpdateTimer);
        GameManager.Instance.OnGameOver.AddListener(HandleGameOver);
    }

    void OnDisable()
    {
        GameManager.Instance.OnScoreChanged.RemoveListener(UpdateScore);
        GameManager.Instance.OnTimeChanged.RemoveListener(UpdateTimer);
        GameManager.Instance.OnGameOver.RemoveListener(HandleGameOver);
    }

    void UpdateScore(int newScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + newScore;
    }

    void UpdateTimer(float timeRemaining)
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.Clamp(timeRemaining, 0, Mathf.Infinity);
    }

    void HandleGameOver()
    {
        // Show Game Over UI or perform other actions
        Debug.Log("Game Over");
        // Here you can add logic to display a Game Over screen or freeze input
    }
}
