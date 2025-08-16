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
    [SerializeField] private List<GameObject> menuBoard = new List<GameObject>(); // 오브젝트 틀//어짜피 이름으로 검색하는 기능밖에 아직 없으니까, 최적화 생각하면 dictionary로 바꾸는 것도 좋을 듯

    [SerializeField] private List<RefrigerItem> refrigerItems = new List<RefrigerItem>(); // 오브젝트 정보 저장용

    RefrigerItem friEggItem = new RefrigerItem("FriEgg", 0.06f, 0.3f, 1); // 기본 falling인 계란 후라이 정보


    private void Awake()
    {
        Debug.Log("gm awake->total_money:" + total_money);
        AddToRefriger("Bomb", 0.05f, 0.5f, 22);
        AddToRefriger("Bomb", 0.05f, 0.5f, 22);
        AddToRefriger("Chicken", 0.06f, 0.3f, 11);
        AddToRefriger("Chicken", 0.05f, 0.5f, 22);

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
    public bool minus_money(int charge = 10)
    {
        if(total_money - charge<0){ return false; }
        total_money -= charge;
        Debug.Log("Total Money: " + total_money);
        return true;
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
            return createMenu(friEggItem);//기본 falling인 계란 후라이 return해줌
        }
        Debug.Log("Refrigerator has items.");
        RefrigerItem item = refrigerItems[0];
        GameObject falling = createMenu(item); // 메뉴 생성
        if (falling == null)//생성 실패. 일반적으로는 발생 불가
        {
            Debug.LogError("Failed to create falling object for: " + item.objectName);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true; // 에디터에서 멈추게 함
#else
                    // 유저용 빌드에서는 fallback 전략
                    return friEgg;
#endif
        }
        refrigerItems.RemoveAt(0); // 사용한 item은 리스트에서 제거 (참조 해제)
        Debug.Log("Created falling object for: " + falling.GetComponent<Falling>().FallingName);
        return falling; //item 이름에 해당하는 비활성화된 상태의 falling반환. item의 속도, 가격 정보를 그대로 저장.
    }



    private GameObject createMenu(RefrigerItem order)//RefrigerItem의 이름과 같은 게임 오브젝트를 menu에서 찾아서 생성 및 반환
    {
        foreach (GameObject menu in menuBoard)
        {
            if (menu.GetComponent<Falling>().FallingName == order.objectName)
            {
                GameObject falling = Instantiate(menu, Vector3.zero, Quaternion.identity);
                falling.SetActive(false); // 메뉴를 비활성화 상태로 생성
                falling.GetComponent<Falling>().set_Values(order);
                Debug.Log("Created menu for: " + order.objectName);
                return falling;
            }
        }
        return null; // 만약 해당 오브젝트가 없으면 null 반환
    }

    public void CreateRandomObject()
    {
        
    }


    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
}
