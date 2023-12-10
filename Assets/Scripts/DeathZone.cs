using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnCollisionEnter(Collision collider) {
        var entity = collider.gameObject.GetComponent<Entity>();

        if (entity) {
            entity.Death();
        }
    }
}
