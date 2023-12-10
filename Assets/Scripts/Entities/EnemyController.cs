using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyController : Entity {

    [Tooltip("적이 현재 장착 중인 무기")]
    public WeaponController weaponController;

    [Tooltip("공격할 대상 레이어")]
    public LayerMask targetLayer;

    [Tooltip("플레이어 감지 범위")]
    public float playerDetectDistance;

    [Tooltip("경로 재탐색 대기시간")]
    public float pathFindDelayTime = 0.25f;

    [Tooltip("적 HP 슬라이더")]
    public Slider healthBar;

    // 경로 재탐색 대기시간
    float time;

    NavMeshAgent navMeshAgent;
    Animator animator;

    Entity target;

    // Start is called before the first frame update
    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        weaponController.owner = this;
        weaponController.targetLayer = targetLayer;

        navMeshAgent.isStopped = false;

        target = FindObjectOfType<SanctuaryController>();
    }

    // Update is called once per frame
    void Update() {
        // 게임 오버되면 게임 오버 메소드로
        if (GameManager.instance.GameStatus == false) {
            OnGameOver();
            return;
        }

        if (isDeath) return;

        FindTarget();
        AttackTarget();
    }

    void FindTarget() {
        // Step 1. target이 플레이어인 경우, 플레이어 거리가 멀면 타게팅 해제
        if (target != null && target.gameObject.CompareTag("Player")) {
            var playerDistance = Vector3.Distance(transform.position, target.transform.position);
            if (playerDistance > playerDetectDistance) {
                target = null; // 플레이어가 멀면 target을 재설정
            }
        }

        // Step 2. target이 없거나 죽었다면, Sanctuary를 target으로 설정
        if (target == null || target.gameObject.activeSelf == false) {
            target = FindObjectOfType<SanctuaryController>();
        }
    }

    void AttackTarget() {
        var targetPosition = target.GetComponent<Collider>().ClosestPoint(transform.position);
        var distance = Vector3.Distance(transform.position, targetPosition);
        if (distance < weaponController.attackDistance) {
            animator.SetBool("HasTarget", false);
            navMeshAgent.isStopped = true;

            var dir = targetPosition - transform.position;
            var rotation = Quaternion.LookRotation(dir).eulerAngles;
            rotation.x = 0;
            rotation.z = 0;
            transform.rotation = Quaternion.Euler(rotation);

            Attack();
        } else {
            animator.SetBool("HasTarget", true);

            navMeshAgent.isStopped = false;
            time += Time.deltaTime;

            if (time > pathFindDelayTime) {
                time = 0f;
                navMeshAgent.SetDestination(targetPosition);
            }

            if (navMeshAgent.isOnOffMeshLink) {
                // 지면으로 순간이동
                navMeshAgent.CompleteOffMeshLink();
            }
        }
    }

    public void Attack() {
        if (weaponController.clip == 0) {
            weaponController.Reload();
        } else {
            animator.SetTrigger("Attack");
            weaponController.Fire();
        }
    }


    public override void OnHit(int damage, bool isCritical, Entity attacker) {
        base.OnHit(damage, isCritical, attacker);
        animator.SetTrigger("Hit");

        if (attacker.priority > target.priority) {
            target = attacker;
        }

        healthBar.value = health / (float)maxHealth;
    }

    public override void Death() {
        isDeath = true;
        navMeshAgent.isStopped = true;
        animator.SetTrigger("Death");

        // 콜라이더 비활성화
        GetComponent<Collider>().enabled = false;

        WaveManager.instance.EnemyDeath();
        Destroy(gameObject, 1.2f);
    }

    public void OnGameOver() {
        navMeshAgent.isStopped = true;
        this.enabled = false;
    }
}
