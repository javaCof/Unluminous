using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuUI : MonoBehaviour
{
    public Button vrBtn;

    private GameManager game;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
    }
    private void Start()
    {
        game.curScene = gameObject.scene.name;
        
        vrBtn.GetComponentInChildren<Text>().text = game.vrEnable ? "PC" : "VR";
    }

    public void OnSingleButtonClick()
    {
        StartCoroutine(SingleLoad());
        SoundManager.instance.PlaySfx("menu");
    }
    private IEnumerator SingleLoad()
    {
        yield return game.StartLoading();
        yield return game.UpdateLoadingText("�̱� �÷��� �ε�");
        yield return game.ChangeScene(game.curScene, "GameScene");
    }

    public void OnMultiButtonClick()
    {
        StartCoroutine(MultiLoad());
        SoundManager.instance.PlaySfx("menu");
    }
    private IEnumerator MultiLoad()
    {
        yield return game.StartLoading();
        yield return game.UpdateLoadingText("��Ƽ �÷��� �ε�");
        yield return game.ChangeScene(game.curScene, "LobbyScene");
    }

    public void SwitchVr()
    {

        game.VrOnOff();
        vrBtn.GetComponentInChildren<Text>().text = game.vrEnable ? "PC" : "VR";
        SoundManager.instance.PlaySfx("menu");
    }

    public void OnExitButtonClick()
    {
        game.ExitGame(true);
        SoundManager.instance.PlaySfx("menu");
    }
}
