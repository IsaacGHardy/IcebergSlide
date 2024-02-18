using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Chat : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject Message;
    public GameObject Content;

    public void SendMessage()
    {

        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, (PhotonNetwork.NickName + " : " + inputField.text));
        print(PhotonNetwork.NickName + " : " + inputField.text);

        inputField.text = "";
    }


    [PunRPC]
    public void GetMessage(string ReceiveMessage)

    {

        GameObject M = Instantiate(Message, GameObject.Find("Content").transform.position, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().MyMessage.text = ReceiveMessage;
        Debug.Log("Position of Content: " + Content.transform.localPosition);
        Debug.Log("Position of instantiated Message: " + M.transform.localPosition);

    }


}