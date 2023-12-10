using System;
using System.Collections;
using System.Collections.Generic;
using LlockhamIndustries.ExtensionMethods;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Tooltip("발사체의 방향")]
    public Vector3 direction;

    [Tooltip("발사체의 지속 시간")]
    public float lifeTime = 5f;

    [Tooltip("공격할 대상 레이어")]
    public LayerMask targetLayer;

    // 발사체의 부모 Weapon
    protected WeaponController weapon;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    protected void Update() {
        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0f) {
            Destroy(gameObject);
        }
    }

    // 총알이 발사된 부모 Weapon을 지정
    public void SetParentWeapon(WeaponController controller) {
        weapon = controller;
    }

    // 충돌 체크
    protected void CheckCollision(GameObject obj) {
        var entity = obj.GetComponent<Entity>();

        // 공격에 맞은 오브젝트가 Entity가 아님
        if (entity == null) return;

        // 공격 대상 Layer에 속하지 않음
        if (targetLayer.Contains(obj.layer) == false) return;

        // 부모 Weapon에서 맞은 오브젝트를 parameter로 OnHit 함수 호출
        weapon.OnHit(entity);
    }
}
