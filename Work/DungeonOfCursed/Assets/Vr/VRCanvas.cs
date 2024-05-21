using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class VRCanvas : MonoBehaviour
{
    public Camera uiCam;
    public GameObject vrInteracter;

    private GameManager game;

    private void Awake()
    {
        game = FindObjectOfType<GameManager>();
    }

    private void Start()
    {
        game.VrOnOff(game.vrEnable);
    }
}
