using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject safeMapPrefab;
    [SerializeField] GameObject[] mapPrefabs;
    [SerializeField] Transform player;
    [SerializeField] float mapLength = 80f;
    [SerializeField] int forwardMapCount = 6;
    [SerializeField] int safeMapCount = 3;
    [SerializeField] float worldSpeed = 30f;
    [SerializeField] float keepBehindDistance = 250f;
    readonly Queue<GameObject> spawnedMaps = new Queue<GameObject>();
    GameObject farthestMap;
    int mapCount;

    void Update()
    {
        SpawnMapsAhead();
        RemoveMapsBehind();
    }

    void SpawnMapsAhead()
    {
        float requiredZ = player.position.z + mapLength * forwardMapCount;
        float farthestZ = GetFarthestZ();
        int guard = 0;
        while (farthestZ < requiredZ)
        {
            float spawnZ = farthestZ + mapLength;
            if (!SpawnMap(spawnZ))
                break;
            farthestZ = spawnZ;
            if (++guard > 2000)
                break;
        }
    }

    float GetFarthestZ()
    {
        return farthestMap != null ? farthestMap.transform.position.z : player.position.z - mapLength;
    }

    bool SpawnMap(float spawnZ)
    {
        bool isSafe = mapCount < safeMapCount;
        GameObject prefab = isSafe ? safeMapPrefab : GetRandomPrefab();
        if (prefab == null)
            return false;
        Quaternion rotation = Quaternion.identity;
        if (!isSafe)
        {
            int rotationY = Random.Range(0, 2) == 0 ? 0 : 180;
            rotation = Quaternion.Euler(0f, rotationY, 0f);
        }
        GameObject map = Instantiate(prefab, new Vector3(0f, 0f, spawnZ), rotation, transform);
        spawnedMaps.Enqueue(map);
        farthestMap = map;
        mapCount++;
        return true;
    }

    GameObject GetRandomPrefab()
    {
        if (mapPrefabs == null || mapPrefabs.Length == 0)
            return null;
        return mapPrefabs[Random.Range(0, mapPrefabs.Length)];
    }

    void RemoveMapsBehind()
    {
        float deleteZ = player.position.z - keepBehindDistance;
        while (spawnedMaps.Count > 0)
        {
            GameObject map = spawnedMaps.Peek();
            if (map == null)
            {
                spawnedMaps.Dequeue();
                continue;
            }
            float frontEdge = map.transform.position.z + mapLength * 0.5f;
            if (frontEdge >= deleteZ)
                break;
            if (map == farthestMap)
                farthestMap = null;
            Destroy(map);
            spawnedMaps.Dequeue();
        }
    }
}