using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
    private string originalText = "";
    private int oldValue = 0;
    public int startingValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        oldValue = startingValue;
        originalText = GetComponent<TextMeshProUGUI>().text;
        UpdateUIText(0);
    }

    public void UpdateUIText(int valueToAdd, bool overrideValue = false)
    {
        if (overrideValue)
            oldValue = valueToAdd;
        else
            oldValue += valueToAdd;

        GetComponent<TextMeshProUGUI>().text = originalText + oldValue;
    }
}
