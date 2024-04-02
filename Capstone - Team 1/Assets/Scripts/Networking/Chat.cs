using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using Unity.VisualScripting;

public class Chat : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject Message;
    public GameObject Content;
    [SerializeField] private float minHeight = 30f;

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

        GameObject M = Instantiate(Message, GameObject.Find("Content").transform.position, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().MyMessage.text = ReceiveMessage;
    }


}