using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class MemoryPoolSpawner : MonoBehaviour
{
    // Public event when spawner spawns an object
    public UnityEvent<GameObject> onSpawn;
    
    // Duration of spawner
    public float duration = 1f;

    // maximum number of GameObjects to spawn
    public int maxSpawnCount = 10;
    
    // GameObject to spawn
    public GameObject spawnGameObject;

    // Where to place our spawned object
    public Transform spawnLocation;
    
    // Store coroutine so we can later stop it
    private IEnumerator coroutine;

    private GameObject[] memoryPool;

    private int memoryPoolSpawnerIndex = 0;

    private void Awake()
    {
        // Create an array representing our memory pool
        memoryPool = new GameObject[maxSpawnCount];
        
        // Iterate over each element of the array
        for (int i = 0; i < memoryPool.Length; i++)
        {
            // Clone the spawn game object and assign it in the arrays index
            memoryPool[i] = Instantiate(spawnGameObject, spawnLocation);
            
            // Disable the game object immediately
            memoryPool[i].SetActive(false);
        }
    }

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

            // Get the spawned game object from our memory pool
            GameObject spawnedGameObject = memoryPool[memoryPoolSpawnerIndex];

            // If the object contains rigidbody we need to reset its velocity 
            Rigidbody rigidbody = spawnedGameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.velocity = Vector3.zero;
                rigidbody.angularVelocity = Vector3.zero;
            }
            
            // Set the parent hierarchy transform to spawn location
            // This will prevent clutter in the scene
            spawnedGameObject.transform.SetParent(spawnLocation);
            
            // Set the spawned game object position to our spawn location
            spawnedGameObject.transform.position = spawnLocation.position;
            
            // Set the spawned game object rotation to our spawn location
            spawnedGameObject.transform.rotation = spawnLocation.rotation;
            
            // Activate the spawned game object
            spawnedGameObject.SetActive(true);

            // Increment our memory pool spawner index
            memoryPoolSpawnerIndex++;

            // Make sure that our spawner index does not overflow our memory pool
            if (memoryPoolSpawnerIndex >= memoryPool.Length)
                memoryPoolSpawnerIndex = 0;
            
            // Check if anyone listens to our event
            if(onSpawn != null)
                // Invoke event
                onSpawn.Invoke(spawnedGameObject);
        }
    }
}
