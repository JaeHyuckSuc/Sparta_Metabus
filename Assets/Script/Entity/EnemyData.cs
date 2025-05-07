using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "TextRPG/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject prefab;
    [Range(1, 10)]
    public int difficulty; // 난이도 1~10 사이 지정
}
