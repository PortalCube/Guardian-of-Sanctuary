using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour {

    [Header("Settings")]
    [Tooltip("수직 시야 감도")]
    public float lookVerticalSensitivity = 5f;

    [Tooltip("수평 시야 감도")]
    public float lookHorizontalSensitivity = 10f;

    [Header("Axis Names")]
    [Tooltip("수직 이동에 사용할 입력 축")]
    public string verticalMovementAxisName = "Vertical";

    [Tooltip("수평 이동에 사용할 입력 축")]
    public string horizontalMovementAxisName = "Horizontal";

    [Tooltip("주 공격에 사용할 입력 축")]
    public string fireButtonName = "Fire1";

    [Tooltip("재장전에 사용할 입력 축")]
    public string reloadButtonName = "Reload";

    [Tooltip("점프에 사용할 입력 축")]
    public string jumpButtonName = "Jump";

    [Tooltip("무기 변경에 사용할 입력 축")]
    public string switchButtonName = "Switch";

    [Tooltip("아날로그 스틱 Rect Transform")]
    public RectTransform analogStickTransform;

    // 플레이어 속도 값
    public float Speed { get; private set; }

    // 플레이어 이동 벡터 값
    public Vector3 MoveDirection { get; private set; }

    // 플레이어 시야 이동 벡터 값
    public Vector2 LookDirection { get; private set; }

    // 플레이어 주 공격 버튼 여부값
    public bool IsFire { get; private set; }

    // 플레이어 재장전 입력 여부값
    public bool IsReload { get; private set; }

    // 플레이어 점프 입력 여부값
    public bool IsJump { get; private set; }

    // 플레이어 무기 변경 입력 여부값
    public bool IsSwitch { get; private set; }

    // 시야 둘러보기를 컨트롤하는 touch의 fingerId
    int lookControlTouchFingerId = -1;

    // 이전 프레임에서의 마우스 위치
    Vector2 previousTouchPosition = Vector2.zero;

    GameManager gameManager;

    // Start is called before the first frame update
    void Start() {
        gameManager = GameManager.instance;
    }

    // Update is called once per frame
    void Update() {
        // 게임 오버되면 게임 오버 메소드로
        if (gameManager.GameStatus == false) {
            OnGameOver();
            return;
        }

        // Player 움직이기
        var verticalMovement = Input.GetAxis(verticalMovementAxisName);
        var horizontalMovement = Input.GetAxis(horizontalMovementAxisName);
        var movement = new Vector3(horizontalMovement, 0, verticalMovement);
        Speed = Mathf.Min(movement.magnitude, 1);
        MoveDirection = movement.normalized;

        Touch? lookTouch = null;

        // 화면 조작을 담당하는 터치가 없는 상태에서
        if (lookControlTouchFingerId == -1) {
            // 터치 시작이 감지되면
            lookTouch = TouchUtil.FindTouchWithBeganState();

            if (lookTouch.HasValue) {

                // 아날로그 스틱을 건드리지 않았다면
                if (TouchUtil.IsTouchInsideRectTransform(lookTouch.Value.position, analogStickTransform) == false) {
                    // 화면 조작을 담당하는 터치로 할당
                    previousTouchPosition = lookTouch.Value.position;
                    lookControlTouchFingerId = lookTouch.Value.fingerId;
                }
            }
        }

        // 화면 조작을 담당하는 터치가 있다면
        if (lookControlTouchFingerId != -1) {

            // 만약 윗 코드를 실행하지 않았다면 터치 찾기
            if (lookTouch.HasValue == false) {
                lookTouch = TouchUtil.FindTouchWithFingerID(lookControlTouchFingerId);
            }

            if (lookTouch.HasValue == false) {
                // 터치를 못 찾았다면 터치가 종료된 것으로 판별, 둘러보기 해제
                lookControlTouchFingerId = -1;
                LookDirection = Vector2.zero;
            } else {
                // 터치를 찾았다면 움직인 만큼 화면 조작
                var touchPosition = lookTouch.Value.position;
                float x = touchPosition.x - previousTouchPosition.x;
                float y = touchPosition.y - previousTouchPosition.y;
                LookDirection = new Vector2(x * lookHorizontalSensitivity, y * lookVerticalSensitivity);

                // 마지막 터치 좌표를 지금 추출한 좌표로 저장
                previousTouchPosition = touchPosition;
            }

        }

        // Button 입력 받아오기
        IsFire = Input.GetButton(fireButtonName);
        IsReload = Input.GetButtonDown(reloadButtonName);
        IsJump = Input.GetButtonDown(jumpButtonName);
        IsSwitch = Input.GetButton(switchButtonName);
    }

    // 게임 오버된 경우
    public void OnGameOver() {
        // 모든 값을 0으로 지정
        Speed = 0;
        MoveDirection = Vector3.zero;
        LookDirection = Vector2.zero;
        IsFire = false;
        IsReload = false;
        IsJump = false;
        IsSwitch = false;

        // PlayerInput 컴포넌트를 비활성화
        this.enabled = false;
    }
}
