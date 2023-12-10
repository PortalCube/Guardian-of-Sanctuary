using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHoleController : MonoBehaviour {

    float time = 0f;
    // Start is called before the first frame update
    void Start() {
        time = GameManager.instance.decalRemainTime;
    }

    // Update is called once per frame
    void Update() {
        time -= Time.deltaTime;

        // 일정 시간이 지나면 데칼을 제거
        if (time <= 0) {
            Destroy(gameObject);
        }
    }
}
