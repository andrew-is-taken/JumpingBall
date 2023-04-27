using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int id;

    public GameObject Tick;
    public GameObject Lock;

    private Shop shop;

    private void Awake()
    {
        shop = GetComponentInParent<Shop>();
        shop.shopItems.Add(this);
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        int equippedSkin = shop.GetEquippedSkin();
        if(equippedSkin != id)
        {
            if (shop.ClickedItemUnlocked(id))
            {
                shop.EquipSelectedSkin(id);
                Tick.SetActive(true);
            }
            else
            {
                print("Locked");
                shop.ClickedOnLockedSkin(id);
            }
        }
    }

    public void ChangeItemState(bool isOn)
    {
        Tick.SetActive(isOn);
    }

    public void UnlockItem()
    {
        Lock.SetActive(false);
    }
}
