using System;
using System.Collections;
using System.Collections.Generic;
using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

public class MeleeAttackController : Projectile {

    [Tooltip("근접 공격의 범위")]
    public Vector3 meleeRange;

    // Start is called before the first frame update
    void Start() {
        var dir = Quaternion.FromToRotation(Vector3.forward, direction);
        Collider[] colliders = Physics.OverlapBox(transform.position, meleeRange, dir);

        // new() == new List<GameObject>()
        List<GameObject> hitObjects = new();

        foreach (var collider in colliders) {
            var obj = collider.gameObject;

            if (hitObjects.Contains(obj)) continue;

            hitObjects.Add(obj);
        }

        foreach (var obj in hitObjects) {
            CheckCollision(obj);
        }

        Destroy(gameObject);
    }
}
