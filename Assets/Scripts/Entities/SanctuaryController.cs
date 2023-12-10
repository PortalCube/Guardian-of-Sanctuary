using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ParticleScenario {
    public GameObject particle;
    public float time;
}

public class SanctuaryController : Entity {

    public ParticleScenario[] particleScenarios;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Attack() {
    }

    public void Reload() {
    }

    public override void OnHit(int damage, bool isCritical, Entity attacker) {
        base.OnHit(damage, isCritical, attacker);
        UIManager.instance.SanctuaryHealth = health;
        UIManager.instance.SanctuaryHealthBar = health / (float)maxHealth;
    }

    public override void Death() {
        isDeath = true;
        GameManager.instance.GameOver();
        StartCoroutine(PlayDefeatEffectCoroutine());
    }

    IEnumerator PlayDefeatEffectCoroutine() {
        Queue<ParticleScenario> queue = new(particleScenarios);

        float time = 0f;
        while (queue.Count > 0) {
            yield return null;
            time += Time.deltaTime;

            var scenario = queue.Peek();

            if (scenario.time < time) {
                scenario = queue.Dequeue();
                scenario.particle.SetActive(true);
            }
        }
    }
}
