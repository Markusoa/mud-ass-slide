using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayBtn() {
        SceneManager.LoadScene("level1");
    }

    public void QuitBtn() {
        Application.Quit();
    }
}
