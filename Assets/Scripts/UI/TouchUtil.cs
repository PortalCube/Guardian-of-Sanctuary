


using UnityEngine;
using UnityEngine.EventSystems;

// 터치 컨트롤에 대한 지원을 제공하는 클래스입니다.
public static class TouchUtil {

    public static Touch? FindTouchWithBeganState() {
        for (int i = 0; i < Input.touchCount; i++) {
            var touch = Input.GetTouch(i);
            var isPointingUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);

            // UI를 터치하는지 확인
            if (isPointingUI == false && touch.phase == TouchPhase.Began) {
                return touch;
            }
        }

        return null;
    }

    public static Touch? FindTouchWithFingerID(int fingerID) {
        for (int i = 0; i < Input.touchCount; i++) {
            var touch = Input.GetTouch(i);

            if (touch.fingerId == fingerID) {
                return touch;
            }
        }

        return null;
    }

    public static bool IsTouchInsideRectTransform(Vector2 touchPosition, RectTransform rectTransform) {
        var rectX = rectTransform.position.x;
        var rectY = rectTransform.position.y;
        var rectHalfWidth = rectTransform.sizeDelta.x / 2;
        var rectHalfHeight = rectTransform.sizeDelta.y / 2;

        var touchX = touchPosition.x;
        var touchY = touchPosition.y;

        var checkX = touchPosition.x > rectX - rectHalfWidth && touchPosition.x < rectX + rectHalfWidth;
        var checkY = touchPosition.y > rectY - rectHalfHeight && touchPosition.y < rectY + rectHalfHeight;

        return checkX && checkY;
    }
}