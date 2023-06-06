using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public int id; // id of this item

    [SerializeField] private GameObject Tick; // tick if item is equipped
    [SerializeField] private GameObject Lock; // lock if item isn't purchased

    private Shop shop; // main shop script

    private void Awake()
    {
        shop = GetComponentInParent<Shop>();
        shop.shopItems.Add(this);
        GetComponent<Button>().onClick.AddListener(Click);
    }

    /// <summary>
    /// Equips skin or starts the purchase process.
    /// </summary>
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
                shop.ClickedOnLockedSkin(id);
            }
        }
    }

    /// <summary>
    /// Enables or disables the item's tick.
    /// </summary>
    /// <param name="isOn"></param>
    public void ChangeItemState(bool isOn)
    {
        Tick.SetActive(isOn);
    }

    /// <summary>
    /// Turns off the lock on item.
    /// </summary>
    public void UnlockItem()
    {
        Lock.SetActive(false);
    }
}
