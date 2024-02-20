//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;
//using UnityEngine.UI;
//using TMPro;

//public class Toggler : MonoBehaviour
//{
//    public GameObject Chat;
//    private GameObject chatWindowInstance;

//    private bool isChatOpen = false;

//    private void Start()
//    {
//        chatWindowInstance = Instantiate(Chat, transform.position, Quaternion.identity);
//        chatWindowInstance.SetActive(false);
//    }

//    public void ToggleChatWindow()
//    {
//        isChatOpen = !isChatOpen;
//        chatWindowInstance.SetActive(isChatOpen);
//    }
//}
