using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class SkyboxManager : MonoBehaviour {
    public float rotationSpeed = 0.3f;
    public float transitionDuration = 1f;

    SkyboxBlender skyboxBlender;

    public static SkyboxManager instance;

    Coroutine coroutine = null;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            Debug.LogError("[Skybox Manager] Scene에 인스턴스가 중복되었습니다.");
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        skyboxBlender = GetComponent<SkyboxBlender>();
    }

    // Update is called once per frame
    void Update() {
        skyboxBlender.rotation += rotationSpeed * Time.deltaTime;

        if (skyboxBlender.rotation > 360f) {
            skyboxBlender.rotation = skyboxBlender.rotation % 360f;
        }
    }

    IEnumerator BlendCoroutine(float from, float to, float duration) {
        float time = 0f;

        while (time < duration) {
            yield return null;

            time += Time.deltaTime;

            var value = Mathf.Lerp(from, to, time / duration);

            skyboxBlender.blend = value;
        }
    }


    public void SetDay() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(BlendCoroutine(1, 0, transitionDuration));
    }

    public void SetNight() {
        if (coroutine != null) {
            StopCoroutine(coroutine);
        }

        coroutine = StartCoroutine(BlendCoroutine(0, 1, transitionDuration));
    }
}
