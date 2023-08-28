using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    float maxHP = 10;
    [SerializeField]
    float startingHP = 5f;
    float currentHP;

    [SerializeField]
    float hpLostPerEnemyHit = 2f;
    [SerializeField]
    float hpLostPerPlayerMiss = 1f;
    [SerializeField]
    float hpGainedPerPlayerHit = 0.25f;
    [SerializeField]
    float hpGainedPerPlayerPerfectHit = 0.5f;

    public Image healthBar;
    public Image specialBar;

    public GameObject[] scoreUI;
    public GameObject[] comboUI;
    public GameObject[] multiplierUI;
    private int score = 0;
    private int combo = 0;
    private int multiplier = 1;


    void Awake()
    {
        currentHP = startingHP;
    }

    void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEnemyHit();
        other.gameObject.GetComponent<EnemyDeath>().TriggerDeath();
    }

    public void OnPlayerDeath()
    {
        gameObject.GetComponent<Animator>().Play("Death");
    }

    public void OnEnemyHit()
    {
        currentHP -= hpLostPerEnemyHit;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        combo = 0;
        multiplier = 1;
        UpdateUI();

        if (currentHP <= 0)
        {
            OnPlayerDeath();
        }
        else
        {
            gameObject.GetComponent<Animator>().Play("TakeDamage");
        }
    }

    public void OnPlayerMiss()
    {
        currentHP -= hpLostPerPlayerMiss;
        currentHP = Mathf.Clamp(currentHP, 1, maxHP);

        combo = 0;
        multiplier = 1;
        UpdateUI();

        gameObject.GetComponent<Animator>().Play("TakeDamage");
    }

    public void OnPlayerHit()
    {
        currentHP += hpGainedPerPlayerHit;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        combo++;
        if (combo < 5)
            multiplier = 1;
        else if (combo < 10)
            multiplier = 2;
        else if (combo < 15)
            multiplier = 3;
        else if (combo < 20)
            multiplier = 4;
        else
            multiplier = 5;
        score += 100 * multiplier;
        UpdateUI();
    }

    public void OnPlayerPerfectHit()
    {
        currentHP += hpGainedPerPlayerPerfectHit;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);

        combo++;
        if (combo < 5)
            multiplier = 1;
        else if (combo < 10)
            multiplier = 2;
        else if (combo < 15)
            multiplier = 3;
        else if (combo < 20)
            multiplier = 4;
        else
            multiplier = 5;
        score += 200 * multiplier;
        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateHealthBar();
        //UpdateSpecialBar();

        foreach (GameObject go in scoreUI)
        {
            go.GetComponent<UpdateText>().UpdateUIText(score, true);
        }
        foreach (GameObject go in comboUI)
        {
            go.GetComponent<UpdateText>().UpdateUIText(combo, true);
        }
        foreach (GameObject go in multiplierUI)
        {
            go.GetComponent<UpdateText>().UpdateUIText(multiplier, true);
        }
}

    private void UpdateHealthBar()
    {
        healthBar.GetComponent<UpdateBar>().UpdateUI(currentHP, maxHP);
    }

    private void UpdateSpecialBar()
    {

    }
}
