using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private Canvas pauseCanvas;
    public GameObject gameManager;
    private MusicManager musicManager;

    private void Start()
    {
        pauseCanvas = GetComponent<Canvas>();
        musicManager = gameManager.GetComponent<MusicManager>();
        StartCoroutine(test());
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(2);
        SetPause(true);
    }

    public void SetPause(bool activate)
    {
        if (activate)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        musicManager.PauseSong(activate);

        pauseCanvas.enabled = activate;
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
