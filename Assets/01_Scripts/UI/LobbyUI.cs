using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LobbyUI : UISystem
{
    public void OnStartGameButton()
    {
        GameManager.Instance.StartGame();
        UIManager.Instance.OnGameStart();
    }

    public void OnShopButton()
    {
        UIManager.Instance.OnOpenShop();
    }
}
