using System;
using UnityEngine;

public class PylonSpawnController : WeaponController {

    [Tooltip("수정체 y 위치")]
    public float yPos;

    [Tooltip("에너지 수정체 모형 GameObject")]
    public GameObject pylonEnergyObject;

    [Tooltip("레이저 수정체 모형 GameObject")]
    public GameObject pylonLaserObject;

    [Tooltip("수정체와 충돌하는 대상 레이어")]
    public LayerMask pylonLayer;

    [Tooltip("에너지 수정체 Prefab")]
    public GameObject pylonEnergyPrefab;

    [Tooltip("레이저 수정체 Prefab")]
    public GameObject pylonLaserPrefab;

    bool isLaser = false;

    // 현재 무기가 가리키는 수정체 위치
    Vector3 targetPosition;

    public bool isSpawnable {
        get {
            return targetPosition != Vector3.zero && CheckCollision() == false;
        }
    }

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
        pylonLaserObject.SetActive(false);
        pylonEnergyObject.SetActive(false);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        CheckPosition();

        pylonLaserObject.SetActive(false);
        pylonEnergyObject.SetActive(false);

        if (isLaser) {
            pylonLaserObject.SetActive(isSpawnable);
        } else {
            pylonEnergyObject.SetActive(isSpawnable);
        }

        UpdateUI();
    }

    void UpdateUI() {
        UIManager.instance.ShowReloading = isReloading;

        if (isReloading) {
            UIManager.instance.ReloadingGauge = coolDown / reloadTime;
            UIManager.instance.ReloadingTime = coolDown;
        }
    }

    // 무기가 가리키는 위치를 가져오기
    void CheckPosition() {
        // firePosition을 Hexagon 좌표계로 변환하고 targetPosition에 저장
        bool isHit = Physics.Raycast(firePosition.position, Vector3.down, out RaycastHit hit, 100f);
        if (isHit == false) {
            targetPosition = Vector3.zero;
            return;
        }

        var hitColliderTransform = hit.collider.transform;
        var parentTransform = hitColliderTransform.parent;

        targetPosition = parentTransform.position;
        targetPosition.y = yPos;

        pylonLaserObject.transform.position = targetPosition;
        pylonEnergyObject.transform.position = targetPosition;
        pylonLaserObject.transform.rotation = Quaternion.identity;
        pylonEnergyObject.transform.rotation = Quaternion.identity;
    }

    bool CheckCollision() {
        Collider[] colliders = Physics.OverlapSphere(targetPosition, 1f, pylonLayer);
        return colliders.Length > 0;
    }

    // 수정체 배치
    public override void Fire() {
        // 스폰 불가능한 위치면 return
        if (isSpawnable == false) return;

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

        var prefab = isLaser ? pylonLaserPrefab : pylonEnergyPrefab;
        Instantiate(prefab, targetPosition, Quaternion.identity);

        // 발사 소리 재생
        if (fireSound) {
            audioSource.Stop();
            audioSource.PlayOneShot(fireSound);
        }

        isReloading = true;
        coolDown = reloadTime;
    }

    // 재장전으로 수정체 종류 변경
    public override void Reload() {
        isLaser = !isLaser;
    }

    // 공격 없음
    public override void OnHit(Entity target) { }

    void OnEnable() {
        UIManager.instance.ShowAmmo = false;
    }

    void OnDisable() {
        UIManager.instance.ShowReloading = false;
    }
}
