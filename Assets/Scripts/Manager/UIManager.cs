using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public GameObject topControlUI;
    public GameObject bottomControlUI;

    public TMP_Text waveTitleText;
    public TMP_Text enemiesText;

    public GameObject ammoUI;
    public TMP_Text ammoText;
    public GameObject needReloadText;

    public GameObject reloadingUI;
    public TMP_Text reloadingText;
    public Image reloadingGauge;

    public TMP_Text playerHealthText;
    public TMP_Text sanctuaryHealthText;
    public RectTransform playerHealthMask;
    public RectTransform sanctuaryHealthMask;

    public GameObject playerDeathUI;
    public TMP_Text playerRespawnTimeText;

    public GameObject gameOverUI;
    public GameObject gameOverBackground;
    public GameObject gameClearBackground;
    public TMP_Text gameOverTitleText;
    public TMP_Text gameOverDescriptionText;

    public static UIManager instance;

    float playerHealthMaskWidth = 460.58f;
    float sanctuaryHealthMaskWidth = 484.33f;

    public bool ShowTopControlUI {
        set {
            topControlUI.SetActive(value);
        }
    }

    public bool ShowBottomControlUI {
        set {
            bottomControlUI.SetActive(value);
        }
    }

    public int PlayerHealth {
        set {
            playerHealthText.text = value.ToString();
        }
    }

    public int SanctuaryHealth {
        set {
            sanctuaryHealthText.text = value.ToString();
        }
    }

    public float PlayerHealthBar {
        set {
            var size = playerHealthMask.sizeDelta;
            size.x = playerHealthMaskWidth * value;
            playerHealthMask.sizeDelta = size;
        }
    }

    public float SanctuaryHealthBar {
        set {
            var size = sanctuaryHealthMask.sizeDelta;
            size.x = sanctuaryHealthMaskWidth * value;
            sanctuaryHealthMask.sizeDelta = size;
        }
    }

    public string WaveTitle {
        set {
            waveTitleText.text = value;
        }
    }

    public int Enemies {
        set {
            enemiesText.text = value.ToString();
        }
    }

    public int Ammo {
        set {
            ammoText.text = value.ToString();
            needReloadText.SetActive(value == 0);
        }
    }

    public bool ShowAmmo {
        set {
            ammoUI.SetActive(value);
        }
    }

    public bool ShowReloading {
        set {
            reloadingUI.SetActive(value);
        }
    }

    public float ReloadingTime {
        set {
            reloadingText.text = String.Format("{0:0.0}", value);
        }
    }

    public float ReloadingGauge {
        set {
            reloadingGauge.fillAmount = 1 - value;
        }
    }

    public bool ShowPlayerDeath {
        set {
            playerDeathUI.SetActive(value);
            ShowBottomControlUI = !value;
        }
    }

    public float PlayerRespawnTime {
        set {
            playerRespawnTimeText.text = String.Format("성소의 힘으로 {0:0.0}초 후에 부활합니다...", value);
        }
    }

    public bool ShowGameOver {
        set {
            gameOverUI.SetActive(value);
            ShowTopControlUI = !value;
            ShowBottomControlUI = !value;
        }
    }

    public bool ShowGameOverBackground {
        set {
            gameOverBackground.SetActive(value);
        }
    }

    public bool ShowGameClearBackground {
        set {
            gameClearBackground.SetActive(value);
        }
    }

    public string GameOverTitle {
        set {
            gameOverTitleText.text = value;
        }
    }

    public string GameOverDescription {
        set {
            gameOverDescriptionText.text = value;
        }
    }

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            Debug.LogError("[UI Manager] Scene에 인스턴스가 중복되었습니다.");
        } else {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start() {
        // playerHealthMaskWidth = playerHealthMask.sizeDelta.x;
        // sanctuaryHealthMaskWidth = sanctuaryHealthMask.sizeDelta.x;
    }

    // Update is called once per frame
    void Update() {

    }
}
