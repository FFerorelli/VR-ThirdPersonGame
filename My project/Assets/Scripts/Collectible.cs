using UnityEngine;

public class Collectible : MonoBehaviour
{
    public int scoreValue = 1;  // Points awarded for collecting this item
    public AudioClip collectSound;  // Reference to the sound clip to play on collection

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))  // Assuming the capsule has the tag "Player"
        {
            // Update the score
            GameManager.Instance.AddScore(scoreValue);

            // Play the sound at the collision point
            if (collectSound != null)
            {
                AudioManager.Instance.PlaySoundAtLocation(collectSound, transform.position);
            }

            // Immediately deactivate the collectible
            gameObject.SetActive(false);  // Better for object pooling
        }
    }
}
