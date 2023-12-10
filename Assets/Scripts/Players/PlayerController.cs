using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Entity {

    // [Header("Movements")]
    [Tooltip("플레이어가 현재 장착 중인 무기")]
    public GameObject _weapon;

    [Tooltip("플레이어가 부활할 지점")]
    public Transform respawnPoint;

    WeaponController weaponController;

    public GameObject Weapon {
        set {
            _weapon = value;
            weaponController = value.GetComponent<WeaponController>();
        }
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
    }

    public void Attack() {
        weaponController.Fire();
        UIManager.instance.Ammo = weaponController.clip;
    }

    public void Reload() {
        weaponController.Reload();
    }

    public override void OnHit(int damage, bool isCritical, Entity attacker) {
        base.OnHit(damage, isCritical, attacker);
        UIManager.instance.PlayerHealth = health;
        UIManager.instance.PlayerHealthBar = health / (float)maxHealth;
    }

    public override void Death() {
        isDeath = true;
        weaponController.RefillAmmo(); // 부활후 바로 사격할 수 있도록 재장전
        GameManager.instance.PlayerRespawn(gameObject); // 플레이어 리스폰 타이머 시작
        UIManager.instance.ShowPlayerDeath = true;
        gameObject.SetActive(false);
    }

    void OnEnable() {
        isDeath = false;
        health = maxHealth;
        GetComponent<PlayerMovement>().SetTransform(respawnPoint);
        UIManager.instance.ShowPlayerDeath = false;
        UIManager.instance.PlayerHealth = health;
        UIManager.instance.PlayerHealthBar = health / (float)maxHealth;
    }
}
