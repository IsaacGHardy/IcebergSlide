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

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;
    public TMP_InputField input_Create;
    public TMP_InputField input_Join;
    [SerializeField] private Canvas connectinToServer;
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas waiting;
    private float waitTime = .5f;

    private void Awake()
    {
        lobby = this;
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
       
        PhotonNetwork.JoinLobby();
      
    }

    public override void OnJoinedLobby()
    {
        lobbyCanvas.gameObject.SetActive(true);
        Debug.Log("You have joined the lobby.");
        int playersInLobbby = PhotonNetwork.CountOfPlayersInRooms + PhotonNetwork.CountOfPlayersOnMaster;
        Debug.Log($"Players in the lobby: {playersInLobbby}");


    }

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
        lobbyCanvas.gameObject.SetActive(false);
        waiting.gameObject.SetActive(true);

        StartCoroutine(checkPlayerCount());

    }

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

        while (!isRoomFull())
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
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("OnlineGame");

    }


}