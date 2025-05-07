using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // 카메라가 따라갈 대상
    float offsetX; // 카메라와 대상 사이의 X축 오프셋
    float offsetY; // 카메라와 대상 사이의 Y축 오프셋

    void Start()
    {
        // target이 지정되지 않으면, 카메라가 아무것도 따라가지 않도록 함
        if (target == null)
            return;

        // 카메라의 현재 위치와 대상의 X 위치 차이를 오프셋으로 저장
        offsetX = transform.position.x - target.position.x;
        offsetY = transform.position.y - target.position.y;
    }

    void Update()
    {
        // target이 null이라면 카메라가 아무것도 따라가지 않음
        if (target == null)
            return;

        // 현재 카메라 위치를 저장
        Vector3 pos = transform.position;
        // 카메라의 X 위치를 대상의 X 위치에 오프셋을 더해 설정
        pos.x = target.position.x + offsetX;
        pos.y = target.position.y + offsetY;
        // 계산된 새로운 위치로 카메라 이동
        transform.position = pos;
    }
}
