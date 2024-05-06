using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerLevel1 : MonoBehaviour
{
    // Public event when spawner spawns an object
    public UnityEvent onSpawn;
    
    // Duration of spawner
    public float duration = 1f;
    
    // GameObject to spawn
    public GameObject spawnGameObject;

    // Where to place our spawned object
    public Transform spawnLocation;
    
    // Define coroutine so we can later stop it
    private IEnumerator coroutine;
    
    // Monobehaviour calls this method when component is enabled in scene
    void OnEnable()
    {
        // Store coroutine in to variable
        coroutine = TimerCoroutine();
        
        // Start coroutine
        StartCoroutine(coroutine);
    }

    // Monobehaviour calls this method when component is disabled in scene
    void OnDisable()
    {
        // Stop coroutine
        StopCoroutine(coroutine);
        
        // Clear variable
        coroutine = null;
    }

    // The coroutine returns IEnumerator which tells Unity when to stop
    IEnumerator TimerCoroutine()
    {
        // We will spawn objects until spawner component is enabled
        while (enabled)
        {
            // Yield means that we want this function to run across multiple frames
            // WaitForSeconds means that the function will wait certain amount of time
            // before it continues execution
            yield return new WaitForSeconds(duration);

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
            if(onSpawn != null)
                // Invoke event
                onSpawn.Invoke();
        }
    }
}
