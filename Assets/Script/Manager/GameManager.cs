using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 게임 전체를 관리하는 메인 매니저 클래스
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public PlayerController player { get; private set; }    // 플레이어 컨트롤러 (읽기 전용 프로퍼티)
    private ResourceController _playerResourceController;

    [SerializeField] private int currentWaveIndex = 0;      // 현재 웨이브 번호
    public int currentDungeonDifficulty = 1;
    public int enemiesPerWave = 3;

    private UIManager uiManager;
    public static bool isFirstLoading = true;
    private bool inDungeon = false;

    [SerializeField] private ChestController chestController;   //ChestController 참조
    [SerializeField] private EnemyManager[] enemyManagers;      // 방 번호에 따라 매핑 (index 0 = Room 1)

    private int currentRoom = -1;

    public void EnterDungeonRoom(int roomNumber)
    {
        // 다른 방으로 이동할 경우 기존 웨이브는 취소 처리
        if (currentRoom != roomNumber)
        {
            Debug.Log($"Entering Room {roomNumber}");
            currentRoom = roomNumber;

            // 각 EnemyManager에서 웨이브 시작
            for (int i = 0; i < enemyManagers.Length; i++)
            {
                if (enemyManagers[i] != null)
                {
                    if (i == roomNumber - 1)
                    {
                        enemyManagers[i].StartCoroutine(WaveRoutine()); // 해당 방에서 웨이브 시작
                    }
                    else
                    {
                        enemyManagers[i].StopWave(); // 다른 방의 웨이브는 중지
                    }
                }
            }
        }
    }

    private void Awake()
    {

            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 유지 (필요시)


        // 플레이어 찾고 초기화
        player = FindObjectOfType<PlayerController>();
        player.Init(this);

        // UI 매니저 참조 획득
        uiManager = FindObjectOfType<UIManager>();

        // 플레이어의 체력 리소스 컨트롤러 설정
        _playerResourceController = player.GetComponent<ResourceController>();

        // 체력 변경 이벤트를 UI에 연결
        _playerResourceController.RemoveHealthChangeEvent(uiManager.ChangePlayerHP);
        _playerResourceController.AddHealthChangeEvent(uiManager.ChangePlayerHP);
    }

    private void Start()
    {
        foreach (EnemyManager manager in enemyManagers)
        {
            manager.Init(this);
        }
    }

    // 던전 방의 웨이브를 시작하는 메서드
    public void StartDungeonRoomWave(int roomIndex)
    {
        // 이전 방의 웨이브 종료
        foreach (EnemyManager manager in enemyManagers)
        {
            manager.StopWave();
        }

        // 각 던전 방마다 다른 설정
        int difficulty = 1;
        int enemyCount = 3;

        switch (roomIndex)
        {
            case 1:
                difficulty = 1;
                enemyCount = 3;  // 방 1의 난이도 및 적 수
                break;
            case 2:
                difficulty = 2;
                enemyCount = 5;  // 방 2의 난이도 및 적 수
                break;
            case 3:
                difficulty = 3;
                enemyCount = 7;  // 방 3의 난이도 및 적 수
                break;
        }

        // 해당 방의 적들을 스폰
        enemyManagers[roomIndex - 1].StartWaves(difficulty, enemyCount);  // 새로 시작하는 웨이브
    }

    public void StartWaves(int difficulty, int enemyCount)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(SpawnWaveWithDifficulty(difficulty, enemyCount));
    }

    // 던전 방에서 나갈 때 웨이브 종료 처리
    public void ExitDungeonRoomWave()
    {
        foreach (EnemyManager manager in enemyManagers)
        {
            manager.StopWave();
        }
        EndOfWave(); // 웨이브 중단 처리
    }

    // 웨이브가 끝난 후 호출되는 메서드
    public void EndOfWave()
    {
        Debug.Log("웨이브 종료");
    }

    // 플레이어가 죽었을 때 게임 오버 처리
    public void GameOver()
    {
        foreach (EnemyManager manager in enemyManagers)
        {
            manager.StopWave();    // 모든 EnemyManager에서 웨이브 중지
        }
        uiManager.SetGameOver();
    }
}