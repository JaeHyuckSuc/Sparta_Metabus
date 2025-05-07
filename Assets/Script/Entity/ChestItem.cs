using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//상자 충돌 처리
public class ChestItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 점수 1점 추가
            if (PlayerScore.Instance != null)
            {
                PlayerScore.Instance.AddScore(1);
            }
            else
            {
                Debug.LogWarning("PlayerScore.Instance가 존재하지 않습니다.");
            }

            // 상자 제거
            Destroy(gameObject);
        }
    }
}
