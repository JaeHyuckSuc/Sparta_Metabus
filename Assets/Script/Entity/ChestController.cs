using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChestController : MonoBehaviour
{
    [SerializeField]
    private GameObject chestPrefab;

    [SerializeField]
    private List<Rect> spawnAreas;

    [SerializeField]
    private Color gizmoColor = new Color(1f, 1f, 0f, 0.3f);

    private List<GameObject> activeChests = new List<GameObject>();

    [SerializeField]
    private float timeBetweenSpawns = 0.5f;

    [SerializeField]
    private int chestsPerWave = 3;

    private bool isSpawning = false;

    private void Start()
    {
        // 자동 스폰 제거
    }

    // 외부에서 이 메서드를 호출해 웨이브 시작 시 Chest 스폰 시작
    public void StartWaveSpawn()
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnWave(chestsPerWave));
        }
    }

    private IEnumerator SpawnWave(int chestCount)
    {
        isSpawning = true;

        for (int i = 0; i < chestCount; i++)
        {
            SpawnRandomChest();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }

        isSpawning = false;
    }

    private void SpawnRandomChest()
    {
        if (chestPrefab == null || spawnAreas.Count == 0)
        {
            Debug.LogWarning("ChestPrefab 또는 SpawnAreas가 설정되지 않았습니다.");
            return;
        }

        Rect area = spawnAreas[Random.Range(0, spawnAreas.Count)];

        Vector2 spawnPos = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax)
        );

        GameObject chest = Instantiate(chestPrefab, spawnPos, Quaternion.identity);
        activeChests.Add(chest);
    }

    public void RemoveChest(GameObject chest)
    {
        if (activeChests.Contains(chest))
            activeChests.Remove(chest);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;

        Gizmos.color = gizmoColor;
        foreach (var area in spawnAreas)
        {
            Vector3 center = new Vector3(area.x + area.width / 2f, area.y + area.height / 2f);
            Vector3 size = new Vector3(area.width, area.height, 0);
            Gizmos.DrawCube(center, size);
        }
    }
}