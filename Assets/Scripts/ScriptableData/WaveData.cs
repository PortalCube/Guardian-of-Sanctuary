using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Enemy : IComparable<Enemy> {
    [Tooltip("생성할 적의 Prefab")]
    public GameObject entityPrefab;

    [Tooltip("생성할 Prefab의 마릿수")]
    public int count;

    [Tooltip("적을 생성할 Spawn Point")]
    public int spawnPoint;

    [Tooltip("Prefab을 생성할 시점")]
    public float time;

    public int CompareTo(Enemy other) {
        var cmp = time - other.time;

        if (cmp < 0) {
            return -1;
        } else if (cmp > 0) {
            return 1;
        } else {
            return 0;
        }
    }
}

// 좀비 생성시 사용할 셋업 데이터
[CreateAssetMenu(menuName = "Scriptable/WaveData", fileName = "Wave")]
public class WaveData : ScriptableObject {
    [Tooltip("Wave 이름")]
    public String name;

    [Tooltip("Wave에서 등장할 적들")]
    public Enemy[] enemies;
}
