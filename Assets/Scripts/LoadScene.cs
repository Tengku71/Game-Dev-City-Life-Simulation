using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadScene : MonoBehaviour
{

    public void ChangeScene(string name)
    {
        SceneManager.LoadScene(name);

    }

    public void Pause()
    {
        Time.timeScale = 0;
    }


    public void Resume()
    {
        Time.timeScale = 1;
    }
}
