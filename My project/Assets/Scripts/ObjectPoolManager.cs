using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    // List to store pooled objects
    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add objects to the pool
    public void AddToPool(GameObject obj)
    {
        obj.SetActive(false);  // Deactivate the object when adding to the pool
        pooledObjects.Add(obj);
        Debug.Log($"Object {obj.name} added to pool.");
    }

    // Get a random inactive object from the pool
    public GameObject GetRandomPooledObject()
    {
        List<GameObject> inactiveObjects = new List<GameObject>();

        // Collect all inactive objects
        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                inactiveObjects.Add(obj);
            }
        }

        // If there are inactive objects, return one at random
        if (inactiveObjects.Count > 0)
        {
            int randomIndex = Random.Range(0, inactiveObjects.Count);
            Debug.Log($"Activating object: {inactiveObjects[randomIndex].name}");
            return inactiveObjects[randomIndex];
        }

        // If no inactive objects are available, return null
        Debug.LogWarning("No inactive objects available in pool.");
        return null;
    }

    // Return an object to the pool (deactivate it)
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);  // Deactivate the object and return it to the pool
        Debug.Log($"Object {obj.name} returned to pool.");
    }

    // Get the total number of objects in the pool
    public int GetTotalObjectCount()
    {
        Debug.Log($"Total objects in pool: {pooledObjects.Count}");
        return pooledObjects.Count;
    }
}
