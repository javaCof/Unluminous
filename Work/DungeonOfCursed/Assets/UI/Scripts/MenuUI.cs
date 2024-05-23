using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MenuUI : MonoBehaviour
{
    public PopupContent settingPopup;
    public Button vrBtn;

    private PopupUI popup;

    private void Awake()
    {
        popup = GetComponent<PopupUI>();
    }

    private void Start()
    {
        GameManager.Instance.curScene = gameObject.scene.name;
        
        vrBtn.GetComponentInChildren<Text>().text = GameManager.Instance.VrEnable ? "PC" : "VR";
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            popup.PopupOnOff(settingPopup);
            SoundManager.instance.PlaySfx("menu");
        }
    }

    public void OnSingleButtonClick()
    {
        StartCoroutine(SingleLoad());
        SoundManager.instance.PlaySfx("menu");
    }
    private IEnumerator SingleLoad()
    {
        yield return GameManager.Instance.StartLoading();
        yield return GameManager.Instance.ChangeScene(GameManager.Instance.curScene, "GameScene");
    }

    public void OnMultiButtonClick()
    {
        StartCoroutine(MultiLoad());
        SoundManager.instance.PlaySfx("menu");
    }
    private IEnumerator MultiLoad()
    {
        yield return GameManager.Instance.StartLoading();
        yield return GameManager.Instance.ChangeScene(GameManager.Instance.curScene, "LobbyScene");
    }

    public void OpenSetting()
    {
        popup.PopupOpen(settingPopup);
        SoundManager.instance.PlaySfx("menu");
    }

    public void SwitchVr()
    {

        GameManager.Instance.VrOnOff();
        vrBtn.GetComponentInChildren<Text>().text = GameManager.Instance.VrEnable ? "PC" : "VR";
        SoundManager.instance.PlaySfx("menu");
    }

    public void OnExitButtonClick()
    {
        GameManager.Instance.ExitGame(true);
        SoundManager.instance.PlaySfx("menu");
    }
}
