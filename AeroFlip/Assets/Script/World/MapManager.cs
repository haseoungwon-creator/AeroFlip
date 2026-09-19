using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] MapPool mapPool;
    [SerializeField] Transform player;
    [SerializeField] float mapLength = 80f;
    [SerializeField] int forwardMapCount = 6;
    [SerializeField] int safeMapCount = 3;
    [SerializeField] float worldSpeed = 30f;
    [SerializeField] float keepBegindDistance = 250f;

    private readonly Queue<SpawnedMap> spawnedMaps = new Queue<SpawnedMap>();
    private GameObject farthestMap;
    int mapCount;

    private struct SpawnedMap
    {
        public GameObject map;
        public int mapIndex;
        public bool isSafe;

        public SpawnedMap(GameObject map,int mapIndex,bool isSafe)
        {
            this.map = map;
            this.mapIndex = mapIndex;
            this.isSafe = isSafe;
        }
    }

    private void Update()
    {
        SpawnMapsAhead();
        RemoveMapsBehind();
    }


    private void SpawnMapsAhead()
    {
        float requiredZ = player.position.z + mapLength * forwardMapCount;
        float forthestZ = GerFarthestZ();
        int guard = 0;

        while (forthestZ < requiredZ)
        {
            float spawnZ = forthestZ + mapLength;
            if (!SpawnMap(spawnZ))
                break;

            forthestZ = spawnZ;

            if (++guard > 2000)
                break;
        }
    }

    private float GerFarthestZ()
    {
        return farthestMap != null ? farthestMap.transform.position.z : player.position.z - mapLength;
    }

    private bool SpawnMap(float spawnZ)
    {
        bool isSafe = mapCount < safeMapCount;
        int mapIndex = isSafe ? -1 : Random.Range(0, mapPool.MapPrefabsCount);
        GameObject map = isSafe ? mapPool.GetSafeMap() : mapPool.GetMap(mapIndex);

        if(map == null)
            return false;

        Quaternion rotation = Quaternion.identity;

        if (!isSafe)
        {
            int rotationY = Random.Range(0, 2) == 0 ? 0 : 180;
            rotation = Quaternion.Euler(0f,rotationY, 0f);
        }

        map.transform.SetPositionAndRotation(new Vector3(0f, 0f, spawnZ), rotation);

        spawnedMaps.Enqueue(new SpawnedMap(map,mapIndex, isSafe));
        farthestMap = map;
        mapCount++;

        return true;
    }

    private void RemoveMapsBehind()
    {
        float deleteZ = player.position.z - keepBegindDistance;

        while(spawnedMaps.Count > 0)
        {
            SpawnedMap spawnedMap = spawnedMaps.Peek();
            GameObject map = spawnedMap.map;

            if(map == null)
            {
                spawnedMaps.Dequeue();
                continue;
            }

            float frontEdge = map.transform.position.z + mapLength * 0.5f;

            if (frontEdge >= deleteZ)
                break;

            if (map == farthestMap)
                farthestMap = null;

            if (spawnedMap.isSafe)
                mapPool.ReturnSafeMap(map);

            else
                mapPool.ReturnMap(spawnedMap.mapIndex,map);

            spawnedMaps.Dequeue();
        }
    }
}
