using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryController : MonoBehaviour {

    // [Header("Movements")]
    [Tooltip("플레이어의 무기 Prefab 목록")]
    public GameObject[] weaponPrefab;

    [Tooltip("플레이어의 무기 position")]
    public Transform weaponPosition;

    [Tooltip("공격할 대상 레이어")]
    public LayerMask targetLayer;

    // Instantiate된 무기 목록
    GameObject[] weapons;

    PlayerController playerController;

    int _currentIndex;
    public int WeaponIndex {
        set {
            weapons[_currentIndex].SetActive(false);
            weapons[value].SetActive(true);
            playerController.Weapon = weapons[value];
            _currentIndex = value;
        }
    }

    // Start is called before the first frame update
    void Start() {
        playerController = GetComponent<PlayerController>();
        weapons = new GameObject[weaponPrefab.Length];

        for (int i = 0; i < weaponPrefab.Length; i++) {
            weapons[i] = Instantiate(weaponPrefab[i], weaponPosition);

            var weaponController = weapons[i].GetComponent<WeaponController>();
            weaponController.owner = playerController;
            weaponController.targetLayer = targetLayer;
            weaponController.OnReload += () => { UIManager.instance.Ammo = weaponController.clip; };

            weapons[i].SetActive(false);
        }

        WeaponIndex = 0;
    }
}
