using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMoneyManager : MonoBehaviour
{
    public Animator Menu; // main animator with menu ui

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OpenMoneyShop);
    }

    /// <summary>
    /// Updates money text in menu.
    /// </summary>
    /// <param name="money"></param>
    public void updateMoney(int money)
    {
        GetComponent<TMP_Text>().text = money + " <sprite anim=0,5,8>";
    }

    /// <summary>
    /// Opens money shop.
    /// </summary>
    public void OpenMoneyShop()
    {
        Menu.SetBool("OpenMoneyShop", true);
    }

    /// <summary>
    /// Closes money shop.
    /// </summary>
    public void CloseMoneyShop()
    {
        Menu.SetBool("OpenMoneyShop", false);
    }
}
