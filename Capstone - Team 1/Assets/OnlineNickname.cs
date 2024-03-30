using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class OnlineNickname : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] Button button;
    [SerializeField] PhotonView photonView;

    private void Start()
    {
        button.onClick.AddListener(changeNickname);
    }

    private void changeNickname()
    {
        input.Select();
        input.ActivateInputField();
    }


}
