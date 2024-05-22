using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject leftJoyStick;
    public Button actionButton;
    public Button jumpButton;
    public TouchPad touchPad;
    public Image hpBar;

    private void Start()
    {
        leftJoyStick.SetActive(false);
        actionButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);
        touchPad.gameObject.SetActive(false);

#if UNITY_ANDROID
        leftJoyStick.SetActive(true);
        actionButton.gameObject.SetActive(true);
        jumpButton.gameObject.SetActive(true);
        touchPad.gameObject.SetActive(true);
#endif
    }
}
