using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponController : MonoBehaviour {

    [Tooltip("무기의 기본 대미지")]
    public int baseDamage;

    [Tooltip("무기의 치명타 공격의 대미지 배수")]
    public float criticalMultiplier;

    [Tooltip("무기의 치명타 확률")]
    public float criticalChance;

    // 안정성은 무기가 고정된 대미지에서 조금 벗어난 대미지를 주도록 하는 장치입니다.
    // 안정성은 0.0 ~ 1.0의 값을 가지며 1.0의 경우에는 무기가 반드시 기본 대미지를 부여합니다. 낮을 수록 대미지가 불안정해집니다.
    // 자세한 구현은 아래의 OnHit 함수를 참고해주세요.
    [Tooltip("무기의 안정성")]
    public float stability;

    [Tooltip("무기의 탄창 크기")]
    public int maxClip;

    [Tooltip("무기의 발사 간격 시간")]
    public float firingDelayTime;

    [Tooltip("무기의 재장전 시간")]
    public float reloadTime;

    [Tooltip("무기의 사정 거리")]
    public float attackDistance;

    [Tooltip("Bullet이 발사될 지점의 Transform")]
    public Transform firePosition;

    [Tooltip("무기의 소유자 (공격자)")]
    public Entity owner;

    [Tooltip("공격할 대상 레이어")]
    public LayerMask targetLayer;

    [Tooltip("발사될 Bullet Prefab")]
    public GameObject bulletPrefab;

    [Tooltip("발사시 재생할 사운드")]
    public AudioClip fireSound;

    [Tooltip("재장전시 재생할 사운드")]
    public AudioClip reloadSound;

    public event Action OnReload;

    protected AudioSource audioSource;

    protected float coolDown;

    public int clip;

    protected bool isReloading = false;

    public Vector3 BulletDirection {
        get {
            return firePosition.position - transform.position;
        }
    }

    void Awake() {
        clip = maxClip;
    }

    protected virtual void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected virtual void Update() {
        UpdateCooldown();
    }

    // 쿨타임을 새로 고침
    void UpdateCooldown() {
        coolDown -= Time.deltaTime;

        if (coolDown < 0f) {

            // 만약 재장전 중이었다면 탄약 채우기
            if (isReloading) {
                RefillAmmo();
            }

            coolDown = 0f;
        }
    }

    // 무기의 방아쇠를 당겼을 때
    public virtual void Fire() {
        // 쿨타임 도는 중이면 return
        if (coolDown > 0) {
            return;
        }

        // 재장전하는 중이면 return
        if (isReloading) {
            return;
        }

        // 탄창이 없으면 return
        if (clip <= 0) {
            return;
        }

        // 공격체 (bullet)를 생성하고 방향과 부모 Weapon을 지정
        var bullet = Instantiate(bulletPrefab, firePosition.position, firePosition.rotation);
        var controller = bullet.GetComponent<Projectile>();

        controller.SetParentWeapon(this);
        controller.targetLayer = targetLayer;
        controller.direction = BulletDirection;

        // 발사 소리 재생
        if (fireSound) {
            audioSource.Stop();
            audioSource.PlayOneShot(fireSound);
        }

        // 탄창에서 총알 한개 삭제
        clip--;

        // 쿨타임 시작
        coolDown = firingDelayTime;
    }

    public virtual void Reload() {
        // 재장전 하는 중이면 return
        if (isReloading) {
            return;
        }

        // 재장전 소리 재생
        if (reloadSound) {
            audioSource.Stop();
            audioSource.PlayOneShot(reloadSound);
        }

        // 재장전 & 쿨타임 시작
        isReloading = true;
        coolDown = reloadTime;
    }

    public void RefillAmmo() {
        clip = maxClip; // 탄창 채우기
        isReloading = false; // 재장전 끝

        // null check를 위해서 ?. 연산자와 Invoke 메소드 활용
        OnReload?.Invoke();
    }

    // 공격체 (bullet)가 맞았을 때
    public virtual void OnHit(Entity target) {
        // 최종 대미지 계산
        int damage = baseDamage;
        bool isCritical = false;

        // Step 1. 치명타 계산
        var rand = Random.Range(0f, 1f);
        if (rand < criticalChance) {
            // 치명타가 발동되었다면 치명타 가산
            isCritical = true;
            damage = Mathf.CeilToInt(damage * criticalMultiplier);
        }

        // Step 2. 안정성 반영
        damage = Mathf.CeilToInt(damage * (1 + Random.Range(0, 1f - stability)));

        // Enemy에게 대미지 반영
        target.OnHit(damage, isCritical, owner);
    }
}
