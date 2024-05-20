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
    private void Start()
    {
        game.curScene = gameObject.scene.name;
    }

    public void OnSingleButtonClick()
    {
        StartCoroutine(SingleLoad());
    }
    private IEnumerator SingleLoad()
    {
        yield return game.StartLoading();
        yield return game.UpdateLoadingText("싱글 플레이 로드");
        yield return game.ChangeScene(game.curScene, "GameScene");
    }

    public void OnMultiButtonClick()
    {
        StartCoroutine(MultiLoad());
    }
    private IEnumerator MultiLoad()
    {
        yield return game.StartLoading();
        yield return game.UpdateLoadingText("멀티 플레이 로드");
        yield return game.ChangeScene(game.curScene, "LobbyScene");
    }

    public void OnExitButtonClick()
    {
        game.ExitGame(true);
    }
}
