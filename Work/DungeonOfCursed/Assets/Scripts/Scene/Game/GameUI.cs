using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public GameObject leftJoyStick;
    public GameObject rightJoyStick;
    public GameObject actionButton;
    public GameObject jumpButton;

    private void Start()
    {
        leftJoyStick.SetActive(false);
        rightJoyStick.SetActive(false);
        actionButton.SetActive(false);
        jumpButton.SetActive(false);

#if UNITY_EDITOR || UNITY_ANDROID
        leftJoyStick.SetActive(true);
        rightJoyStick.SetActive(true);
        actionButton.SetActive(true);
        jumpButton.SetActive(true);
#endif
    }
}
