using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RefrigerItem
{
    public string objectName;
    public float backSpeed;
    public float upSpeed;
    public int price;
    
    public RefrigerItem(string name, float backSpeed, float upSpeed, int price)
    {
        this.objectName = name;
        this.backSpeed = backSpeed;
        this.upSpeed = upSpeed;
        this.price = price;
    }
}

public class GameManager : MonoBehaviour
{

    //싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static GameManager _instance;//GameManager내부에서 자기 자신을 하나로 제한하고, 외부에 자기 자신을 전달하기 위한 변수인듯
    //인스턴스에 접근하기 위한 프로퍼티
    public static GameManager Instance //GameManager.Instance로 접근 가능
    {//+)static이라 이 클래스에 종속되지 않음
        get
        {
            //인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }



    [SerializeField] int total_money = 0;
    public List<RefrigerItem> refrigerItems = new List<RefrigerItem>(); // 오브젝트 정보 저장용
    [SerializeField] private GameObject friEgg;
    [SerializeField] private int refrigerIndex = 0;


    private void Awake()
    {
        Debug.Log(total_money);
        //------------------------싱글톤 패턴------------------------
        if (_instance == null)
        {
            _instance = this;
        }
        //인스턴스가 존재하는 경우 새로 생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        //아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);
    }

    public void add_money(int money)
    {
        total_money += money;
        Debug.Log("Total Money: " + total_money);
    }

    public void AddToRefriger(string objectName, float backSpeed, float upSpeed, int price)
    {
        RefrigerItem newItem = new RefrigerItem(objectName, backSpeed, upSpeed, price);
        refrigerItems.Add(newItem);
        Debug.Log($"Added to refrigerator: {objectName}, BackSpeed: {backSpeed}, UpSpeed: {upSpeed}, Price: {price}");
    }

    public GameObject get_refriger()
    {
        Debug.Log("냉장고 크기: " + refrigerItems.Count);
        if (refrigerItems.Count == 0)
        {
            Debug.Log("No items in the refrigerator.");
            return friEgg;//기본 falling인 계란 후라이 return해줌
        }
        Debug.Log("Refrigerator has items.");
        RefrigerItem item = refrigerItems[0];
        refrigerItems.RemoveAt(0); // 리스트에서 제거 (참조 해제)


        return friEgg; // 현재는 friEgg를 반환하지만, 실제로는 item 정보를 사용하여 다른 오브젝트를 반환해야함
    }

    // RefrigerItem 정리 메서드들
    public void ClearAllRefrigerItems()
    {
        refrigerItems.Clear(); // 모든 참조 해제
        Debug.Log("All refrigerator items cleared.");
    }

    public void RemoveRefrigerItem(string objectName)
    {
        for (int i = refrigerItems.Count - 1; i >= 0; i--)
        {
            if (refrigerItems[i].objectName == objectName)
            {
                refrigerItems.RemoveAt(i);
                Debug.Log($"Removed {objectName} from refrigerator.");
                break;
            }
        }
    }


    
}
