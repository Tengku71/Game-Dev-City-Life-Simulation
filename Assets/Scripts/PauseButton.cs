using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject analog;
    [SerializeField] GameObject analogCircle;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        pauseMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }

    public void Home()
    {
        pauseMenu.SetActive(true);
    }

}
