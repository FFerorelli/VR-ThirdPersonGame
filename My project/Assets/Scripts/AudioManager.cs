using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to play sound at a specific position
    public void PlaySoundAtLocation(AudioClip clip, Vector3 position)
    {
        // Create a temporary GameObject at the specified position
        GameObject tempAudioObject = new GameObject("TempAudio");
        tempAudioObject.transform.position = position;

        // Add an AudioSource component, set the clip and play
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        // Destroy the GameObject after the clip duration
        Destroy(tempAudioObject, clip.length);
    }
}
