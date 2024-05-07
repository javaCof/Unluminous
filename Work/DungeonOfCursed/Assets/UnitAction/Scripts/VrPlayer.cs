using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrPlayer : Player
{
    //���Ͱ� �´� ��ġ

    public Enemy enemy;
    public GameObject sword;




    private void Awake()
    {
        sword = GameObject.Find("swordColider");
        hitPoint = Vector3.zero;
        pv= GetComponent<PhotonView>();
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

       
    }

    public void VrAttackAction(Collision target)
    {
       

        //Ÿ���� null�� �ƴϰ� Ÿ���� �±װ� ���ʹ��� ���
        if (target == null && target.transform.tag == "Enemy")
        {
            //���ʹ� ������ �޾ƿ� ������ ���ʹ̸� �ִ´�
            enemy = target.gameObject.GetComponent<Enemy>();

            //���Ͱ� Į�̶� �΋H�� ��ġ
            hitPoint = target.transform.position;

            //���ʹ̰� ����������� ����Ʈ ����
            if (!enemy.isDead)
            { 
                MakeEffect(hitPoint); 
            }

            if (!PhotonNetwork.inRoom)
            {
                enemy.Hit_Master(stat.ATK);
            }
            //else if (pv.isMine)





        }
    }



    public override void AttackAction()
    {

    }
}
