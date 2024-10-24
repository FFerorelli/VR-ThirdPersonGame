using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 1f;  // Time between each spawn
    private int activationCount = 0;  // Number of activations
    private int totalObjectsInPool = 0;  // Total number of objects in the pool

    private void Start()
    {
        // Start the coroutine to gradually activate objects from the pool
        StartCoroutine(WaitForPoolToInitialize());
    }

    private IEnumerator WaitForPoolToInitialize()
    {
        // Wait until objects have been added to the pool
        while (ObjectPoolManager.Instance.GetTotalObjectCount() == 0)
        {
            yield return null;  // Wait for the pool to be populated
        }

        // Once the pool is initialized, get the total object count
        totalObjectsInPool = ObjectPoolManager.Instance.GetTotalObjectCount();
        Debug.Log($"Total objects in pool: {totalObjectsInPool}");

        // Now start the activation coroutine
        StartCoroutine(ActivateObjectsFromPoolRoutine());
    }

    private IEnumerator ActivateObjectsFromPoolRoutine()
    {
        while (activationCount < totalObjectsInPool)  // Activate objects until the limit is reached
        {
            // Get a random inactive object from the pool
            GameObject pooledObject = ObjectPoolManager.Instance.GetRandomPooledObject();

            if (pooledObject != null)
            {
                // Activate the object (it retains its original position from FindSpawnPositions)
                pooledObject.SetActive(true);
                activationCount++;  // Increment the number of activations
                Debug.Log($"Activated object {pooledObject.name}, total activations: {activationCount}");
            }
            else
            {
                Debug.Log("No available objects in the pool.");
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }

        Debug.Log("All objects in the pool have been activated.");
    }
}
