using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomList : MonoBehaviourPunCallbacks
{
    public GameObject RoomPrefab;
    public GameObject[] AllRooms;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        for (int i = 0; i < AllRooms.Length; i++)
        {
            if (AllRooms[i] != null)
            {
                Destroy(AllRooms[i]);
            }
        }
        AllRooms = new GameObject[roomList.Count];
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].IsOpen && roomList[i].IsVisible && roomList[i].PlayerCount == 1)
            {
                GameObject Room = Instantiate(RoomPrefab, new Vector3(155, -100 - (i * 60), 0), Quaternion.identity, GameObject.Find("Content").transform);
                Room.GetComponent<Room>().Name.text = roomList[i].Name;

                AllRooms[i] = Room;
            }
        }
    }
}