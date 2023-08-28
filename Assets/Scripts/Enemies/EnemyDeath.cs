using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    public void TriggerDeath()
    {
        Destroy(gameObject);
        //Play animation
        //Trigger event so score/player health update
    }
}
