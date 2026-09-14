using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Safe Maps")]
    [SerializeField] private GameObject[] safeMapPrefabs;

    [Header("Obstacle Maps")]
    [SerializeField] private GameObject[] obstacleMapPrefabs;

    [Header("Map Settings")]
    [SerializeField] private float mapLength = 80f;
    [SerializeField] private int initialSafeMapCount = 3;
    [SerializeField] private int initialMapCount = 6;

    private readonly List<GameObject> spawnedMaps = new List<GameObject>();

    private float nextSpawnZ;
    private int spawnedMapCount;

    private void Start()
    {
        for (int i = 0; i < initialMapCount; i++)
        {
            SpawnMap();
        }
    }

    private void Update()
    {
        SpawnNextMap();
        DestroyOldMap();
    }

    private void SpawnNextMap()
    {
        if (spawnedMaps.Count < initialMapCount)
            return;

        if (nextSpawnZ < 240f)
            return;

        SpawnMap();
    }

    private void SpawnMap()
    {
        GameObject[] mapPool;

        if (spawnedMapCount < initialSafeMapCount)
            mapPool = safeMapPrefabs;
        else
            mapPool = obstacleMapPrefabs;

        if (mapPool.Length == 0)
            return;

        int randomIndex = Random.Range(0, mapPool.Length);

        GameObject map = Instantiate(
            mapPool[randomIndex],
            new Vector3(0f, 0f, nextSpawnZ),
            Quaternion.identity,
            transform
        );

        spawnedMaps.Add(map);

        nextSpawnZ += mapLength;
        spawnedMapCount++;
    }

    private void DestroyOldMap()
    {
        for (int i = spawnedMaps.Count - 1; i >= 0; i--)
        {
            GameObject map = spawnedMaps[i];

            if (map == null)
            {
                spawnedMaps.RemoveAt(i);
                continue;
            }

            if (map.transform.position.z < -mapLength)
            {
                Destroy(map);
                spawnedMaps.RemoveAt(i);
            }
        }
    }
}