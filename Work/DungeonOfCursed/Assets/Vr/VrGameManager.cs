using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrGameManager : MonoBehaviour
{
    //메뉴씬에서의 vr
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
        //씬에 있는 Xr 오리진 활성화
        menuVr.SetActive(true);

        //메뉴 캔버스 월드 스페이스로 전환
        menuCanvas.renderMode = RenderMode.WorldSpace;

        //캔버스 rect트랜스폼 가져옴
        RectTransform rectTransform = menuCanvas.GetComponent<RectTransform>();
        //스케일 조절
        rectTransform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //위치 조절을 위한 벡터
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
