using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineEndGame : MonoBehaviour
{
    [SerializeField] private Penguin p1;
    [SerializeField] private Penguin p2;
    [SerializeField] private TextMeshProUGUI WinnerText;
    [SerializeField] PhotonView photonView;
    [SerializeField] private TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private TextMeshProUGUI otherPlayAgainText;
    [SerializeField] private TextMeshProUGUI otherNewTeamsText;
    [SerializeField] private TextMeshProUGUI otherToMenuText;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button toMenuButton;
    private bool otherPlay;
    private bool mePlay;
    private bool otherSwap;
    private bool meSwap;

    private void Awake()
    {
        p1.setHat(OnlineCharacterCustomizationUI.XHAT);
        p2.setHat(OnlineCharacterCustomizationUI.OHAT);


        if (QuixoClass.isXWin && QuixoClass.isOWin)
        {
            p1.Play("Spin");
            p2.Play("Spin");
            WinnerText.text = "You Tied!";
        }
        else if (QuixoClass.isXWin)
        {
            p1.Play("Jump");
            p2.Play("Death");
            WinnerText.text = $"{OnlineCharacterCustomizationUI.P1Nickname} Wins!";
        }
        else if (QuixoClass.isOWin)
        {
            p1.Play("Death");
            p2.Play("Jump");
            WinnerText.text = $"{OnlineCharacterCustomizationUI.P2Nickname} Wins!";
        }
    }

    public void onlinePlayAgain()
    {
        photonView.RPC("setOtherPlay", RpcTarget.Others);
        mePlay = !mePlay;
        if (mePlay)
        {
            swapButton.interactable = false;
            toMenuButton.interactable = false;
        }
        else
        {
            swapButton.interactable = true;
            toMenuButton.interactable= true;
        }
        if (!mePlay)
        {
            playText.gameObject.SetActive(false);
        }
        photonView.RPC("playAgain", RpcTarget.All);
    }

    [PunRPC]
    private void setOtherPlay()
    {
        otherPlay = !otherPlay;
        if(otherPlay)
        {
            otherPlayAgainText.gameObject.SetActive(true);
        }
        else {
            otherPlayAgainText.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void playAgain()
    {
        if (otherPlay && mePlay)
        {
            SceneManager.LoadScene("OnlineGame");
            otherPlay = false;
            mePlay = false;
            swapButton.interactable = true;
            toMenuButton.interactable = true;
            playText.gameObject.SetActive(false);
        }
        else if (mePlay && !otherPlay)
        {
            playText.gameObject.SetActive(true);
        }
    }
     public void onlineNewTeams()
    {
        //To make appropraite changes: change RPC function call
        photonView.RPC("setOtherSwap", RpcTarget.Others);
        //change meSwap to correct bool
        meSwap = !meSwap;
        if (meSwap)
        {
            playButton.interactable = false;
            toMenuButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
            toMenuButton.interactable = true;
        }
        //change meSwap to correct bool
        if (!meSwap)
        {
            //grab correct text
            swapText.gameObject.SetActive(false);
        }
        //Change RPC func call
        photonView.RPC("newTeams", RpcTarget.All);
    }
    [PunRPC]
    private void setOtherSwap()
    {
        otherSwap = !otherSwap;
        if (otherSwap)
        {
            otherNewTeamsText.gameObject.SetActive(true);
        }
        else
        {
            otherNewTeamsText.gameObject.SetActive(false);
        }
    }
    [PunRPC]
    private void newTeams()
    {
        //change to correct bools
        if (otherSwap && meSwap)
        {
            //change logic to accomplish necssary
            SceneManager.LoadScene("OnlineTeamSelection");
            //change these two bools and text
            otherSwap = false;
            meSwap = false;
            playButton.interactable = true;
            toMenuButton.interactable=true;
            swapText.gameObject.SetActive(false);
        }
        //change bools
        else if (meSwap && !otherSwap)
        {
            //change text
            swapText.gameObject.SetActive(true);
        }

    }

    public void DisconnectAndWait()
    {
        PhotonNetwork.Disconnect();
        StartCoroutine(WaitForDisconnect());

    }

    private IEnumerator WaitForDisconnect()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        OnlineCharacterCustomizationUI.resetHats();
        SceneManager.LoadScene("Lobby");
    }

    public void returnToLobby()
    {
        //NOT WORKING
        photonView.RPC("otherToLobby", RpcTarget.Others);
        waitSeconds(0.1f);
        DisconnectAndWait();
    }

    private IEnumerator waitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    [PunRPC]
    private void otherToLobby()
    {
        otherToMenuText.gameObject.SetActive(true);
    }
}

