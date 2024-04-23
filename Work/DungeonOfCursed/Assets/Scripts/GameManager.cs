using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public string firstScene;
    public string loadingScene;
    public string errorScene;

    private bool now_loading;
    
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneManager.LoadSceneAsync(firstScene);
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
            AsyncOperation op = SceneManager.LoadSceneAsync(loadingScene, LoadSceneMode.Additive);
            yield return op;
        }
        yield return null;
    }
    public IEnumerator EndLoading()
    {
        if (now_loading)
        {
            AsyncOperation op = SceneManager.UnloadSceneAsync(loadingScene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
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
                StartCoroutine(MoveToScene(errorScene, 0.3f));
                break;
        }
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
