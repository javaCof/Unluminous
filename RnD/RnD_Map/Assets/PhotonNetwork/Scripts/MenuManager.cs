using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    public void MoveToScene(string scene)
    {
        SceneManager.LoadSceneAsync(scene);
    }

    public void ExitGame(bool askQuit)
    {
#if UNITY_EDITOR
        if (!askQuit || EditorUtility.DisplayDialog("게임 종료", "게임을 종료하시겠습니까?", "예", "아니오"))
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
