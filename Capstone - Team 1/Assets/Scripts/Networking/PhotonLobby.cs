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
    public TMP_InputField nickname;
    [SerializeField] private Canvas connectinToServer;
    [SerializeField] private Canvas lobbyCanvas;
    [SerializeField] private Canvas waiting;
    [SerializeField] MenuSounds menuSounds;
    private float waitTime = .5f;

    private void Awake()
    {
        lobby = this;
        input_Create.onEndEdit.AddListener(delegate { checkEnterKeyCreate(input_Create); });
        input_Join.onEndEdit.AddListener(delegate { checkEnterKeyJoin(input_Join); });
    }
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void checkEnterKeyCreate(TMP_InputField input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            CreateRoom();
        }
    }

    private void checkEnterKeyJoin(TMP_InputField input)
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            JoinRoom();
        }
    }

    public override void OnConnectedToMaster()
    {
        menuSounds.playReady();
        PhotonNetwork.JoinLobby();

    }

    public override void OnJoinedLobby()
    {
        connectinToServer.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
        int playersInLobbby = PhotonNetwork.CountOfPlayersInRooms + PhotonNetwork.CountOfPlayersOnMaster;


    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(input_Create.text, new RoomOptions() { MaxPlayers = 2, IsVisible = true, IsOpen = true }, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
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
        lobbyCanvas.gameObject.SetActive(false);
        waiting.gameObject.SetActive(true);

        StartCoroutine(checkPlayerCount());
    }

    void UpdatePlayerCount()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected from server for reason " + cause.ToString());
        SceneManager.LoadScene("Main Menu");

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

        startGame();

    }

    void startGame()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel("OnlineTeamSelection");

    }

    public void backToMenu()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Main Menu");
    }


}
