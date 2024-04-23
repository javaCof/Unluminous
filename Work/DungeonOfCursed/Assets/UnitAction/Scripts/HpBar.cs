using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    public Image hpBar;
    public PlayerAction playerAction;
    public float hp;

    private void Awake()
    {
        hpBar = transform.GetChild(1).GetComponent<Image>();
        playerAction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAction>();
        hp = playerAction.curHP;
    }

    public void HpChanged()
    {
        hp = playerAction.curHP;
        hpBar.fillAmount = hp/1000;
    }

}
