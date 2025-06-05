using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

public class PauseButton : MonoBehaviour
{

    [SerializeField] GameObject pauseMenu;

    public void Pause()
    {
        pauseMenu.SetActive(true);
    }

    public void Home()
    {
        pauseMenu.SetActive(true);
    }

}
