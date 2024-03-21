using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineCharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] private OnlineCharacterCustomization characterCustomization;
    [SerializeField] private PhotonView photonView;
    [SerializeField] private TextMeshProUGUI swapText;
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button p1Button;
    [SerializeField] private Button p2Button;
    [SerializeField] private GameObject p1Buttons;
    [SerializeField] private GameObject p2Buttons;
    public static Hat XHAT;
    public static Hat OHAT;
    private bool isP1 = false;
    private bool otherPlay;
    private bool mePlay;
    private bool otherSwap;
    private bool meSwap;

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
    }

    [PunRPC]
    public void switchSides()
    {
        //change to correct bools
        if (otherSwap && meSwap)
        {
            //change logic to accomplish necssary
            characterCustomization.switchSides();
            //change these two bools and text
            otherSwap = false;
            meSwap = false;
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

    public void onlineStart()
    {
        photonView.RPC("setOtherPlay", RpcTarget.Others);
        mePlay = !mePlay;
        if(mePlay)
        {
            swapButton.interactable = false;
        }
        else
        {
            swapButton.interactable = true;
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
        p1Buttons.SetActive(true);
        p2Button.gameObject.SetActive(false);
        p1Button.gameObject.SetActive(false);
        characterCustomization.seeP1();
    }

    [PunRPC]
    private void otherChooseP1()
    {
        characterCustomization.seeP1();
        p1Button.gameObject.SetActive(false);
    }

    public void onlineChooseP2()
    {
        meChooseP2();
        photonView.RPC("otherChooseP2", RpcTarget.Others);
    }

    private void meChooseP2()
    {
        isP1 = false;
        p2Buttons.SetActive(true);
        p1Button.gameObject.SetActive(false);
        p2Button.gameObject.SetActive(false);
        characterCustomization.seeP2();
    }

    [PunRPC]
    private void otherChooseP2()
    {
        characterCustomization.seeP2();
        p2Button.gameObject.SetActive(false);
    }

}
