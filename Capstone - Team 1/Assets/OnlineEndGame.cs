using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class OnlineEndGame : MonoBehaviour
{
    [SerializeField] private Penguin p1;
    [SerializeField] private Penguin p2;
    [SerializeField] private TextMeshProUGUI WinnerText;
    [SerializeField] PhotonView photonView;
    [SerializeField] private TextMeshProUGUI middleText;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button toMenuButton;
    private bool otherPlay;
    private bool mePlay;
    private bool otherSwap;
    private bool meSwap;
    private string p1Name = "Player 1";
    private string p2Name = "Player 2";
    private Player[] playerList;

    private void Awake()
    {
        playerList = PhotonNetwork.PlayerList;


        p1.setHat(OnlineCharacterCustomizationUI.XHAT);
        p2.setHat(OnlineCharacterCustomizationUI.OHAT);

        foreach (Player player in playerList)
        {
            string nickname = player.NickName;
            string localId = PhotonNetwork.LocalPlayer.UserId;
            if (player.UserId != localId && nickname != "")
            {
                //not my nickname, so if im p1 then give to p2
                if (OnlineCharacterCustomizationUI.isP1)
                {
                    p2Name = nickname;
                }
                else
                {
                    p1Name = nickname;
                }
            }
            else if (player.UserId == localId && nickname != "")
            {
                //my nickname, so give to p1 if im p1
                if (OnlineCharacterCustomizationUI.isP1)
                {
                    p1Name = nickname;
                }
                else
                {
                    p2Name = nickname;
                }
            }
        }

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
            WinnerText.text = $"{p1Name} Wins!";
        }
        else if (QuixoClass.isOWin)
        {
            p1.Play("Death");
            p2.Play("Jump");
            WinnerText.text = $"{p2Name} Wins!";
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
            middleText.text = $"Waiting on {(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} to play again";
        }
        else
        {
            swapButton.interactable = true;
            toMenuButton.interactable= true;
        }
        if (!mePlay)
        {
            middleText.text = "";
        }
        photonView.RPC("playAgain", RpcTarget.All);
    }

    [PunRPC]
    private void setOtherPlay()
    {
        otherPlay = !otherPlay;
        if(otherPlay)
        {
            middleText.text = $"{(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} would like to play again";
        }
        else {
            middleText.text = "";
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
            middleText.text = "";
        }
        else if (mePlay && !otherPlay)
        {
            middleText.text = $"Waiting on {(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} to play again";
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
            middleText.text = "";
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
            middleText.text = $"{(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} would like to get new team";
        }
        else
        {
            middleText.text = "";
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
            middleText.text = "";
        }
        //change bools
        else if (meSwap && !otherSwap)
        {
            //change text
            middleText.text = $"Waiting on {(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} to get new teams";
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
        waitSeconds(0.5f);
        DisconnectAndWait();
    }

    private IEnumerator waitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    [PunRPC]
    private void otherToLobby()
    {
        middleText.text = $"{(OnlineCharacterCustomizationUI.isP1 ? p2Name : p1Name)} has left";
    }

}

