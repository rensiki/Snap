using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShopUI : UISystem
{

    public void BuyRandomMenu()
    {
        OnBuyItem(ShopItem.RandomMenu);
    }
    public void BuyHat()
    {
        OnBuyItem(ShopItem.Hat);
    }

    public void OnBuyItem(ShopItem item)
    {
        GameManager.Instance.BuyItem(item);
    }

    public void OnBackButton()
    {
        UIManager.Instance.OnReturnToLobby();
    }
}
