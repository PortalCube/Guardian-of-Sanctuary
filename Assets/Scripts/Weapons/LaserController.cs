using UnityEngine;
using VolumetricLines;

public class LaserController : Projectile {

    [Tooltip("Level에 남길 탄흔 Prefab")]
    public GameObject decalPrefab;

    [Tooltip("탄흔 delay")]
    public float decalDelay;

    [Tooltip("관통 불가능한 레이어")]
    public LayerMask collisionLayer;

    [Tooltip("레이저 최대 길이")]
    public float distance;

    float decalTime;

    VolumetricLineBehavior volumetricLine;
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start() {
        volumetricLine = GetComponent<VolumetricLineBehavior>();
        meshRenderer = GetComponent<MeshRenderer>();
        CheckLaserCollision();
        DrawLaser();
    }

    // Update is called once per frame
    void Update() {
        base.Update();
        UpdateTransform();
        DrawLaser();

        decalTime -= Time.deltaTime;

        if (decalTime < 0f) {
            decalTime = 0f;
        }
    }

    void UpdateTransform() {
        transform.position = weapon.firePosition.position;
        transform.rotation = weapon.firePosition.rotation;
        direction = weapon.BulletDirection;
    }

    void DrawLaser() {
        bool isHit = Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, collisionLayer);
        // meshRenderer.enabled = true;

        if (isHit) {
            volumetricLine.EndPos = transform.InverseTransformPoint(hit.point);
            CreateDecal(hit);
        } else {
            volumetricLine.EndPos = transform.InverseTransformPoint(transform.position + direction * distance);
        }
    }

    void CheckLaserCollision() {
        bool isHit = Physics.Raycast(transform.position, direction, out RaycastHit hit, distance, collisionLayer);
        if (isHit == false) return;

        // Entity가 맞는지 확인. 아니라면 탄흔 그리기
        var entity = hit.collider.GetComponent<Entity>();
        if (entity == null) {
            CreateDecal(hit);
            return;
        }

        CheckCollision(entity.gameObject);
    }

    void CreateDecal(RaycastHit hit) {
        // 탄흔 쿨타임 도는 중이면 스킵
        if (decalTime > 0f) return;

        // 박힌 대상이 Level 오브젝트고 지정된 탄흔 데칼이 있음
        if (hit.collider.gameObject.CompareTag("Level") && decalPrefab != null) {
            // 충돌 지점에 탄흔 데칼 생성
            var direction = Quaternion.FromToRotation(Vector3.back, hit.normal);
            Instantiate(decalPrefab, hit.point, direction);

            decalTime = decalDelay;
        }
    }
}
