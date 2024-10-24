using UnityEngine;
using System.Collections;

public class PoolInitializer : MonoBehaviour
{
    [SerializeField] private string objectTag = "Collectible";  // Tag for spawned objects
    private int expectedObjectCount = 10;  // Number of objects FindSpawnPositions should spawn

    private void Start()
    {
        // Start the coroutine to pool objects once FindSpawnPositions finishes
        StartCoroutine(AddSpawnedObjectsToPool());
    }

    private IEnumerator AddSpawnedObjectsToPool()
    {
        // Wait until all the objects are spawned (based on the expected count)
        while (GameObject.FindGameObjectsWithTag(objectTag).Length < expectedObjectCount)
        {
            yield return null;  // Wait one frame and recheck
        }

        // Now that all objects are spawned, add them to the pool
        GameObject[] spawnedObjects = GameObject.FindGameObjectsWithTag(objectTag);
        foreach (GameObject obj in spawnedObjects)
        {
            ObjectPoolManager.Instance.AddToPool(obj);  // Add them to the pool and deactivate them
        }

        //Debug.Log("All spawned objects have been added to the pool.");
    }
}
