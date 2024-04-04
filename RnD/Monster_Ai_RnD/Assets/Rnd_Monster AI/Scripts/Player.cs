using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //����ĳ��Ʈ�� ���� ���͸� ������� ���� 
    public monsterCtrl mon = null;
    //�÷��̾� ü��

    public float hp = 100.0f;
    //�÷��̾� ���ݷ�

    public float attackDmg = 33.5f;

    Ray ray;

    public RaycastHit hitInfo;

    //����ĳ��Ʈ �Ÿ�
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
        //�÷��̾� ray ����
        ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        //ray �������� �����
        Debug.DrawRay(ray.origin, ray.direction * maxRayDist, Color.red);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    public void Attack()
    {       //�� ray�� �������� �±װ� Enemy �϶�
        if (Physics.Raycast(ray, out hitInfo, maxRayDist) && hitInfo.transform.tag == "Enemy")
        {
            //mon ������ ���� ray�� ���� monster�� ����
            mon = hitInfo.transform.GetComponent<monsterCtrl>();

            Debug.Log("����!");

            //���� monster�� ����� �Լ� ����
            mon.Damage(attackDmg);
        }
    }
}
