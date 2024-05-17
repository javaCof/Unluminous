using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        yield return game.pdateLoadingText("�̱� �÷��� �ε�");
        yield return game.ChangeScene("GameScene");
    }

    public void OnMultiButtonClick()
    {
        StartCoroutine(MultiLoad());
    }
    private IEnumerator MultiLoad()
    {
        yield return game.StartLoading();
        yield return game.pdateLoadingText("��Ƽ �÷��� �ε�");
        yield return game.ChangeScene("LobbyScene");
    }

    public void OnExitButtonClick()
    {
        game.ExitGame(true);
    }
}
