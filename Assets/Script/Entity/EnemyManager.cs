using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using Random = UnityEngine.Random;



public class EnemyManager : MonoBehaviour
{
    GameManager gameManager;

    [Header("Enemy Settings")]
    [SerializeField] private List<EnemyData> enemyDataList;         // 각 적의 데이터 (난이도 포함)
    [SerializeField] private List<Rect> spawnAreas;                 // 적을 생성할 영역 리스트
    [SerializeField] private float timeBetweenSpawns = 0.2f;        // 개별 적 생성 간 간격
    [SerializeField] private float timeBetweenWaves = 1f;           // 웨이브 간 대기 시간
    [SerializeField] private GameObject chestPrefab;
    [SerializeField] private GameObject[] enemyPrefabs; // 다양한 적 종류
    [SerializeField] private float chestSpawnInterval = 5f;
    [SerializeField] private int waveCount = 3;                     // 기본 웨이브 수


    private float chestSpawnTimer = 0f;


    [Header("Gizmo")]
    [SerializeField] private Color gizmoColor = new Color(1, 0, 0, 0.3f); // 기즈모 색상

    private List<EnemyController> activeEnemies = new List<EnemyController>(); // 현재 활성화된 적들
    private Coroutine waveRoutine;          // 현재 실행 중인 웨이브 코루틴
    private Coroutine waveCoroutine;

    private bool enemySpawnComplete;        // 현재 웨이브 스폰이 완료되었는지 여부
    private bool isWaveRunning;             
    private bool isWaveInProgress = false; // 웨이브 진행 중인지 여부
    private int currentWave = 0;            // 현재 진행 중인 웨이브
    private int maxWaveCount = 30;          // 최대 웨이브 수 (30번 웨이브)
    private int currentDifficulty = 1;
    private int enemiesPerWave = 3;


    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
    }
    // 각 던전 방에 맞는 웨이브를 시작하는 메서드
    // 웨이브 시작
    public void StartWave()
    {
        if (currentWave > maxWaveCount)
        {
            // 웨이브가 10번을 초과하면 종료
            gameManager.EndOfWave();
            return;
        }

        // 기존 웨이브가 진행 중이면 중단
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        // 새 웨이브 시작
        waveRoutine = StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        yield return StartCoroutine(SpawnWaveWithDifficulty(currentDifficulty + currentWave / 2, enemiesPerWave + currentWave));
        currentWave++;
    }

    // 웨이브가 진행 중일 때 더 강한 몬스터를 스폰하는 메서드
    private void SpawnEnemies(int dungeonRoomIndex)
    {
        // 예시로 던전 방에 따라 다른 수의 적을 스폰
        int numberOfEnemies = dungeonRoomIndex * 3; // 던전 1 = 3, 던전 2 = 6, 던전 3 = 9

        for (int i = 0; i < numberOfEnemies; i++)
        {
            // 적을 생성하는 로직
            // 적을 스폰할 수 있는 코드 작성 (이 부분은 게임 로직에 맞춰서 작성)

            // 예시: enemies.Add(SpawnEnemy(i));  (실제 적 스폰 로직을 구현)
        }
        // 웨이브 종료 조건
        EndWave();
    }


    // 웨이브 종료 처리
    public void StopWave()
    {
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        // 현재 방에 남은 적 제거
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }


    private IEnumerator WaveRoutine()
    {
        while (currentWave < maxWaveCount && isWaveRunning)
        {
            int difficulty = currentDifficulty + currentWave / 2; // 웨이브마다 난이도 증가
            int count = enemiesPerWave + currentWave;             // 웨이브마다 적 수 증가

            yield return StartCoroutine(SpawnWaveWithDifficulty(difficulty, count));
            currentWave++;
            yield return new WaitForSeconds(timeBetweenWaves);      // 다음 웨이브 대기
        }

        if (isWaveRunning)
        {
            Debug.Log("웨이브 완료");
            GameManager.Instance.EndOfWave(); // 웨이브 종료
        }
    }

    // 웨이브 단위 적 생성
    private IEnumerator SpawnWaveWithDifficulty(int difficulty, int enemyCount)
    {
        //enemySpawnComplete = false;

        for (int i = 0; i < enemyCount; i++)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnEnemyByDifficulty(difficulty);
        }

        //enemySpawnComplete = true;
    }

    // 난이도에 맞는 적 생성
    private void SpawnEnemyByDifficulty(int difficulty)
    {
        if (enemyDataList.Count == 0 || spawnAreas.Count == 0)
        {
            Debug.LogWarning("EnemyDataList 또는 Spawn Areas가 비어 있습니다.");
            return;
        }

        // 난이도 이하의 적들만 필터링
        List<EnemyData> filteredEnemies = enemyDataList.FindAll(e => e.difficulty <= difficulty);

        if (filteredEnemies.Count == 0)
        {
            Debug.LogWarning("해당 난이도 이하의 적이 없습니다.");
            return;
        }

        EnemyData selectedData = filteredEnemies[Random.Range(0, filteredEnemies.Count)];
        GameObject selectedPrefab = selectedData.prefab;

        Rect area = spawnAreas[Random.Range(0, spawnAreas.Count)];
        Vector2 position = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax)
        );

        GameObject enemy = Instantiate(selectedData.prefab, position, Quaternion.identity);
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.Init(this, gameManager.player.transform);

        activeEnemies.Add(ec);
    }

    // 적 사망 시 호출
    public void RemoveEnemyOnDeath(EnemyController enemy)
    {
        activeEnemies.Remove(enemy);

        // 적 전부 제거되고, 스폰이 끝났다면 자동으로 다음 웨이브 준비 가능
        if (enemySpawnComplete && activeEnemies.Count == 0 && isWaveRunning)
        {
            // 다음 웨이브는 WaveRoutine 코루틴에서 자동 진행됨
        }
    }

    public void OnExitDungeon()
    {
        StopWave();
        gameManager.EndOfWave(); // 웨이브 중단 처리
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnAreas == null) return;
        Gizmos.color = gizmoColor;

        foreach (var area in spawnAreas)
        {
            Vector3 center = new(area.x + area.width / 2, area.y + area.height / 2);
            Vector3 size = new(area.width, area.height);
            Gizmos.DrawCube(center, size);
        }
    }

    private void Update()
    {
        if (!isWaveRunning) return;

        chestSpawnTimer += Time.deltaTime;
        if (chestSpawnTimer >= chestSpawnInterval)
        {
            SpawnChest();
            chestSpawnTimer = 0f;
        }
    }

    private void SpawnChest()
    {
        if (spawnAreas.Count == 0 || chestPrefab == null) return;

        Rect area = spawnAreas[Random.Range(0, spawnAreas.Count)];
        Vector2 pos = new Vector2(
            Random.Range(area.xMin, area.xMax),
            Random.Range(area.yMin, area.yMax)
        );
        Instantiate(chestPrefab, pos, Quaternion.identity);
    }
}