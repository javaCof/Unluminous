using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public GameObject leftJoyStick;
    public GameObject rightJoyStick;
    public Button actionButton;
    public Button jumpButton;

    private void Start()
    {
        leftJoyStick.SetActive(false);
        rightJoyStick.SetActive(false);
        actionButton.gameObject.SetActive(false);
        jumpButton.gameObject.SetActive(false);

#if !UNITY_EDITOR && UNITY_ANDROID
        leftJoyStick.SetActive(true);
        rightJoyStick.SetActive(true);
        actionButton.gameObject.SetActive(true);
        jumpButton.gameObject.SetActive(true);
#endif
    }
}
