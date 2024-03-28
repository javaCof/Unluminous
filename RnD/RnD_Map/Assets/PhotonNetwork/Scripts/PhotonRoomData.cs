using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonRoomData : MonoBehaviour
{
    public Text textRoomName;
    public Text textConnectInfo;

    [HideInInspector] public string roomName = "";
    [HideInInspector] public int connectPlayer = 0;
    [HideInInspector] public int maxPlayers = 0;

    public void UpdateRoomData()
    {
        textRoomName.text = roomName;
        textConnectInfo.text = "(" + connectPlayer.ToString() + "/" + maxPlayers.ToString() + ")";
    }
}
