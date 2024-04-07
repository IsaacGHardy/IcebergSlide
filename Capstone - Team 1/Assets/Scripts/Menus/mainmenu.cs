using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainmenu : MonoBehaviour
{
    public void PlayGame ()
    {
        SceneManager.LoadScene("TeamSelection");
    }

    public void MultiPlayGame()
    {
        SceneManager.LoadScene("Lobby"); 
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    } 

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}
