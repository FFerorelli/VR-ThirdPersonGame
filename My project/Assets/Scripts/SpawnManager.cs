using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] private float spawnInterval = 1f;  // Time between each spawn
    private int activationCount = 0;  // Number of activations
    private int totalObjectsInPool;  // Total number of objects in the pool
    private bool canSpawn = true;  // Control whether objects can be spawned

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        // Subscribe to the GameOver event from GameManager
        GameManager.Instance.OnGameOver.AddListener(StopSpawning);

        // Wait for the pool to be initialized before starting spawning
        StartCoroutine(WaitForPoolInitialization());
    }

    private IEnumerator WaitForPoolInitialization()
    {
        // Wait until the object pool has been populated
        while (ObjectPoolManager.Instance.GetTotalObjectCount() == 0)
        {
            yield return null;  // Wait for one frame and recheck
        }

        // Get the total number of objects in the pool
        totalObjectsInPool = ObjectPoolManager.Instance.GetTotalObjectCount();
        StartCoroutine(ActivateObjectsFromPoolRoutine());
    }

    private IEnumerator ActivateObjectsFromPoolRoutine()
    {
        while (activationCount < totalObjectsInPool && canSpawn)  // Check if we have activated fewer than the total objects
        {
            // Get a random inactive object from the pool
            GameObject pooledObject = ObjectPoolManager.Instance.GetRandomPooledObject();

            if (pooledObject != null)
            {
                // Activate the object (it retains its original position from FindSpawnPositions)
                pooledObject.SetActive(true);
                activationCount++;  // Increment the number of activations
                //Debug.Log($"Activated object {pooledObject.name}, total activations: {activationCount}");
            }
            else
            {
                //Debug.Log("No available objects in the pool.");
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }

        //Debug.Log("All objects in the pool have been activated or spawning has been stopped.");
    }

    private void StopSpawning()
    {
        canSpawn = false;
    }

    // New method to restart spawning
    public void RestartSpawning()
    {
        canSpawn = true;
        activationCount = 0;  // Reset activation count
        StartCoroutine(ActivateObjectsFromPoolRoutine());  // Restart the spawn process
    }
}
