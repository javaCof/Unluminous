using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public string nextScene;

    private bool now_loading;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneManager.LoadSceneAsync(nextScene);
    }

    public IEnumerator MoveToScene(string scene, float delay=0)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadSceneAsync(scene);
    }
    public IEnumerator ChangeScene(string scene_from, string scene_to)
    {
        AsyncOperation opLoad = SceneManager.LoadSceneAsync(scene_to, LoadSceneMode.Additive);
        AsyncOperation opUnload = SceneManager.UnloadSceneAsync(scene_from, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);

        yield return opLoad;
        yield return opUnload;
    }
    public IEnumerator StartLoading()
    {
        if (!now_loading)
        {
            now_loading = true;
            AsyncOperation op = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
            yield return op;
        }
        yield return null;
    }
    public IEnumerator EndLoading()
    {
        if (now_loading)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync("LoadingScene", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            yield return op;
            now_loading = false;
        }
        yield return null;
    }

    public void ErrorGame(ERROR_CODE code)
    {
        switch (code)
        {
            case ERROR_CODE.PHOTON_CONNECT_ERROR:
            case ERROR_CODE.PHOTON_CREATE_ROOM_ERROR:
            case ERROR_CODE.PHOTON_JOIN_ROOM_ERROR:
                StartCoroutine(MoveToScene("ErrorScene", 0.3f));
                break;
        }
    }
    public void ExitGame(bool askQuit)
    {
#if UNITY_EDITOR
        if (!askQuit || EditorUtility.DisplayDialog("���� ����", "������ �����Ͻðڽ��ϱ�?", "��", "�ƴϿ�"))
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}