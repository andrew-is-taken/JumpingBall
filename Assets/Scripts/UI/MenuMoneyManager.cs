using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuMoneyManager : MonoBehaviour
{
    private Animator MenuAnim; // main animator with menu ui

    private void Start()
    {
        MenuAnim = Menu.instance.GetComponent<Animator>();
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
        MenuAnim.SetBool("OpenMoneyShop", true);
    }

    /// <summary>
    /// Closes money shop.
    /// </summary>
    public void CloseMoneyShop()
    {
        MenuAnim.SetBool("OpenMoneyShop", false);
    }
}
