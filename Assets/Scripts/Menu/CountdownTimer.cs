using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownTimer : MonoBehaviour
{
    private float timer = 26f;
    public GameObject credits;
    public GameObject menu;
    public GameObject logo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            credits.SetActive(false);
            menu.SetActive(true);
            logo.SetActive(true);
        }
    }
}
