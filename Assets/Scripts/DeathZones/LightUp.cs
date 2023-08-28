using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightUp : MonoBehaviour
{
    Material material;

    Color originalColor;
    Color litUpColor;

    [Range(0.0f, 1.0f)]
    public float litUpAlpha = 0.5f;

    public float secondsMaterialStaysLitUp = 0.25f;

    private float litUpTimer = 0f;
    private bool isLitUp;

    private bool killedSomeone = false;

    BoxCollider collider;

    public GameObject player;

    void Start()
    {
        material = gameObject.GetComponent<Renderer>().material;

        originalColor = material.color;
        litUpColor = new Color(originalColor.r, originalColor.g, originalColor.b, litUpAlpha);

        collider = gameObject.GetComponent<BoxCollider>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        //Tout ça pour éviter une erreur commune possible avec les coroutines
        if (isLitUp)
        {
            litUpTimer -= Time.deltaTime;
            if (litUpTimer <= 0)
            {
                Deactivate();
            }
        }
    }

    public void SetKilledSomeone(bool newValue)
    {
        killedSomeone = newValue;
    }

    public void Activate()
    {
        material.color = litUpColor;
        isLitUp = true;
        collider.enabled = true;
        litUpTimer = secondsMaterialStaysLitUp;
    }

    public void Deactivate()
    {
        if (!killedSomeone)
            player.GetComponent<PlayerHealth>().OnPlayerMiss();
        killedSomeone = false;

        material.color = originalColor;
        isLitUp = false;
        collider.enabled = false;
    }
}
