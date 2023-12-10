using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {
    public float rotationSpeed = 0.3f;
    public float blendSpeed = 1f;
    SkyboxBlender skyboxBlender;

    float angle = 0f;

    // Start is called before the first frame update
    void Start() {
        skyboxBlender = GetComponent<SkyboxBlender>();
    }

    // Update is called once per frame
    void Update() {
        skyboxBlender.rotation += rotationSpeed * Time.deltaTime;

        angle += blendSpeed * Time.deltaTime;

        if (angle > 360f) {
            angle = angle % 360f;
        }

        if (skyboxBlender.rotation > 360f) {
            skyboxBlender.rotation = skyboxBlender.rotation % 360f;
        }

        var value = (Mathf.Sin(angle * Mathf.Deg2Rad) + 1) / 2;

        skyboxBlender.blend = value;
    }

    public void GameStart() {
        SceneManager.LoadScene("GameScene");
    }

    public void Exit() {
        Application.Quit();
    }
}
