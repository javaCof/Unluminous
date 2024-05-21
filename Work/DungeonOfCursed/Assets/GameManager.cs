using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.XR;
using UnityEngine.XR.Management;

public class GameManager : MonoBehaviour
{
    public string nextScene;
    public string curScene;

    private bool now_loading;

    private float _InputSensitivity;
    private float _BgmVolume;
    private float _SfxVolume;

    public bool vrEnable;// { get; set; }

    public float InputSensitivity { get { return _InputSensitivity; } set { _InputSensitivity = value; PlayerPrefs.SetFloat("INPUT_SENSITIVITY", value); } }
    public float BgmVolume { get { return _BgmVolume; } set { _BgmVolume = value; PlayerPrefs.SetFloat("BGM_VOLUME", value); } }
    public float SfxVolume { get { return _SfxVolume; } set { _SfxVolume = value; PlayerPrefs.SetFloat("SFX_VOLUME", value); } }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Firebase.Database.FirebaseDatabase.DefaultInstance.GoOffline();
    }
    IEnumerator Start()
    {
        curScene = SceneManager.GetActiveScene().name;

        Firebase.Database.FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        Firebase.Database.FirebaseDatabase.DefaultInstance.GoOnline();

        yield return StartLoading();

        if (XRSettings.enabled)
        {
            yield return new WaitUntil(() => XRGeneralSettings.Instance.Manager.isInitializationComplete);
        }

        vrEnable = XRSettings.enabled;

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
        yield return SceneManager.LoadSceneAsync(to_scene, LoadSceneMode.Additive);
        yield return SceneManager.UnloadSceneAsync(from_scene, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
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

    [ContextMenu("onoff vr")]
    public void VrOnOff() => VrOnOff(!vrEnable);
    public void VrOnOff(bool on)
    {
        vrEnable = on;

        UpdateVRUI();

        if (vrEnable) XRGeneralSettings.Instance.Manager.StartSubsystems();
        else XRGeneralSettings.Instance.Manager.StopSubsystems();
    }
    public void UpdateVRUI()
    {
        if (vrEnable)
        {
            foreach (var vrCanvas in FindObjectsByType<VRCanvas>(FindObjectsSortMode.None))
            {
                if (vrCanvas)
                {
                    Canvas canvas = vrCanvas.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.WorldSpace;
                    canvas.worldCamera = vrCanvas.vrInteracter.GetComponentInChildren<Camera>();

                    RectTransform rect = canvas.GetComponent<RectTransform>();
                    rect.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    rect.anchoredPosition = new Vector2(0, 2.5f);

                    Camera uiCam = vrCanvas.uiCam;
                    if (uiCam) uiCam.gameObject.SetActive(false);

                    vrCanvas.vrInteracter.SetActive(true);
                }
            }
        }
        else
        {
            foreach (var vrCanvas in FindObjectsByType<VRCanvas>(FindObjectsSortMode.None))
            {
                if (vrCanvas)
                {
                    Canvas canvas = vrCanvas.GetComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                    Camera uiCam = vrCanvas.uiCam;
                    if (uiCam) uiCam.gameObject.SetActive(true);

                    GameObject vrInter = vrCanvas.vrInteracter;
                    if (vrInter) vrInter.SetActive(false);
                }
            }
        }
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
