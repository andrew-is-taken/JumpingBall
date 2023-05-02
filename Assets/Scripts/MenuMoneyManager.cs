using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMoneyManager : MonoBehaviour
{
    public Animator Menu;
    public GameObject moneyShop;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenMoneyShop);
    }

    public void updateMoney(int money)
    {
        GetComponent<TMP_Text>().text = money + " <sprite anim=0,5,8>";
    }

    public void OpenMoneyShop()
    {
        Menu.SetBool("OpenMoneyShop", true);
    }

    public void CloseMoneyShop()
    {
        Menu.SetBool("OpenMoneyShop", false);
    }
}
