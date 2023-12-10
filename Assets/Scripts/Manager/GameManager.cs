using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [Tooltip("총알의 탄흔을 얼마나 유지할지 결정합니다.")]
    public float decalRemainTime = 10f;

    [Tooltip("플레이어가 부활하는데 걸리는 시간을 결정합니다.")]
    public float playerRespawnTime = 10f;

    [Tooltip("게임 오버 씬의 대기 시간입니다.")]
    public float gameOverWaitTime = 3f;

    [Tooltip("게임 종료시 전환될 카메라입니다.")]
    public CinemachineVirtualCamera gameoverCamera;

    public static GameManager instance;

    Coroutine respawnCoroutine;

    public bool GameStatus { get; private set; } = true;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            Debug.LogError("[Game Manager] Scene에 인스턴스가 중복되었습니다.");
        } else {
            instance = this;
        }

        Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator RespawnCoroutine(GameObject playerObject) {

        float time = 0f;

        while (time < playerRespawnTime) {
            yield return null;

            time += Time.deltaTime;

            UIManager.instance.PlayerRespawnTime = playerRespawnTime - time;
        }

        playerObject.SetActive(true);
    }

    public void PlayerRespawn(GameObject playerObject) {
        respawnCoroutine = StartCoroutine(RespawnCoroutine(playerObject));
    }

    IEnumerator GameOverCoroutine() {
        yield return new WaitForSeconds(gameOverWaitTime);

        UIManager.instance.ShowGameOver = true;
        UIManager.instance.ShowGameOverBackground = true;
        UIManager.instance.GameOverTitle = "GAME OVER";
        UIManager.instance.GameOverDescription = "성소가 파괴되었습니다.";
    }

    IEnumerator GameClearCoroutine() {
        yield return new WaitForSeconds(gameOverWaitTime);

        UIManager.instance.ShowGameOver = true;
        UIManager.instance.ShowGameClearBackground = true;
        UIManager.instance.GameOverTitle = "GAME CLEAR";
        UIManager.instance.GameOverDescription = "축하드립니다! 성소를 지켜내었습니다.";
    }

    public void GameOver() {
        GameStatus = false;
        gameoverCamera.enabled = true;
        gameoverCamera.Priority = 100;

        if (respawnCoroutine != null) {
            StopCoroutine(respawnCoroutine);
        }

        UIManager.instance.ShowPlayerDeath = false;
        UIManager.instance.ShowTopControlUI = false;
        UIManager.instance.ShowBottomControlUI = false;
        StartCoroutine(GameOverCoroutine());
    }

    public void GameClear() {
        GameStatus = false;
        gameoverCamera.enabled = true;
        gameoverCamera.Priority = 100;

        if (respawnCoroutine != null) {
            StopCoroutine(respawnCoroutine);
        }

        UIManager.instance.ShowPlayerDeath = false;
        UIManager.instance.ShowTopControlUI = false;
        UIManager.instance.ShowBottomControlUI = false;
        StartCoroutine(GameClearCoroutine());
    }

    public void Restart() {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenMainMenu() {
        SceneManager.LoadScene("MainScene");
    }
}
