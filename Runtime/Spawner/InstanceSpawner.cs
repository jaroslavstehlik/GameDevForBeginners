using UnityEngine;
using UnityEngine.Events;

public class InstanceSpawner : MonoBehaviour
{
    // Public event when spawner spawns an object
    public UnityEvent<GameObject> onSpawn;
    
    // GameObject to spawn
    public GameObject spawnGameObject;

    // Where to place our spawned object
    public Transform spawnLocation;

    public void Spawn()
    {
        // Clone the game object and store it in local variable
        GameObject spawnedGameObject = Instantiate(spawnGameObject);
            
        // Set the parent hierarchy transform to spawn location
        // This will prevent clutter in the scene
        spawnedGameObject.transform.SetParent(spawnLocation);
            
        // Set the spawned game object position to our spawn location
        spawnedGameObject.transform.position = spawnLocation.position;
            
        // Set the spawned game object rotation to our spawn location
        spawnedGameObject.transform.rotation = spawnLocation.rotation;
            
        // Check if anyone listens to our event
        if (onSpawn != null)
        {
            // Invoke event
            onSpawn.Invoke(spawnedGameObject);
        }
    }
}
