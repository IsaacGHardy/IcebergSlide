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
    new Camera camera;
    [SerializeField] private Vector3 gamePosition;
    [SerializeField] private Vector3 chatPosition;
    private float moveSpeed = 5.0f;



    private void Start()
    {
        inputField.onEndEdit.AddListener(delegate { checkEnterKey(inputField); });
        new Camera();
        camera = Camera.main;
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

    private IEnumerator moveCameraForChat(Vector3 position)
    {
        Vector3 curr = camera.transform.position;
        float distance = Vector3.Distance(curr, position);
        float duration = distance / moveSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            camera.transform.position = Vector3.Lerp(curr, position, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        camera.transform.position = position;
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
        if (chat.activeSelf) { 
            clickChatArea();
            StartCoroutine(moveCameraForChat(chatPosition));
        }
        else
        {
            StartCoroutine(moveCameraForChat(gamePosition));
        }
        notification.SetActive(false);
        scrollTobottom();
    }

    private void scrollTobottom()
    {
        Canvas.ForceUpdateCanvases();
        scrollArea.verticalNormalizedPosition = 0f;
    }


}