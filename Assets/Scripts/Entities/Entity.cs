using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    [Header("Stats")]
    [Tooltip("엔티티의 최대 체력")]
    public int maxHealth;

    [Tooltip("엔티티의 현재 체력")]
    public int health;

    // 방어력은 엔티티가 대미지를 얼마나 견디는지에 대한 수치입니다.
    // 방어력은 0.0 ~ 1.0의 값을 가지며 0에 가까울 수록 대미지를 강하게 버텨냅니다.
    // 방어력은 단순히 최종 대미지에 곱해져서 적용됩니다.
    // (방어력 0.5인 엔티티에게 100의 대미지를 입히면 50만큼만 들어갑니다.)
    [Tooltip("엔티티의 방어력")]
    public float defense;

    // Pylon 및 Enemy가 사용하는 타게팅 우선순위입니다.
    // 높을 수록 우선순위가 높습니다.
    [Tooltip("엔티티의 AI 타게팅 우선순위")]
    public int priority;


    // 게임 오브젝트가 사망 처리를 완료했는지를 나타냅니다.
    protected bool isDeath = false;

    virtual public void OnHit(int damage, bool isCritical, Entity attacker) {
        if (isDeath) return; // 사망한 엔티티인경우, skip

        int finalDamage = damage;

        // 방어력 반영
        finalDamage = Mathf.CeilToInt(finalDamage * defense);

        health -= finalDamage;

        if (health <= 0) {
            Death();
        }
    }

    virtual public void Death() {
        isDeath = true;
        Destroy(gameObject);
    }

}
