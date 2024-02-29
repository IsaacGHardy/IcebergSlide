using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OnlineCharacterCustomizationUI : MonoBehaviour
{
    [SerializeField] private OnlineCharacterCustomization characterCustomization;
    [SerializeField] private PhotonView photonView;
    public static Hat XHAT;
    public static Hat OHAT;

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
        photonView.RPC("switchSides", RpcTarget.All);
    }

    [PunRPC]
    public void switchSides()
    {
        characterCustomization.switchSides();
    }

    public void onlineStart()
    {
        photonView.RPC("startGame", RpcTarget.All);
    }

    [PunRPC]
    public void startGame()
    {
        XHAT = characterCustomization.p1Hat;
        OHAT = characterCustomization.p2Hat;
        SceneManager.LoadScene("OnlineGame");
    }


    public void p1Rand()
    {
        characterCustomization.setRandomHat(true);
    }

    public void p2Rand()
    {
        characterCustomization.setRandomHat(false);
    }

}
