using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    public string nextScene;
    [HideInInspector] public string curScene;

    [TextArea(0, 1000)] public string ToDo;

    private bool now_loading;

    private float _InputSensitivity;
    private float _BgmVolume;
    private float _SfxVolume;

    public float InputSensitivity { get { return _InputSensitivity; } set { _InputSensitivity = value; PlayerPrefs.SetFloat("INPUT_SENSITIVITY", value); } }
    public float BgmVolume { get { return _BgmVolume; } set { _BgmVolume = value; PlayerPrefs.SetFloat("BGM_VOLUME", value); } }
    public float SfxVolume { get { return _SfxVolume; } set { _SfxVolume = value; PlayerPrefs.SetFloat("SFX_VOLUME", value); } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    IEnumerator Start()
    {
        curScene = SceneManager.GetActiveScene().name;

        yield return StartLoading();

        InputSensitivity = PlayerPrefs.GetFloat("INPUT_SENSITIVITY", 0.5f);
        BgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1f);
        SfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1f);

        /*DB Load*/
        System.Threading.Tasks.Task task;
        {
            yield return UpdateLoadingText("몬스터 정보를 불러오는 중...");
            task = FirebaseManager.UnitLoadData();
            yield return new WaitUntil(() => task.IsCompleted);

            yield return UpdateLoadingText("아이템 정보를 불러오는 중...");
            task = FirebaseManager.ItemLoadData();
            yield return new WaitUntil(() => task.IsCompleted);

            yield return UpdateLoadingText("장비 정보를 불러오는 중...");
            task = FirebaseManager.EquipLoadData();
            yield return new WaitUntil(() => task.IsCompleted);
        }

        yield return ChangeScene(curScene, nextScene);
        yield return EndLoading();
    }

    public IEnumerator MoveToScene(string scene)
    {
        yield return SceneManager.LoadSceneAsync(scene);
    }
    public IEnumerator ChangeScene(string from_scene, string to_scene)
    {
        AsyncOperation opLoad = SceneManager.LoadSceneAsync(to_scene, LoadSceneMode.Additive);
        AsyncOperation opUnload = SceneManager.UnloadSceneAsync(from_scene);

        yield return opLoad;
        yield return opUnload;
    }
    public void LoadingScene(string scene)
    {
        StartCoroutine(_LoadingScene(scene));
    }
    private IEnumerator _LoadingScene(string scene)
    {
        yield return StartLoading();
        yield return ChangeScene(curScene, scene);
        yield return EndLoading();
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
            yield return UpdateLoadingText("");
            AsyncOperation op = SceneManager.UnloadSceneAsync("LoadingScene", UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
            yield return op;
            now_loading = false;
        }
        yield return null;
    }

    public IEnumerator UpdateLoadingText(string msg)
    {
        LoadingUI loadingUI;
        if (loadingUI = FindObjectOfType<LoadingUI>())
        {
            loadingUI.loadingText.text = msg;
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
                StartCoroutine(MoveToScene("ErrorScene"));
                break;
        }
    }
    public void ExitGame(bool askQuit)
    {
#if UNITY_EDITOR
        if (!askQuit || EditorUtility.DisplayDialog("게임 종료", "게임을 종료하시겠습니까?", "네", "아니요"))
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
