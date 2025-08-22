using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum UIType
{
    Lobby,
    Shop,
    InGame
}
public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(UIManager)) as UIManager;
                if (_instance == null)
                    Debug.Log("no Singleton UIManager obj");
            }
            return _instance;
        }
    }

    [SerializeField] private LobbyUI lobbyUI;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private InGameUI inGameUI;

    [Header("공통 UI")]
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private TextMeshProUGUI gameDurationText;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ShowUI(UIType type)
    {
        lobbyUI.gameObject.SetActive(type == UIType.Lobby);
        shopUI.gameObject.SetActive(type == UIType.Shop);
        inGameUI.gameObject.SetActive(type == UIType.InGame);
    }

    public void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = "Money: " + money.ToString();
    }

    // 예시: GameManager에서 호출
    public void OnGameStart()
    {
        gameDurationText.gameObject.SetActive(true);
        moneyText.gameObject.SetActive(true);
        ShowUI(UIType.InGame);
    }

    public void OnReturnToLobby()
    {
        gameDurationText.gameObject.SetActive(false);
        moneyText.gameObject.SetActive(false);
        ShowUI(UIType.Lobby);
    }

    public void OnOpenShop()
    {
        gameDurationText.gameObject.SetActive(false);
        moneyText.gameObject.SetActive(true);
        ShowUI(UIType.Shop);
    }

    public void UpdateGameDurationUI(float duration)
    {
        if (gameDurationText != null)
            gameDurationText.text =  duration.ToString("F0") + "s";
    }

}
public class UISystem : MonoBehaviour
{
    
}