using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 캐릭터의 기본 능력치를 저장하고 관리하는 클래스
public class StatHandler : MonoBehaviour
{
    // 체력 (1 ~ 100 사이 값만 허용)
    [Range(1, 100)][SerializeField] private int health = 10;
    // 외부에서 접근 가능한 프로퍼티 (값 변경 시 자동으로 0~100 사이로 제한)
    public int Health
    {
        get => health;
        set => health = Mathf.Clamp(value, 0, 100);
    }
    // 이동 속도 (1f ~ 20f 사이 값만 허용)
    [Range(1f, 20f)][SerializeField] private float speed = 3;
    // 외부에서 접근 가능한 프로퍼티 (값 변경 시 0~20f로 제한)
    public float Speed
    {
        get => speed;
        set => speed = Mathf.Clamp(value, 0, 20);
    }
}
