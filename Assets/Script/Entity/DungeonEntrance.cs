using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
    [SerializeField] private int dungeonRoomIndex = 1;  // 던전 방 번호 (1, 2, 3)

    private bool hasPassedOnce = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (!hasPassedOnce)
        {
            hasPassedOnce = true;
            GameManager.Instance.StartDungeonRoomWave(dungeonRoomIndex); // 방 번호를 전달하여 웨이브 시작
        }
        else
        {
            hasPassedOnce = false;
            GameManager.Instance.ExitDungeonRoomWave();  // 이전 방의 웨이브 종료
        }
    }
}