using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController : MonoBehaviour
{
    public static EventController Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    public event Action OnEnemyDeath;

    //Map events
    public void FireEnemyDeathEvent(GameObject movingCharacter)
    {
        OnEnemyDeath?.Invoke();
    }
}
