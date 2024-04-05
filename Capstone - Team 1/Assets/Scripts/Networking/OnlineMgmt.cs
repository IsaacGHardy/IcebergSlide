using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnlineMgmt : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerLeftGame;
    [SerializeField] private GameObject drawRequest;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject otherPauseMenu;


    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        drawRequest.SetActive(false);
        pauseMenu.SetActive(false);
        otherPauseMenu.SetActive(false);
        playerLeftGame.gameObject.SetActive(true);
        StartCoroutine(waitReturnToLobby());
    }

    IEnumerator waitReturnToLobby()
    {
        yield return new WaitForSeconds(10f);

        returnToLobby();
    }

    public void DisconnectAndWait()
    {
        Time.timeScale = 1.0f;
        PhotonNetwork.Disconnect();
        StartCoroutine(WaitForDisconnect());

    }

    private IEnumerator WaitForDisconnect()
    {
        while(PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        OnlineCharacterCustomizationUI.resetHats();
        SceneManager.LoadScene("Lobby");
    }

    public void returnToLobby()
    {
        DisconnectAndWait();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
