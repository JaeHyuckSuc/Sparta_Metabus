using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustParticleControl : MonoBehaviour
{
    [SerializeField] private bool createDustOnWalk = true;          // 걷기 중 파티클 생성 여부
    [SerializeField] private ParticleSystem dustParticleSystem;     // 사용할 파티클 시스템

    public void CreateDustParticles()
    {
        if (createDustOnWalk)
        {
            dustParticleSystem.Stop();  // 이전 재생을 멈추고
            dustParticleSystem.Play();  // 새로 재생
        }
    }
}
