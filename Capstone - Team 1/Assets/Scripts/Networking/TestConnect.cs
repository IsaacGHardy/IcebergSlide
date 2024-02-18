using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnect : MonoBehaviourPunCallbacks
{
    void Start()
    {
        print("Connecting to server.");
        PhotonNetwork.GameVersion = "0.0.1";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        print("Connected to server.");
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("You have joined the lobby.");
        int playersInLobby = PhotonNetwork.CountOfPlayersInRooms + PhotonNetwork.CountOfPlayersOnMaster;
        Debug.Log($"Players in the lobby: {playersInLobby}");
    }


}
