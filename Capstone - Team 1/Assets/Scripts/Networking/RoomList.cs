using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject RoomPrefab;
    public GameObject[] AllRooms;
    private List<RoomInfo> RoomListings = new List<RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        RoomListings.Clear();

        RoomListings.AddRange(roomList);

        AllRooms = new GameObject[RoomListings.Count];
        for (int i = 0; i < RoomListings.Count; i++)
        {
            if (RoomListings[i].IsOpen && RoomListings[i].IsVisible && RoomListings[i].PlayerCount == 1)
            {
                GameObject Room = Instantiate(RoomPrefab, new Vector3(155, -100 - (i * 60), 0), Quaternion.identity, GameObject.Find("Content").transform);
                Room.GetComponent<Room>().Name.text = RoomListings[i].Name;

                AllRooms[i] = Room;
            }
        }
    }
}