using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrGameManager : MonoBehaviour
{
    //�޴��������� vr
    [SerializeField]
    GameObject menuVr;

    [SerializeField]
    Canvas menuCanvas;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeVrMode()
    {
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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
