using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Chat : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject Message;
    public GameObject Content;
    [SerializeField] private float minHeight = 30f;
    [SerializeField] GameObject chat;
    [SerializeField] GameObject notification;
    [SerializeField] ScrollRect scrollArea;


    private void Start()
    {
        inputField.onEndEdit.AddListener(delegate { checkEnterKey(inputField); });
    }

    public void SendMessage()
    {
        if (inputField.text != "")
        {
            GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, (PhotonNetwork.NickName + ": " + inputField.text));
        }
        inputField.text = "";
        clickChatArea();
        scrollTobottom();
    }
    
    private void checkEnterKey(TMP_InputField input)
    {
        if(Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SendMessage();
        }
    }

    public void clickChatArea()
    {
        inputField.Select();
        inputField.ActivateInputField();
    }


    [PunRPC]
    public void GetMessage(string ReceiveMessage)
    {
        if(!chat.activeSelf)
        {
            notification.SetActive(true);
        }
        GameObject M = Instantiate(Message, Content.transform.position, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().MyMessage.text = ReceiveMessage;
        scrollTobottom();
    }

    public void toggleChat()
    {
        chat.SetActive(!chat.activeSelf);
        if (chat.activeSelf) { clickChatArea(); }
        notification.SetActive(false);
        scrollTobottom();
    }

    private void scrollTobottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollArea.verticalNormalizedPosition = 0f;
    }


}