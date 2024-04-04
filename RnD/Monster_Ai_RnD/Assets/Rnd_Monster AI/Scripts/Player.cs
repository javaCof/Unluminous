using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //레이캐스트를 맞은 몬스터를 담기위한 변수 
    public monsterCtrl mon = null;
    //플레이어 체력

    public float hp = 100.0f;
    //플레이어 공격력

    public float attackDmg = 33.5f;

    Ray ray;

    public RaycastHit hitInfo;

    //레이캐스트 거리
    public float maxRayDist = 30.0f;

    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //플레이어 ray 설정
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray 보기위한 디버그
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    public void Attack()
    {       //쏜 ray에 맞은것의 태그가 Enemy 일때
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            //mon 변수에 지금 ray에 닿은 monster를 참조
            mon = hitInfo.transform.GetComponent<monsterCtrl>();

            Debug.Log("공격!");

            //닿은 monster의 대미지 함수 실행
            mon.Damage(attackDmg);
        }
    }
}
