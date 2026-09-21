using System.Collections.Generic;
using UnityEngine;

public class MousePool : MonoBehaviour
{

    [Header("Player")]
    [SerializeField] private Transform playerTransform;

    [Header("Mouse Prefabs")]
    [SerializeField] private GameObject[] mousePrefabs;

    [SerializeField] private int poolSizePerType = 10;

    [Header("Mouse Holes")]
    [SerializeField] private Transform[] mouseHoles;

    [Header("Spawn")]
    [SerializeField] private float spawnInterval = 2f;

    [SerializeField] private int maxMouseInRoom = 20;

    [Header("Active Limit")]
    [SerializeField] private int maxActiveMouse = 8;

    [Header("Spawn Count")]
    [SerializeField] private int minMousePerSpawn = 1;
    [SerializeField] private int maxMousePerSpawn = 3;

    // Mỗi prefab có một pool riêng
    private Dictionary<GameObject, Queue<GameObject>> mousePools =
        new Dictionary<GameObject, Queue<GameObject>>();

    private float spawnTimer;

    // Tổng số chuột đã spawn trong level
    private int totalMouseSpawned;

    // Số chuột đang active
    private int activeMouseCount;

    private void Awake()
    {
        CreatePools();
    }

    private void Update()
    {
        HandleSpawn();
    }

    //==================================================
    // CREATE POOLS
    //==================================================
    public int TotalMouse()
    {
        return maxMouseInRoom;
    }
    private void CreatePools()
    {
        if (mousePrefabs == null || mousePrefabs.Length == 0)
        {
            Debug.LogError("MousePool: Chưa có Mouse Prefabs!");
            return;
        }

        foreach (GameObject prefab in mousePrefabs)
        {
            if (prefab == null)
                continue;

            Queue<GameObject> pool = new Queue<GameObject>();

            for (int i = 0; i < poolSizePerType; i++)
            {
                GameObject mouse = Instantiate(
                    prefab,
                    transform
                );

                mouse.SetActive(false);

                pool.Enqueue(mouse);
            }

            mousePools.Add(prefab, pool);
        }
    }

    //==================================================
    // GET MOUSE
    //==================================================

    private GameObject GetMouse(GameObject prefab)
    {
        if (prefab == null)
            return null;

        if (!mousePools.TryGetValue(prefab, out Queue<GameObject> pool))
        {
            Debug.LogWarning(
                $"MousePool: Không tìm thấy pool cho {prefab.name}"
            );

            return null;
        }

        if (pool.Count == 0)
        {
            Debug.LogWarning(
                $"MousePool: Pool của {prefab.name} đã hết!"
            );

            return null;
        }

        GameObject mouse = pool.Dequeue();

        mouse.SetActive(true);

        return mouse;
    }

    //==================================================
    // RETURN MOUSE
    //==================================================

    public void ReturnMouse(
        GameObject mouse,
        GameObject prefab
    )
    {
        if (mouse == null || prefab == null)
            return;

        if (!mousePools.TryGetValue(
            prefab,
            out Queue<GameObject> pool))
        {
            return;
        }

        mouse.SetActive(false);

        pool.Enqueue(mouse);

        activeMouseCount--;

        if (activeMouseCount < 0)
            activeMouseCount = 0;
    }

    //==================================================
    // SPAWN
    //==================================================

    private void HandleSpawn()
    {
        if (totalMouseSpawned >= maxMouseInRoom)
            return;

        if (activeMouseCount >= maxActiveMouse)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer < spawnInterval)
            return;

        spawnTimer = 0f;

        SpawnRandomWave();
    }

    private void SpawnRandomWave()
    {
        if (mouseHoles == null || mouseHoles.Length == 0)
            return;

        if (mousePrefabs == null || mousePrefabs.Length == 0)
            return;

        int remainingMouse =
            maxMouseInRoom - totalMouseSpawned;

        int availableActiveSlot =
            maxActiveMouse - activeMouseCount;

        int spawnCount = Random.Range(
            minMousePerSpawn,
            maxMousePerSpawn + 1
        );

        spawnCount = Mathf.Min(
            spawnCount,
            remainingMouse
        );

        spawnCount = Mathf.Min(
            spawnCount,
            availableActiveSlot
        );

        for (int i = 0; i < spawnCount; i++)
        {
            Transform hole = GetRandomHole();

            if (hole == null)
                continue;

            GameObject randomPrefab =
                GetRandomMousePrefab();

            if (randomPrefab == null)
                continue;

            SpawnMouse(
                randomPrefab,
                hole
            );
        }
    }

    //==================================================
    // SPAWN ONE MOUSE
    //==================================================

    private void SpawnMouse(
        GameObject prefab,
        Transform hole
    )
    {
        if (prefab == null || hole == null)
            return;

        if (totalMouseSpawned >= maxMouseInRoom)
            return;

        if (activeMouseCount >= maxActiveMouse)
            return;

        GameObject mouse = GetMouse(prefab);

        if (mouse == null)
            return;

        mouse.transform.SetPositionAndRotation(
            hole.position,
            hole.rotation
        );

        totalMouseSpawned++;
        activeMouseCount++;

        MouseAI mouseAI =
            mouse.GetComponent<MouseAI>();

        if (mouseAI != null)
        {
            mouseAI.SetTarget(playerTransform);
        }
    }

    //==================================================
    // RANDOM
    //==================================================

    private GameObject GetRandomMousePrefab()
    {
        int index = Random.Range(
            0,
            mousePrefabs.Length
        );

        return mousePrefabs[index];
    }

    private Transform GetRandomHole()
    {
        int index = Random.Range(
            0,
            mouseHoles.Length
        );

        return mouseHoles[index];
    }

    //==================================================
    // LEVEL
    //==================================================

    public void ResetLevel()
    {
        spawnTimer = 0f;

        totalMouseSpawned = 0;
        activeMouseCount = 0;

        foreach (KeyValuePair<GameObject, Queue<GameObject>> pair
                 in mousePools)
        {
            Queue<GameObject> pool = pair.Value;

            // Đưa các mouse active về pool
            MouseAI[] mice =
                GetComponentsInChildren<MouseAI>(true);

            foreach (MouseAI mouse in mice)
            {
                if (mouse.gameObject.activeSelf)
                {
                    mouse.gameObject.SetActive(false);

                    if (!pool.Contains(mouse.gameObject))
                    {
                        pool.Enqueue(mouse.gameObject);
                    }
                }
            }
        }
    }

    //==================================================
    // GETTERS
    //==================================================

    public int GetActiveMouseCount()
    {
        return activeMouseCount;
    }

    public int GetTotalMouseSpawned()
    {
        return totalMouseSpawned;
    }
}