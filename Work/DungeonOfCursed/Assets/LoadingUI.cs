using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Text loadingText;

    private void Update()
    {
        loadingText.text = FindObjectOfType<GameManager>().loadingText;
    }
}
