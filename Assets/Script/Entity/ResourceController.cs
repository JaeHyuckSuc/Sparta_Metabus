using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private float healthChangeDelay = .5f;     // 피해 후 무적 지속 시간

    private BaseController baseController;
    private StatHandler statHandler;
    private AnimationHandler animationHandler;

    private float timeSinceLastChange = float.MaxValue;         // 마지막 체력 변경 이후 경과 시간

    public float CurrentHealth { get; private set; }            // 현재 체력 (외부 접근만 허용)
    public float MaxHealth => statHandler.Health;               // 최대 체력은 StatHandler로부터 가져옴

    public AudioClip damageClip;

    // 체력이 바뀔 때 호출되는 이벤트 (현재 체력, 최대 체력 전달)
    private Action<float, float> OnChangeHealth;

    private void Awake()
    {
        statHandler = GetComponent<StatHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        baseController = GetComponent<BaseController>();
    }

    private void Start()
    {
        CurrentHealth = statHandler.Health;
    }

    private void Update()
    {
        // 아직 무적 상태라면 시간 누적
        if (timeSinceLastChange < healthChangeDelay)
        {
            // 무적 시간 종료 시 애니메이션에도 알림
            timeSinceLastChange += Time.deltaTime;
            if (timeSinceLastChange >= healthChangeDelay)
            {
                animationHandler.InvincibilityEnd();
            }
        }
    }

    // 체력 변경 함수 (피해 or 회복)
    public bool ChangeHealth(float change)
    {
        // 변화 없거나 무적 상태면 무시
        if (change == 0 || timeSinceLastChange < healthChangeDelay)
        {
            return false;
        }

        timeSinceLastChange = 0f;       // 다시 무적 시작

        // 체력 적용
        CurrentHealth += change;
        CurrentHealth = CurrentHealth > MaxHealth ? MaxHealth : CurrentHealth;
        CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;

        // 체력 변경 이벤트 호출 (UI 등에서 이 값을 수신해 처리함)
        OnChangeHealth?.Invoke(CurrentHealth, MaxHealth);

        // 데미지일 경우 (음수)
        if (change < 0)
        {
            animationHandler.Damage();      // 맞는 애니메이션 실행

            // 사운드가 설정되어 있을 경우 재생
            if (damageClip != null)
                SoundManager.PlayClip(damageClip);

        }

        // 체력이 0 이하가 되면 사망 처리
        if (CurrentHealth <= 0f)
        {
            Death();
        }

        return true;
    }

    // 외부에서 체력 변경 이벤트를 등록하는 함수
    public void AddHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth += action;
    }

    // 외부에서 체력 변경 이벤트를 제거하는 함수
    public void RemoveHealthChangeEvent(Action<float, float> action)
    {
        OnChangeHealth -= action;
    }

    private void Death()
    {
        baseController.Death();
    }

}