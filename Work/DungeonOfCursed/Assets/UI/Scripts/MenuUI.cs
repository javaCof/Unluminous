using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuUI : MonoBehaviour
{
    private GameManager game;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
    }

    public void OnSingleButtonClick()
    {
        StartCoroutine(SingleLoad());
    }
    private IEnumerator SingleLoad()
    {
        yield return game.StartLoading();
        yield return game.ChangeScene("MenuScene", "GameScene");
    }

    public void OnMultiButtonClick()
    {
        StartCoroutine(MultiLoad());
    }
    private IEnumerator MultiLoad()
    {
        yield return game.StartLoading();
        yield return game.ChangeScene("MenuScene", "LobbyScene");
    }

    public void OnExitButtonClick()
    {
        game.ExitGame(true);
    }
}
