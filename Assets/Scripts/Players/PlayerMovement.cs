using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {


    [Header("Movements")]
    [Tooltip("플레이어의 이동 속도")]
    public float moveSpeed = 10f;

    [Tooltip("플레이어의 점프 높이")]
    public float jumpHeight = 20f;

    [Tooltip("플레이어의 수직 시야 최소각")]
    public float minVerticalAngle = -25f;

    [Tooltip("플레이어의 수직 시야 최대각")]
    public float maxVerticalAngle = 85f;

    [Tooltip("플레이어가 벽 또는 천장으로 인정하는 법선 벡터의 y 성분 값")]
    public float wallVectorComponent = 0.3f;

    [Tooltip("플레이어가 바닥으로 인정하는 법선 벡터의 y 성분 값")]
    public float floorVectorComponent = 0.6f;


    [Header("Components")]
    [Tooltip("플레이어가 바라볼 GameObject의 Transform")]
    public Transform cameraLookAt;

    public bool isJumpable = true;

    PlayerController playerController;
    PlayerInput playerInput;
    Rigidbody rigidbodyComponent;

    // Start is called before the first frame update
    void Start() {
        playerController = GetComponent<PlayerController>();
        playerInput = GetComponent<PlayerInput>();
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    void Update() {
        if (GameManager.instance.GameStatus == false) return;

        UpdateMovement();
    }


    void UpdateMovement() {
        // 이동
        var movement = playerInput.MoveDirection * playerInput.Speed * moveSpeed * Time.deltaTime;
        rigidbodyComponent.MovePosition(transform.position + transform.rotation * movement);

        // 시야 둘러보기
        var horizontalRotation = playerInput.LookDirection.x * Time.deltaTime;
        var verticalRotation = playerInput.LookDirection.y * Time.deltaTime;

        // 수평 시야
        var characterRotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y + horizontalRotation, 0);
        rigidbodyComponent.MoveRotation(characterRotation);

        // 수직 시야
        var cameraAngle = cameraLookAt.rotation.eulerAngles.x;

        if (cameraAngle > 180) {
            cameraAngle -= 360;
        }

        var cameraRotationAngle = Mathf.Clamp(cameraAngle - verticalRotation, minVerticalAngle, maxVerticalAngle);
        cameraLookAt.localRotation = Quaternion.Euler(cameraRotationAngle, 0, 0);

        // 점프
        if (playerInput.IsJump && isJumpable) {
            rigidbodyComponent.velocity += Vector3.up * jumpHeight;
            isJumpable = false;
        }

        // 발사
        if (playerInput.IsFire) {
            playerController.Attack();
        }

        // 재장전
        if (playerInput.IsReload) {
            playerController.Reload();
        }
    }

    void OnCollisionEnter(Collision collider) {
        if (collider.contactCount > 0) {
            CheckJumpable(collider.contacts[0]);
        }

    }

    // 적당한 경사를 가진 바닥에 충돌했는지 판정하여 점프 가능 여부를 결정
    void CheckJumpable(ContactPoint contact) {
        // 충돌 지점의 법선 벡터로 벽이나 천장과 부딫혔는지 판정
        if (contact.normal.y <= wallVectorComponent) {
            // 바닥이 아니므로 스킵
            return;
        }

        isJumpable = contact.normal.y > floorVectorComponent;
    }

    public void SetTransform(Transform newTransform) {
        transform.position = newTransform.position;
        transform.rotation = newTransform.rotation;
        cameraLookAt.localRotation = Quaternion.Euler(newTransform.rotation.eulerAngles.x, 0, 0);
    }
}
