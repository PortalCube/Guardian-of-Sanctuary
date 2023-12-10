using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponMenuController : MonoBehaviour {

    [Tooltip("플레이어 인벤토리 Controller")]
    public PlayerInventoryController playerInventoryController;

    [Tooltip("버튼 클릭시 활성화할 Menu UI")]
    public GameObject weaponMenuUI;

    [Tooltip("Menu Background 이미지")]
    public Image backgroundImage;

    [Tooltip("Menu HighLight 이미지")]
    public Image highlightImage;

    [Tooltip("현재 선택된 무기 이미지")]
    public Image selectedImage;

    [Tooltip("Menu 아이템 Prefab")]
    public GameObject itemPrefab;

    [Tooltip("버튼 클릭 후 Drag를 활성화할 distance")]
    public float dragEnableDistance = 10f;

    [Tooltip("Menu 아이템 radius padding")]
    public float itemPadding = 10f;

    [Tooltip("메뉴에 생성할 이미지들")]
    public Sprite[] sprites;

    float imageSize;

    int currentSelect = -1;

    Image[] itemImages;

    RectTransform rectTransform;

    // TODO: 수학 좀 더 공부하기........

    public float ItemRingRadius {
        get {
            var buttonWidth = rectTransform.sizeDelta.x;
            var buttonRadius = buttonWidth / 2;
            var backgroundRadius = backgroundImage.GetComponent<RectTransform>().sizeDelta.x / 2;
            return backgroundRadius - buttonRadius;
        }
    }

    public float ItemRingStartAngle {
        get {
            // bottom angle
            float fillOriginAngle = 270f;
            return (fillOriginAngle + backgroundImage.GetComponent<RectTransform>().rotation.eulerAngles.z - ItemRingSweep) % 360;
        }
    }

    public float ItemRingSweep {
        get {
            return 360 * backgroundImage.fillAmount;
        }
    }

    public float ItemSweep {
        get {
            return ItemRingSweep / sprites.Length;
        }
    }

    Vector2 AngleToVector2(float angle) {
        var radian = angle * Mathf.Deg2Rad;
        return new Vector2(Mathf.Sin(radian), Mathf.Cos(radian)).normalized;
    }

    // Start is called before the first frame update
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        itemImages = new Image[sprites.Length];

        // 무기 아이템을 메뉴에 추가
        for (int i = 0; i < sprites.Length; i++) {
            var angle = ItemRingStartAngle - i * ItemSweep;

            if (angle < 0) {
                angle += 360;
            }

            var direction = AngleToVector2(angle) * (ItemRingRadius + itemPadding);
            var position = rectTransform.position + new Vector3(direction.x, direction.y, 0);

            GameObject obj = Instantiate(itemPrefab, position, Quaternion.identity, weaponMenuUI.transform);
            itemImages[i] = obj.GetComponent<Image>();
            itemImages[i].sprite = sprites[i];
        }

        imageSize = itemImages[0].GetComponent<RectTransform>().sizeDelta.x;
    }

    // Update is called once per frame
    void Update() {

    }

    public void OnPointerDown(BaseEventData eventData) {
        weaponMenuUI.SetActive(true);
    }

    public void Drag(BaseEventData eventData) {
        // eventData가 PointerEventData가 맞는지 확인
        if (eventData is PointerEventData == false) return;

        // PointerEventData로 다운캐스팅
        PointerEventData pointerEventData = eventData as PointerEventData;

        var halfSize = 360 * highlightImage.fillAmount / 2;

        var currentPosition = new Vector2(rectTransform.position.x, rectTransform.position.y);
        var direction = pointerEventData.position - currentPosition;
        var angle = Vector2.SignedAngle(Vector2.right, direction);

        if (angle < 0) {
            angle += 360;
        }

        // angle로부터 현재 커서가 hover 중인 item 찾기
        int i = Mathf.FloorToInt((angle - ItemRingStartAngle) / ItemSweep);
        if (i >= 0 && i < sprites.Length) {

            if (currentSelect != -1) {
                itemImages[currentSelect].GetComponent<RectTransform>().sizeDelta = Vector2.one * imageSize;
            }

            currentSelect = i;
            itemImages[currentSelect].GetComponent<RectTransform>().sizeDelta = Vector2.one * imageSize * 1.2f;
        } else {
            currentSelect = -1;
        }

        angle += 90 + halfSize;

        // 버튼으로 부터 거리가 충분하지 않다면 drag 비활성화
        var distance = Vector2.Distance(currentPosition, pointerEventData.position);
        if (distance < dragEnableDistance) return;

        highlightImage.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void OnPointerUp(BaseEventData eventData) {
        weaponMenuUI.SetActive(false);

        if (currentSelect != -1) {
            itemImages[currentSelect].GetComponent<RectTransform>().sizeDelta = Vector2.one * imageSize;
            selectedImage.sprite = sprites[currentSelect];

            playerInventoryController.WeaponIndex = currentSelect;

            currentSelect = -1;
        }
    }

    void OnDisable() {
        weaponMenuUI.SetActive(false);
        currentSelect = -1;
    }
}
