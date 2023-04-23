using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuMoneyManager : MonoBehaviour
{
    public void updateMoney(int money)
    {
        GetComponent<TMP_Text>().text = money + " <sprite anim=0,5,8>";
    }
}
