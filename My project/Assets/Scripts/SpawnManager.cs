using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 5f;  // Time between each spawn

    private void Start()
    {
        // Start the coroutine to gradually activate objects from the pool
        StartCoroutine(ActivateObjectsFromPoolRoutine());
    }

    private IEnumerator ActivateObjectsFromPoolRoutine()
    {
        while (true)
        {
            // Get a random inactive object from the pool
            GameObject pooledObject = ObjectPoolManager.Instance.GetRandomPooledObject();

            if (pooledObject != null)
            {
                // Activate the object (it retains its original position from FindSpawnPositions)
                pooledObject.SetActive(true);
            }
            else
            {
                Debug.Log("No available objects in the pool.");
            }

            // Wait for the next spawn interval
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
