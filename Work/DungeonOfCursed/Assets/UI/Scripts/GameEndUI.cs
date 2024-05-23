using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    public Text gameEndText;

    private void Start()
    {
        GameManager.Instance.curScene = "GameEndScene";

        if (GameManager.Instance.gameClear)
        {
            gameEndText.text = "Game Clear";
            gameEndText.color = Color.blue;
        }
        else
        {
            gameEndText.text = "Game End";
            gameEndText.color = Color.red;
        }

        Invoke("BackTo", 3);
    }

    public void BackTo()
    {
        string nextScene = PhotonNetwork.inRoom ? "RoomScene" : "MenuScene";
        StartCoroutine(GameManager.Instance.MoveToScene(nextScene));
    }
}
