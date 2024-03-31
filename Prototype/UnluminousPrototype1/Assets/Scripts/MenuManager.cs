using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuManager : MonoBehaviour
{
    public void MoveToScene(SceneAsset scene)
    {
        SceneManager.LoadSceneAsync(scene.name);
    }

    public void ExitGame(bool askQuit)
    {
        if (!askQuit || EditorUtility.DisplayDialog("���� ����", "������ �����Ͻðڽ��ϱ�?", "��", "�ƴϿ�"))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }
    }
}
