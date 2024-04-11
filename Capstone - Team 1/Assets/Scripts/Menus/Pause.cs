using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject otherPauseMenu;
    [SerializeField] GameObject pauseButton;
    [SerializeField] GameObject chat;
    [SerializeField] GameObject tutorial;
    private bool tutorialWasOpen;

    private void Awake()
    {
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0;
    }

    public void onlinePauseGame()
    {
        photonView.RPC("otherPauseGame", RpcTarget.Others);
    }

    public void onlineResumeGame()
    {
        photonView.RPC("otherResumeGame", RpcTarget.All);
    }

    [PunRPC]
    private void otherPauseGame()
    {
        pauseButton.SetActive(false);
        chat.SetActive(false);
        otherPauseMenu.SetActive(true);
        pauseGame();
    }

    [PunRPC]
    private void otherResumeGame()
    {
        resumeGame();
        pauseButton.SetActive(true);
        chat.SetActive(true);
        otherPauseMenu.SetActive(false);
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
    }

    public void resumeGame()
    {
        Time.timeScale = 1;
    }

    public void openPauseWithTutorial()
    {
        if(tutorial != null)
        {
            tutorialWasOpen = tutorial.activeSelf;
        }
    }

    public void closePauseWithTutorial()
    {
        tutorial.SetActive(tutorialWasOpen);
    }
}
