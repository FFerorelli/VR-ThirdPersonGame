using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 1; // Points awarded for collecting this item

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Assuming the capsule has the tag "Player"
        {
            // Update the score
            GameManager.Instance.AddScore(scoreValue);

            // Optionally, play a sound or particle effect here

            // Disable or destroy the collectible
            gameObject.SetActive(false); // Better for object pooling
            // Or use Destroy(gameObject); if not using pooling
        }
    }
}
