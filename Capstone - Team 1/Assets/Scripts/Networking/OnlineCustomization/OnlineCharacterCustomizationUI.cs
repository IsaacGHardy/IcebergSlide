using ExitGames.Client.Photon;
using JetBrains.Annotations;
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

public class OnlineCharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] private OnlineCharacterCustomization characterCustomization;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private TextMeshProUGUI selectText;
    [SerializeField] private TextMeshProUGUI otherSwapText;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button p1Button;
    [SerializeField] private Button p2Button;
    [SerializeField] private GameObject rotate1;
    [SerializeField] private GameObject rotate2;
    [SerializeField] private GameObject p1ReadyButton;
    [SerializeField] private GameObject p2ReadyButton;
    [SerializeField] private GameObject p1Buttons;
    [SerializeField] private GameObject p2Buttons;
    public static Hat XHAT;
    public static Hat OHAT;
    public static string P1Nickname = "Player 1";
    public static string P2Nickname = "Player 2";
    private bool isP1 = false;
    private bool otherPlay;
    private bool mePlay;
    private bool otherSwap;
    private bool meSwap;
    private Player[] playerList;

    private void Awake()
    {
        playerList = PhotonNetwork.PlayerList;
    }

    public static void resetHats()
    {
        XHAT = null;
        OHAT = null;
    }

    public void onlineP1For()
    {
        photonView.RPC("p1For", RpcTarget.All);
    }

    [PunRPC]
    public void p1For()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.ForwardHat(true);
    }

    public void onlineP1Back()
    {
        photonView.RPC("p1Back", RpcTarget.All);
    }

    [PunRPC]
    public void p1Back()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.BackwardHat(true);

    }

    public void onlineP2For()
    {
        photonView.RPC("p2For", RpcTarget.All);
    }

    [PunRPC]
    public void p2For()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.ForwardHat(false);

    }

    public void onlineP2Back()
    {
        photonView.RPC("p2Back", RpcTarget.All);
    }

    [PunRPC]
    public void p2Back()
    {
        //boolean indicates if it is the p1 penguin being changed
        characterCustomization.BackwardHat(false);
    }

    public void onlineSwitchSides()
    {
        //To make appropraite changes: change RPC function call
        photonView.RPC("setOtherSwap", RpcTarget.Others);
        //change meSwap to correct bool
        meSwap = !meSwap;
        if (meSwap)
        {
            playButton.interactable = false;
        }
        else
        {
            playButton.interactable = true;
        }
        //change meSwap to correct bool
        if (!meSwap)
        {
            //grab correct text
            swapText.gameObject.SetActive(false);
        }
        //Change RPC func call
        photonView.RPC("switchSides", RpcTarget.All);
    }

    [PunRPC]
    public void setOtherSwap()
    {
        //change to correct bool
        otherSwap = !otherSwap;
        if(otherSwap)
        {
            otherSwapText.gameObject.SetActive(true);
        }
        else
        {
            otherSwapText.gameObject.SetActive(false);

        }
    }

    [PunRPC]
    public void switchSides()
    {
        //change to correct bools
        if (otherSwap && meSwap)
        {
            //change logic to accomplish necssary
            characterCustomization.switchSides();
            swapNicknames();
            isP1 = !isP1;
            swapCustomButtons();
            //change these two bools and text
            otherSwap = false;
            meSwap = false;
            otherSwapText.gameObject.SetActive(false);
            playButton.interactable = true;
            swapText.gameObject.SetActive(false);
        }
        //change bools
        else if (meSwap && !otherSwap)
        {
            //change text
            swapText.gameObject.SetActive(true);
        }
    }
    
    private void swapNicknames()
    {
        string temp = P1Nickname;
        P1Nickname = P2Nickname;
        P2Nickname = temp;
    }

    public void onlineStart()
    {
        photonView.RPC("setOtherPlay", RpcTarget.Others);
        mePlay = !mePlay;
        if(mePlay)
        {
            swapButton.interactable = false;
            setMyReadyButtons(mePlay);

        }
        else
        {
            swapButton.interactable = true;
            setMyReadyButtons(mePlay);
        }
        if (!mePlay)
        {
            playText.gameObject.SetActive(false);
        }
        photonView.RPC("startGame", RpcTarget.All);
    }

    [PunRPC]
    private void setOtherPlay()
    {
        otherPlay = !otherPlay;
        if(otherPlay)
        {
            setOtherReadyButtons(otherPlay);
        }
        else
        {
            setOtherReadyButtons(otherPlay);
        }
    }

    private void setOtherReadyButtons(bool on)
    {
        if (isP1)
        {
            p2ReadyButton.gameObject.SetActive(on);
        }
        else
        {
            p1ReadyButton.gameObject.SetActive(on);
        }
    }

    private void setMyReadyButtons(bool on)
    {
        if (isP1)
        {
            p1ReadyButton.gameObject.SetActive(on);
        }
        else
        {
            p2ReadyButton.gameObject.SetActive(on);
        }
    }

    [PunRPC]
    public void startGame()
    {
        if (otherPlay && mePlay)
        {
            XHAT = characterCustomization.p1Hat;
            OHAT = characterCustomization.p2Hat;
            QuixoClass.isPlayer1 = isP1;
            SceneManager.LoadScene("OnlineGame");
            otherPlay = false;
            mePlay = false;
            swapButton.interactable = true;
            playText.gameObject.SetActive(false);
        }
        else if (mePlay && !otherPlay)
        {
            playText.gameObject.SetActive(true);
        }
    }


    public void p1Rand()
    {
        characterCustomization.setRandomHat(true);
    }

    public void p2Rand()
    {
        characterCustomization.setRandomHat(false);
    }

    public void onlineChooseP1()
    {
        meChooseP1();
        photonView.RPC("otherChooseP1", RpcTarget.Others);
    }

    private void meChooseP1()
    {
        isP1 = true;
        selectText.gameObject.SetActive(false);
        setMiddleButtons();
        p1Buttons.SetActive(true);
        activateRotateView();
        p2Button.gameObject.SetActive(false);
        p1Button.gameObject.SetActive(false);
        characterCustomization.seeP1();

        foreach (Player player in playerList)
        {
            string nickname = player.NickName;
            string localId = PhotonNetwork.LocalPlayer.UserId;
            if (player.UserId ==  localId && nickname != "")
            {
                P1Nickname = nickname;
            }
            else if(player.UserId != localId && nickname != "")
            {
                P2Nickname = nickname;
            }
        }
    }

    [PunRPC]
    private void otherChooseP1()
    {
        characterCustomization.seeP1();
        p1Button.gameObject.SetActive(false);
        rotate1.gameObject.SetActive(true);
        rotate1.GetComponent<Image>().enabled = false;

        foreach (Player player in playerList)
        {
            string nickname = player.NickName;
            string localId = PhotonNetwork.LocalPlayer.UserId;
            if (player.UserId != localId && nickname != "")
            {
                P1Nickname = nickname;
            }
            else if (player.UserId == localId && nickname != "")
            {
                P2Nickname = nickname;
            }
        }
    }

    public void onlineChooseP2()
    {
        meChooseP2();
        photonView.RPC("otherChooseP2", RpcTarget.Others);
    }

    private void meChooseP2()
    {
        isP1 = false;
        selectText.gameObject.SetActive(false);
        setMiddleButtons();
        p2Buttons.SetActive(true);
        activateRotateView();
        p1Button.gameObject.SetActive(false);
        p2Button.gameObject.SetActive(false);
        characterCustomization.seeP2();
    }

    [PunRPC]
    private void otherChooseP2()
    {
        characterCustomization.seeP2();
        p2Button.gameObject.SetActive(false);
        rotate2.gameObject.SetActive(true);
        rotate2.GetComponent<Image>().enabled = false;
    }

    private void swapCustomButtons()
    {
        p1Buttons.SetActive(!p1Buttons.activeSelf);
        rotate1.GetComponent<Image>().enabled = isP1;

        p2Buttons.SetActive(!p2Buttons.activeSelf);
        rotate2.GetComponent<Image>().enabled = !isP1 ;
    }

    private void activateRotateView()
    {
        if (isP1) {
            rotate1.gameObject.SetActive(true);
            rotate2.gameObject.SetActive(true);
            rotate2.GetComponent<Image>().enabled = false;
        }
        else
        {
            rotate2.gameObject.SetActive(true);
            rotate1.gameObject.SetActive(true);
            rotate1.GetComponent<Image>().enabled = false;
        }
    }

    private void setMiddleButtons()
    {
        playButton.gameObject.SetActive(true);
        swapButton.gameObject.SetActive(true);
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


        resetHats();
        SceneManager.LoadScene("Lobby");
    }

    public void returnToLobby()
    {

        DisconnectAndWait();
    }

}
