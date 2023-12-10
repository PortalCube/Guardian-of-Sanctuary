using UnityEngine;

public class BulletController : Projectile {

    [Tooltip("총알의 속도")]
    public float speed = 25f;

    [Tooltip("Level에 남길 탄흔 Prefab")]
    public GameObject decalPrefab;

    // Start is called before the first frame update
    void Start() {
        var rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(direction.normalized * speed, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update() {
        base.Update();
    }

    // 총알이 박힘
    void OnCollisionEnter(Collision collider) {
        CheckCollision(collider.gameObject);

        // 박힌 대상이 Level 오브젝트고 지정된 탄흔 데칼이 있음
        if (collider.gameObject.CompareTag("Level") && decalPrefab != null) {
            // 충돌 지점을 확인
            var contact = collider.GetContact(0);

            // 충돌 지점에 탄흔 데칼 생성
            var direction = Quaternion.FromToRotation(Vector3.back, contact.normal);
            Instantiate(decalPrefab, contact.point, direction);
        }

        Destroy(gameObject);
    }
}
