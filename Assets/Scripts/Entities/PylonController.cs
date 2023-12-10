using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PylonController : Entity {

    [Tooltip("수정체가 현재 장착 중인 무기")]
    public WeaponController weaponController;

    [Tooltip("공격할 대상 레이어")]
    public LayerMask targetLayer;

    [Tooltip("관통 불가능한 레이어")]
    public LayerMask collisionLayer;

    [Tooltip("적 감지 범위")]
    public float enemyDetectRadius;

    [Tooltip("소환시 재생할 이펙트 Prefab")]
    public GameObject effectPrefab;

    public Entity target = null;

    // Start is called before the first frame update
    void Start() {
        weaponController.owner = this;
        weaponController.targetLayer = targetLayer;
        Instantiate(effectPrefab, transform);
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.GameStatus == false) return;

        FindTarget();
        Attack();
    }

    void FindTarget() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectRadius, targetLayer);

        var distance = enemyDetectRadius;

        if (target) {
            var targetDistance = Vector3.Distance(transform.position, target.transform.position);

            if (targetDistance > distance) {
                target = null;
            } else {
                distance = targetDistance;
            }
        }

        foreach (var collider in colliders) {
            // 콜라이더가 entity인지 확인
            var entity = collider.GetComponent<Entity>();
            if (entity == null) continue;

            // 현재 target보다 가까운 곳에 있는지 확인
            var newDistance = Vector3.Distance(transform.position, collider.transform.position);
            if (newDistance > distance) continue;

            // 중간에 가로막혀 있지 않은지 확인
            var direction = entity.transform.position - transform.position;
            var isHit = Physics.Raycast(transform.position, direction, out RaycastHit hit, 200f, collisionLayer);
            if (isHit == false || hit.collider != collider) continue;

            target = entity;
            distance = newDistance;
        }
    }

    public void Attack() {
        if (target == null) return;

        // weapon을 target 방향으로
        var dir = target.transform.position - weaponController.transform.position;
        var rotation = Quaternion.LookRotation(dir).eulerAngles;
        rotation.x = 0;
        rotation.z = 0;
        weaponController.transform.rotation = Quaternion.Euler(rotation);


        if (weaponController.clip == 0) {
            weaponController.Reload();
        } else {
            weaponController.Fire();
        }
    }

    public override void OnHit(int damage, bool isCritical, Entity attacker) {
        base.OnHit(damage, isCritical, attacker);
    }

    public override void Death() {
        base.Death();
    }
}
