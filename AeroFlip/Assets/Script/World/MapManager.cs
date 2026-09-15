using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 프로젝트 구조: 플레이어는 (거의) 제자리에 있고, 맵이 WorldMovement에 의해
/// 스스로 Vector3.back 방향으로 계속 움직여서 플레이어 쪽으로 흘러온다.
///
/// 이게 중요한 이유: 한 번 스폰된 맵도 시간이 지나면서 계속 위치가 바뀐다.
/// 그래서 "가장 앞쪽 맵의 위치"를 변수에 캐싱해두고 재사용하면 안 되고,
/// 매 프레임 실제 살아있는 맵들의 "현재" 위치를 다시 조회해야 한다.
/// (캐싱했을 때 생기는 문제: 실제로는 맵들이 계속 뒤로 밀려나는데 매니저는
///  "아직 앞에 충분히 있다"고 착각해서 새 맵을 더 이상 스폰하지 않게 됨)
/// </summary>
public class MapManager : MonoBehaviour
{
    [Header("프리팹")]
    [SerializeField] private GameObject safeMapPrefab;
    [SerializeField] private GameObject[] mapPrefabs;

    [Header("플레이어 (기준점 - 보통 제자리에 고정됨)")]
    [SerializeField] private Transform player;

    [Header("맵 설정")]
    [Tooltip("맵 프리팹 하나의 Z축 길이 (Plane 스케일 80,1,80 -> 80)")]
    [SerializeField] private float mapLength = 80f;
    [Tooltip("플레이어 앞쪽에 항상 유지할 맵 개수")]
    [SerializeField] private int forwardMapCount = 6;
    [Tooltip("게임 시작 시 깔아줄 안전 맵 개수 (가장 가까운 것부터)")]
    [SerializeField] private int safeMapCount = 3;

    [Header("월드 이동 속도 (WorldMovement의 worldSpeed와 같은 값)")]
    [SerializeField] private float worldSpeed = 30f;
    [Tooltip("플레이어보다 이 거리 이상 뒤로 지나간 맵을 제거한다. " +
             "되감기 스킬을 쓸 거라면 (worldSpeed * 되감기초 + 맵길이) 이상으로 넉넉히 잡을 것")]
    [SerializeField] private float keepBehindDistance = 250f;

    private readonly List<GameObject> spawnedMaps = new List<GameObject>();
    private int mapCount;
    private bool initialized;

    private void Update()
    {
        // 다른 스크립트의 Start()보다 먼저/나중에 실행되는 순서 문제를 피하려고
        // 첫 Update()에서 초기화한다 (모든 오브젝트의 Start()는 Update()보다 항상 먼저 끝남).
        if (!initialized)
        {
            SpawnMapsAhead();
            initialized = true;
        }

        SpawnMapsAhead();
        RemoveMapsBehind();
    }

    private void SpawnMapsAhead()
    {
        float requiredZ = player.position.z + mapLength * forwardMapCount;
        float farthestZ = GetFarthestMapZ();

        int guard = 0;
        while (farthestZ < requiredZ)
        {
            float spawnZ = farthestZ + mapLength;

            if (!SpawnMap(spawnZ))
                break; // 프리팹이 비어있는 등 실패 - 다음 프레임에 다시 시도

            farthestZ = spawnZ;

            if (++guard > 2000)
            {
                Debug.LogWarning("[MapManager] SpawnMapsAhead 안전 한도 도달 - 설정값을 확인하세요.");
                break;
            }
        }
    }

    /// <summary>
    /// 현재 살아있는 맵들 중 가장 앞쪽(Z가 가장 큰) 위치를 실시간으로 조회한다.
    /// 맵들이 계속 스스로 움직이기 때문에 절대 캐싱하지 않고 매번 다시 계산한다.
    /// </summary>
    private float GetFarthestMapZ()
    {
        float farthest = float.MinValue;

        for (int i = 0; i < spawnedMaps.Count; i++)
        {
            if (spawnedMaps[i] == null)
                continue;

            float z = spawnedMaps[i].transform.position.z;
            if (z > farthest)
                farthest = z;
        }

        // 살아있는 맵이 하나도 없으면 플레이어 위치부터 채우기 시작한다.
        return farthest == float.MinValue ? player.position.z - mapLength : farthest;
    }

    private bool SpawnMap(float spawnZ)
    {
        bool isSafe = mapCount < safeMapCount;
        GameObject prefab = isSafe ? safeMapPrefab : GetRandomPrefab();

        if (prefab == null)
        {
            Debug.LogWarning(isSafe
                ? "[MapManager] safeMapPrefab이 비어 있습니다. Inspector를 확인하세요."
                : "[MapManager] mapPrefabs 배열이 비어 있거나 요소가 없습니다. Inspector를 확인하세요.");
            return false;
        }

        GameObject map = Instantiate(
            prefab,
            new Vector3(0f, 0f, spawnZ),
            Quaternion.identity,
            transform
        );

        spawnedMaps.Add(map);
        mapCount++;

        return true;
    }

    private GameObject GetRandomPrefab()
    {
        if (mapPrefabs == null || mapPrefabs.Length == 0)
            return null;

        return mapPrefabs[Random.Range(0, mapPrefabs.Length)];
    }

    private void RemoveMapsBehind()
    {
        float deleteZ = player.position.z - keepBehindDistance;

        for (int i = spawnedMaps.Count - 1; i >= 0; i--)
        {
            GameObject map = spawnedMaps[i];

            if (map == null)
            {
                spawnedMaps.RemoveAt(i);
                continue;
            }

            // 맵의 앞쪽 끝(플레이어 방향 가장자리)까지 완전히 지나간 경우에만 제거.
            float frontEdge = map.transform.position.z + mapLength * 0.5f;
            if (frontEdge < deleteZ)
            {
                Destroy(map);
                spawnedMaps.RemoveAt(i);
            }
        }
    }
}