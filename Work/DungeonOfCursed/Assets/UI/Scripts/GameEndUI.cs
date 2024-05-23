using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.curScene = "GameEndScene";
        Invoke("BackTo", 3);
    }

    public void BackTo()
    {
        string nextScene = PhotonNetwork.inRoom ? "RoomScene" : "MenuScene";
        StartCoroutine(GameManager.Instance.MoveToScene(nextScene));
    }
}
