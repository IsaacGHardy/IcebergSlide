using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Photon.Pun;
using Photon.Realtime;
using System;
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
    [SerializeField] private GameObject selectText;
    [SerializeField] private TextMeshProUGUI otherSwapText;
    [SerializeField] private TMP_InputField p1Name;
    [SerializeField] private TMP_InputField p2Name;
    [SerializeField] private Button swapButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button p1Button;
    [SerializeField] private Button p2Button;
    [SerializeField] private GameObject rotate1;
    [SerializeField] private GameObject rotate2;
    [SerializeField] private GameObject p1NameEdit;
    [SerializeField] private GameObject p2NameEdit;
    [SerializeField] private GameObject p1ReadyButton;
    [SerializeField] private GameObject p2ReadyButton;
    [SerializeField] private GameObject p1Buttons;
    [SerializeField] private GameObject p2Buttons;
    public static Hat XHAT;
    public static Hat OHAT;
    public static bool isP1 = false;
    private bool meSelect = false;
    private bool otherPlay;
    private bool mePlay;
    private bool otherSwap;
    private bool meSwap;
    private Player[] playerList;
    private string myOldName = "";

    private void Awake()
    {
        playerList = PhotonNetwork.PlayerList;

        foreach (Player player in playerList)
        {
            string nickname = player.NickName;
            string localId = PhotonNetwork.LocalPlayer.UserId;
            if (player.UserId == localId && nickname != "")
            {
                myOldName = nickname;
            }
        }
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
        if(otherSwap && meSelect)
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
        removeListeners();
        string temp = p2Name.text;
        p2Name.text = p1Name.text;
        p1Name.text = temp;
        resetListeners();

    }

    private void removeListeners()
    {
        if(isP1)
        {
            p1Name.onValueChanged.RemoveAllListeners();
            p1Name.onEndEdit.RemoveAllListeners();
        }
        else
        {
            p2Name.onValueChanged.RemoveAllListeners();
            p2Name.onEndEdit.RemoveAllListeners();
        }
    }

    private void resetListeners()
    {
        if (isP1)
        {
            p1Name.onValueChanged.AddListener(onInputFieldChanged);
            p1Name.onEndEdit.AddListener(defaultName);
        }
        else
        {
            p2Name.onValueChanged.AddListener(onInputFieldChanged);
            p2Name.onEndEdit.AddListener(defaultName);
        }
    }

    public void SaveUsername()
    {
        PhotonNetwork.NickName = (isP1 ? p1Name.text : p2Name.text);
        PlayerPrefs.SetString("Username", (isP1 ? p1Name.text : p2Name.text));

        QuixoClass.p1Name = p1Name.text;
        QuixoClass.p2Name = p2Name.text;

    }

    public void onInputFieldChanged(string text)
    {
         photonView.RPC("syncInput", RpcTarget.Others, text);
    }

    [PunRPC]
    private void syncInput(string text)
    {
        //edit the other text
        if (isP1)
        {
            p2Name.text = text;
        }
        else
        {
            p1Name.text = text;
        }
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
            SaveUsername();
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
        photonView.RPC("otherChooseP1", RpcTarget.Others, p1Name.text);
    }

    private void meChooseP1()
    {
        isP1 = true;
        meSelect = true;
        if (otherSwap)
        {
            otherSwapText.gameObject.SetActive(true);
        }
        selectText.gameObject.SetActive(false);
        setMiddleButtons();
        p1Buttons.SetActive(true);
        activateRotateView();
        activateNameEditView();
        if(myOldName != "")
        {
            p1Name.text = myOldName;
        }
        p2Button.gameObject.SetActive(false);
        p1Button.gameObject.SetActive(false);
        characterCustomization.seeP1();
    }

    [PunRPC]
    private void otherChooseP1(string name)
    {
        characterCustomization.seeP1();
        p1Button.gameObject.SetActive(false);
        p1Name.gameObject.SetActive(true);
        p1Name.interactable = false;
        p1Name.text = name;


        rotate1.gameObject.SetActive(true);
        rotate1.GetComponent<Image>().enabled = false;
        foreach (Image image in rotate1.GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }
    }

    public void onlineChooseP2()
    {
        meChooseP2();
        photonView.RPC("otherChooseP2", RpcTarget.Others, p2Name.text);
    }

    private void meChooseP2()
    {
        isP1 = false;
        meSelect = true;
        if (otherSwap)
        {
            otherSwapText.gameObject.SetActive(true);
        }
        selectText.gameObject.SetActive(false);
        setMiddleButtons();
        p2Buttons.SetActive(true);
        activateRotateView();
        activateNameEditView();
        if (myOldName != "")
        {
            p2Name.text = myOldName;
        }
        p1Button.gameObject.SetActive(false);
        p2Button.gameObject.SetActive(false);
        characterCustomization.seeP2();
    }

    [PunRPC]
    private void otherChooseP2(string name)
    {
        isP1 = true;
        characterCustomization.seeP2();
        p2Button.gameObject.SetActive(false);
        p2Name.gameObject.SetActive(true);
        p2Name.interactable = false;
        p2Name.text = name;


        rotate2.gameObject.SetActive(true);
        rotate2.GetComponent<Image>().enabled = false;
        foreach (Image image in rotate2.GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }

    }

    private void swapCustomButtons()
    {
        p1Buttons.SetActive(!p1Buttons.activeSelf);
        rotate1.GetComponent<Image>().enabled = isP1;
        foreach (Image image in rotate1.GetComponentsInChildren<Image>())
        {
            image.enabled = isP1;
        }
        p1NameEdit.GetComponent<Image>().enabled = isP1;
        p1Name.interactable = isP1;

        p2Buttons.SetActive(!p2Buttons.activeSelf);
        rotate2.GetComponent<Image>().enabled = !isP1;
        foreach (Image image in rotate2.GetComponentsInChildren<Image>())
        {
            image.enabled = !isP1;
        }
        p2NameEdit.GetComponent<Image>().enabled = !isP1;
        p2Name.interactable = !isP1;

        if(isP1)
        {
            p1Name.onValueChanged.AddListener(onInputFieldChanged);
            p1Name.onEndEdit.AddListener(defaultName);
            p2Name.onValueChanged.RemoveAllListeners();
        }
        else
        {
            p2Name.onValueChanged.AddListener(onInputFieldChanged);
            p2Name.onEndEdit.AddListener(defaultName);
            p1Name.onValueChanged.RemoveAllListeners();
        }

    }

    private void activateNameEditView()
    {
        if (isP1) {
            p1Name.gameObject.SetActive(true);
            p1Name.onValueChanged.AddListener(onInputFieldChanged);
            p1Name.onEndEdit.AddListener(defaultName);
            p1NameEdit.gameObject.SetActive(true);

            p2NameEdit.gameObject.SetActive(true);
            p2NameEdit.GetComponent<Image>().enabled = false;
            p2Name.interactable = false;
        }
        else
        {
            p2Name.gameObject.SetActive(true);
            p2Name.onValueChanged.AddListener(onInputFieldChanged);
            p2Name.onEndEdit.AddListener(defaultName);
            p2NameEdit.gameObject.SetActive(true);

            p1NameEdit.gameObject.SetActive(true);
            p1NameEdit.GetComponent<Image>().enabled = false;
            p1Name.interactable = false;
        }
    }

    private void defaultName(string text)
    {
        if(text == "" || text == (isP1 ? p2Name.text : p1Name.text))
        {
            if(isP1)
            {
                p1Name.text = "Player 1";
                p1Name.onValueChanged.Invoke(p1Name.text);
            }
            else
            {
                p2Name.text = "Player 2";
                p2Name.onValueChanged.Invoke(p2Name.text);
            }
        }
    }

    private void activateRotateView()
    {
        if (isP1)
        {
            rotate1.gameObject.SetActive(true);
            rotate2.gameObject.SetActive(true);
            rotate2.GetComponent<Image>().enabled = false;
            foreach(Image image in rotate2.GetComponentsInChildren<Image>())
            {
                image.enabled = false;
            }
        }
        else
        {
            rotate2.gameObject.SetActive(true);
            rotate1.gameObject.SetActive(true);
            rotate1.GetComponent<Image>().enabled = false;
            foreach (Image image in rotate1.GetComponentsInChildren<Image>())
            {
                image.enabled = false;
            }

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

    private void OnDestroy()
    {
        if (p1Name != null) { 
            p1Name.onEndEdit.RemoveAllListeners();
            p1Name.onValueChanged.RemoveAllListeners();
        }
        if (p2Name != null) { 
            p2Name.onEndEdit.RemoveAllListeners();
            p2Name.onValueChanged.RemoveAllListeners();
        }
    }

}
