using Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPooling : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public static ObjectPooling Instance;

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private bool isInitialized = false; // Prevents OnSceneLoaded from interfering with Start

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!isInitialized)
        {
            InitializePools();
            isInitialized = true;
        }
    }

    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            if (pool.prefab == null)
            {
                Debug.LogError($"Pool '{pool.tag}' has a null prefab!");
                continue;
            }

            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                DespawnObject(obj);
                objectPool.Enqueue(obj);

                Enemy enemy = obj.GetComponent<Enemy>();
                if (enemy != null) enemy.Initialize();
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag {tag} doesn't exist.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        if (objectToSpawn == null)
        {
            Debug.LogError($"Spawned object from pool {tag} is null!");
            return null;
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        IPooledObject pooledObj = objectToSpawn.GetComponent<IPooledObject>();

        if (pooledObj != null)
        {
            pooledObj.OnObjectSpawn();
        }

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    public void DespawnObject(GameObject objectToDespawn)
    {
        objectToDespawn.transform.position = new Vector3(1000, 1000, 1000);
        objectToDespawn.SetActive(false);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!isInitialized)
        {
            return; // Avoid running during the initial Play mode load
        }

        foreach (var pool in poolDictionary)
        {
            foreach (var obj in pool.Value)
            {
                if (obj != null) Destroy(obj);
            }
        }

        poolDictionary.Clear();
        InitializePools();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
