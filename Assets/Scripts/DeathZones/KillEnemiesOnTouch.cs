using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemiesOnTouch : MonoBehaviour
{
    public GameObject player;
    private LightUp lightUp;

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        lightUp = GetComponent<LightUp>();
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<EnemyDeath>().TriggerDeath();
        player.GetComponent<PlayerHealth>().OnPlayerHit();
        lightUp.SetKilledSomeone(true);
    }
}
