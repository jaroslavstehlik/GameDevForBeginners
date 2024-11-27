using UnityEngine;
using UnityEngine.Events;

namespace GameDevForBeginners
{
    [AddComponentMenu("GMD/Spawner/MemoryPoolSpawner")]
    public class MemoryPoolSpawner : MonoBehaviour
    {
        // maximum number of GameObjects to spawn
        public int maxSpawnCount = 10;

        // GameObject to spawn
        public GameObject spawnGameObject;

        // Where to place our spawned object
        public Transform spawnLocation;

        // Public event when spawner spawns an object
        public UnityEvent<GameObject> onSpawn;

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

        public void Spawn()
        {
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
            if (onSpawn != null)
                // Invoke event
                onSpawn.Invoke(spawnedGameObject);
        }
    }
}