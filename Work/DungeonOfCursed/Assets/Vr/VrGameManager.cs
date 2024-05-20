using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VrGameManager : MonoBehaviour
{
    //�޴��������� vr
    [SerializeField]
    GameObject menuVr;

    [SerializeField]
    Canvas menuCanvas;

    [SerializeField]
    Button vrButton;
    
    [SerializeField]
    Button pcButton;

    [SerializeField]
    Camera pcCam;

    [SerializeField]
    Camera vrCam;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMenuVrMode()
    {
        //Pcī�޶� ��Ȱ��ȭ
        pcCam.gameObject.SetActive(false);
        //vr��ư ��Ȱ��ȭ
        vrButton.gameObject.SetActive(false);


        //pc��ư Ȱ��ȭ
        pcButton.gameObject.SetActive(true);
        //Vrī�޶� Ȱ��ȭ
        vrCam.gameObject.SetActive(true);


        //���� �ִ� Xr ������ Ȱ��ȭ
        menuVr.SetActive(true);

        //�޴� ĵ���� ���� �����̽��� ��ȯ
        menuCanvas.renderMode = RenderMode.WorldSpace;

        //ĵ���� rectƮ������ ������
        RectTransform rectTransform = menuCanvas.GetComponent<RectTransform>();
        //������ ����
        rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //��ġ ������ ���� ����
        Vector2 newPos = rectTransform.anchoredPosition;
        newPos.x = 0f;
        newPos.y = 2.5f;
        
        rectTransform.anchoredPosition = newPos;
    }

    public void ChangeMenuPcMode()
    {
        //Vrī�޶� ��Ȱ��ȭ
        vrCam.gameObject.SetActive(false);
        //pc��ư ��Ȱ��ȭ
        pcButton.gameObject.SetActive(false);

        //vr��ư Ȱ��ȭ
        vrButton.gameObject.SetActive(true);
        //Pcī�޶� Ȱ��ȭ
        pcCam.gameObject.SetActive(true);


        //���� �ִ� Xr ������ ��Ȱ��ȭ
        menuVr.SetActive(false);

        //�޴� ĵ���� ��ũ�������̽��� ��ȯ
        menuCanvas.renderMode = RenderMode.ScreenSpaceOverlay;


    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
