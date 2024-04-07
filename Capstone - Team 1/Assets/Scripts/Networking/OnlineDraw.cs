using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnlineDraw : MonoBehaviour
{
    [SerializeField] PhotonView photonView;
    [SerializeField] GameObject drawScreen;
    [SerializeField] TextMeshProUGUI drawText;
    [SerializeField] QuixoClass quixoClass;
    [SerializeField] GameObject rejectMessage;
    [SerializeField] Button acceptBtn;
    [SerializeField] Button rejectBtn;


    private void setButtons(bool value)
    {
        acceptBtn.gameObject.SetActive(value);
        rejectBtn.gameObject.SetActive(value);
    }


    public void offerDraw()
    {
        photonView.RPC("offeredDraw", RpcTarget.Others);
        drawScreen.SetActive(true);
        setButtons(false);
        drawText.text = "Waiting on opponents response";
    }


    [PunRPC]
    private void offeredDraw()
    {
        drawScreen.SetActive(true);
        setButtons(true);
        drawText.text = "Your opponent would like to call the game a draw";
    }

    public void acceptDraw()
    {
        photonView.RPC("sendDrawResponse", RpcTarget.All, true);
    }

    public void rejectDraw()
    {
        photonView.RPC("sendDrawResponse", RpcTarget.Others, false);
        drawScreen.SetActive(false);
    }


    [PunRPC]
    private void sendDrawResponse(bool isAccepted)
    {
        if(isAccepted)
        {
            drawScreen.gameObject.SetActive(false);
            quixoClass.draw();
        }
        else
        {
            drawScreen.gameObject.SetActive(false);
            StartCoroutine(tellRejected());
        }
    }

    private IEnumerator tellRejected()
    {
        rejectMessage.SetActive(true);

        yield return new WaitForSeconds(5f);

        rejectMessage.SetActive(false);
    }


}
