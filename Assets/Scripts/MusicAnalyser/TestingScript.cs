using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log((Application.dataPath + "/Songs/test.mp3"));
        MusicAnalyser musicAnalyser = new MusicAnalyser();
        musicAnalyser.CreateEnemyDataFromFile("test.mp3");
    }
}
