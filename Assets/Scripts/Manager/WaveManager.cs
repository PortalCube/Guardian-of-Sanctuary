using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    public AudioClip dayTimeMusic;
    public AudioClip nightTimeMusic;
    public float fadeTime = 1f;
    public float delayTime = 1f;

    public WaveData[] waveList;

    Queue<WaveData> waves;

    public WaveData currentWave;

    public Queue<Enemy> enemies;

    public Transform[] spawnPoints;

    public float breakTime;

    public float waveTime;

    public bool isWaveStart;

    AudioSource audioSource;

    int _enemiesLeft = 0;
    public int EnemiesLeft {
        get {
            int count = _enemiesLeft;
            foreach (var enemy in enemies) {
                count += enemy.count;
            }
            return count;
        }
    }

    public static WaveManager instance;

    void Awake() {
        if (instance) {
            Destroy(gameObject);
            Debug.LogError("[Wave Manager] Scene에 인스턴스가 중복되었습니다.");
        } else {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        waveTime = breakTime;
        isWaveStart = false;
        waves = new Queue<WaveData>(waveList);
        enemies = new Queue<Enemy>();
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(dayTimeMusic);

        UIManager.instance.WaveTitle = "BREAK TIME";
        UIManager.instance.Enemies = (int)waveTime;
    }

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.GameStatus == false) {
            OnGameOver();
            return;
        }

        UpdateTime();

        if (isWaveStart && EnemiesLeft <= 0) {
            EndWave();
        }
    }

    void UpdateTime() {
        if (isWaveStart) {
            waveTime += Time.deltaTime;
            SpawnEnemies();
        } else {
            waveTime -= Time.deltaTime;
            UIManager.instance.Enemies = (int)waveTime;

            if (waveTime < 0f) {
                StartWave();
            }
        }
    }

    void SpawnEnemies() {
        if (enemies.Count <= 0) return;

        var nextEnemy = enemies.Peek();
        if (nextEnemy.time <= waveTime) {
            nextEnemy = enemies.Dequeue();

            // 만약 없는 spawnPoint를 찾는 경우, spawnPoint를 0으로 지정
            if (nextEnemy.spawnPoint < 0 || nextEnemy.spawnPoint >= spawnPoints.Length) {
                nextEnemy.spawnPoint = 0;
            }

            // spawnPoints 중에서 엔티티를 소환할 위치를 지정
            var spawnPoint = spawnPoints[nextEnemy.spawnPoint];

            // 엔티티를 count 만큼 소환
            for (int i = 0; i < nextEnemy.count; i++) {
                Instantiate(nextEnemy.entityPrefab, spawnPoint.position, spawnPoint.rotation);
            }

            // 필드상의 엔티티에 추가
            _enemiesLeft += nextEnemy.count;
        }
    }

    void StartWave() {
        StartCoroutine(ChangeAudio(nightTimeMusic));

        // 밤으로 전환되는 애니메이션이 종료 후 적이 생성되도록, -2.5초로 부여
        waveTime = -2.5f;
        isWaveStart = true;

        // wave 목록 중 첫번째 wave를 가져오기
        currentWave = waves.Dequeue();

        // currentWave의 적 목록을 시간순으로 정렬하고 enemies에 Queue로 저장
        var enemyList = new List<Enemy>(currentWave.enemies);
        enemyList.Sort();

        foreach (var enemy in enemyList) {
            enemies.Enqueue(enemy);
        }

        // 남은 적 업데이트
        _enemiesLeft = 0;

        UIManager.instance.WaveTitle = currentWave.name;
        UIManager.instance.Enemies = EnemiesLeft;

        // 밤으로 전환
        SkyboxManager.instance.SetNight();
    }

    void EndWave() {
        StartCoroutine(ChangeAudio(dayTimeMusic));

        waveTime = breakTime;
        isWaveStart = false;

        UIManager.instance.WaveTitle = "BREAK TIME";
        UIManager.instance.Enemies = (int)waveTime;

        // 낮으로 전환
        SkyboxManager.instance.SetDay();

        if (waves.Count <= 0) {
            // 게임 클리어
            GameManager.instance.GameClear();
        }
    }

    public void EnemyDeath() {
        _enemiesLeft--;
        UIManager.instance.Enemies = EnemiesLeft;
    }

    IEnumerator StopAudio() {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }


    IEnumerator ChangeAudio(AudioClip clip) {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0) {
            audioSource.volume -= startVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;

        yield return new WaitForSeconds(delayTime);

        audioSource.PlayOneShot(clip);
    }

    void OnGameOver() {
        StartCoroutine(StopAudio());
        this.enabled = false;
    }
}
