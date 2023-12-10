
using UnityEngine;

public class PlayerWeaponController : WeaponController {

    // Update is called once per frame
    protected override void Update() {
        base.Update();
        UpdateUI();
    }

    void UpdateUI() {
        UIManager.instance.ShowReloading = isReloading;

        if (isReloading) {
            UIManager.instance.ReloadingGauge = coolDown / reloadTime;
            UIManager.instance.ReloadingTime = coolDown;
        }
    }

    void OnEnable() {
        UIManager.instance.ShowAmmo = true;
        UIManager.instance.Ammo = clip;
    }


    void OnDisable() {
        UIManager.instance.ShowReloading = false;
    }
}
