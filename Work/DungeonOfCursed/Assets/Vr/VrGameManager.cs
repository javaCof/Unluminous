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
        menuVr.SetActive(true);

        menuCanvas.renderMode = RenderMode.WorldSpace;



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
