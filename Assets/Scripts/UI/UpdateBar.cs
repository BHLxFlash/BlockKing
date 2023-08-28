using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateBar : MonoBehaviour
{
    private float maxWidth;

    private void Awake()
    {
        maxWidth = gameObject.GetComponent<RectTransform>().rect.width;
        Debug.Log(maxWidth);
    }

    public void UpdateUI(float newValue, float maxValue)
    {
        gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (newValue/maxValue)*maxWidth);
    }
}
