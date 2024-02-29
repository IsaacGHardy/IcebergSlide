using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    public TMP_InputField input_Create;
    public TMP_InputField input_Join;
    [SerializeField] private Canvas lobby;
    [SerializeField] private Canvas waiting;
    private float waitTime = .5f;

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(input_Create.text, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("You have created a new room!");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(input_Join.text);
    }
    public void JoinRoomInList(string RoomName)
    {
        PhotonNetwork.JoinRoom(RoomName);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"You have joined the room '{PhotonNetwork.CurrentRoom.Name}'");
        Debug.Log($"Players in this room: {PhotonNetwork.CurrentRoom.PlayerCount}");
        lobby.gameObject.SetActive( false );
        waiting.gameObject.SetActive( true );

        StartCoroutine(checkPlayerCount());

    }


    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //    {
    //        Debug.Log($"A new player has joined the room '{PhotonNetwork.CurrentRoom.Name}'!");
    //        UpdatePlayerCount();
    //    }

    //    public override void OnPlayerLeftRoom(Player otherPlayer)
    //    {
    //        Debug.Log($"A player has left the room '{PhotonNetwork.CurrentRoom.Name}'!");
    //        UpdatePlayerCount();
    //    }

    void UpdatePlayerCount()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        Debug.Log($"Players in room '{PhotonNetwork.CurrentRoom.Name}': {playerCount}");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server for reason " + cause.ToString());
    }


    bool isRoomFull()
    {
        return PhotonNetwork.CurrentRoom.PlayerCount == 2;
    }

    IEnumerator checkPlayerCount()
    {

        while(!isRoomFull())
        {
            yield return new WaitForSeconds(waitTime);
        }

        setPlayerTeams();
        startGame();

    }

    void setPlayerTeams()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("Xteam"))
        {
           PhotonNetwork.LocalPlayer.CustomProperties["Oteam"] = PhotonNetwork.LocalPlayer.UserId;

        }
        else
        {
            PhotonNetwork.LocalPlayer.CustomProperties["Xteam"] = PhotonNetwork.LocalPlayer.UserId;
        }
      
    }


    void startGame()
    {
        PhotonNetwork.LoadLevel("OnlineTeamSelection");

    }



}