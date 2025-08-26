using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using DamageNumbersPro;

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

public enum ShopItem
{
    RandomMenu,
    Hat
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

    public float gameDurationTimer { get; private set; } = 0;

    [Header("Lists")]
    [SerializeField] private List<GameObject> Obstacles = new List<GameObject>();
    [SerializeField] private List<GameObject> menuBoard = new List<GameObject>(); // 오브젝트 틀//어짜피 이름으로 검색하는 기능밖에 아직 없으니까, 최적화 생각하면 dictionary로 바꾸는 것도 좋을 듯
    [SerializeField] private List<RefrigerItem> refrigerItems = new List<RefrigerItem>(); // 오브젝트 정보 저장용

    [SerializeField] int total_money = 0;

    [Header("Event Settings")]
    [SerializeField] int eventStartTime = 60;
    [SerializeField] int eventDelay = 10;
    [SerializeField] int eventBombMax_Y = 40;
    [SerializeField] int eventBombMin_Y = 30;
    [SerializeField] int bombPerEvent = 3;
    [SerializeField] int eventPhase = 120;

    [Header("Others")]
    [SerializeField] GameObject ground;
    [SerializeField] DamageNumber goldGetUI;
    [SerializeField] DamageNumber goldLoseUI;


    RefrigerItem friEggItem = new RefrigerItem("FriEgg", 0.1f, 0.3f, 1); // 기본 falling인 계란 후라이 정보
                                                                         // Falling의 Rigidbody 속성: 0.1, 0.5, 0.05

    private int eventCount = 0;


    private void Awake()
    {
        SpawnGoldText(transform.position, 0);

        //임시로 바로 로비로 오게 함
        add_money(0);
        UIManager.Instance.OnReturnToLobby();
        Debug.Log("gm awake->total_money:" + total_money);
        AddToRefriger("Bomb", 0.1f, 0.3f, 22);
        AddToRefriger("Bomb", 0.1f, 0.3f, 22);
        AddToRefriger("Chicken", 0.1f, 0.3f, 11);
        AddToRefriger("Chicken", 0.1f, 0.3f, 22);

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

    void Update()
    {
        gameDurationTimer += Time.deltaTime;
        UIManager.Instance.UpdateGameDurationUI(gameDurationTimer);
        GameEventTrigger();
    }

    void GameEventTrigger()
    {
        if (gameDurationTimer > eventPhase)
        {
            eventPhase += 60;
            eventPhaseUpdate();
        }
        if (gameDurationTimer > eventStartTime)
        {
            eventStartTime += eventDelay;
            Debug.Log("Game Event Triggered! at " + (int)gameDurationTimer + " seconds.");
            CreateRandomObject();
            eventCount++;
            if (bombPerEvent <= eventCount)
            {
                CreateEventBomb();
            }
        }
    }

    void eventPhaseUpdate()
    {
        if (eventDelay > 2)
        {
            eventDelay--;
        }
        if (bombPerEvent > 1)
        {
            bombPerEvent--;
        }
    }

    public void add_money(int money)
    {
        total_money += money;
        Debug.Log("Total Money: " + total_money);
        UIManager.Instance.UpdateMoneyUI(total_money);
    }
    public bool minus_money(int charge = 10)
    {
        if(total_money <= 0)
        {
            return false;
        }

        if (total_money - charge < 0)
        {
            total_money = 0;
        }
        else
        {
            total_money -= charge;
        }
        Debug.Log("Total Money: " + total_money);
        UIManager.Instance.UpdateMoneyUI(total_money);
        return true;
    }

    public void AddToRefriger(string objectName, float backSpeed, float upSpeed, int price)
    {
        RefrigerItem newItem = new RefrigerItem(objectName, backSpeed, upSpeed, price);
        refrigerItems.Add(newItem);
        Debug.Log($"Added to refrigerator: {objectName}, BackSpeed: {backSpeed}, UpSpeed: {upSpeed}, Price: {price}");
    }
    public void AddToRefriger(RefrigerItem item)
    {
        refrigerItems.Add(item);
        Debug.Log($"Added to refrigerator: {item.objectName}, BackSpeed: {item.backSpeed}, UpSpeed: {item.upSpeed}, Price: {item.price}");
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
                    return createMenu(friEggItem);//기본 falling인 계란 후라이 return해줌
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

    Vector3 getRandomPositionOnGround()
    {
        Collider groundCollider = ground.GetComponent<Collider>();
        // Ground의 바운드 내에서 랜덤 위치 생성
        Bounds bounds = groundCollider.bounds;

        float offset = 1;

        Vector3 defaultGround = new Vector3(
                Random.Range(bounds.min.x + offset, bounds.max.x - offset),
                bounds.max.y + 0.1f, // ground 위에 약간 띄워서 생성
                Random.Range(bounds.min.z + offset, bounds.max.z - offset)
            );
        return defaultGround;
    }

    public void CreateEventBomb()
    {
        Vector3 bombRandomPos = getRandomPositionOnGround() + new Vector3(0, Random.Range(eventBombMin_Y, eventBombMax_Y), 0);
        GameObject bomb = createMenu(CreateRefrigerItemByName("Bomb"));
        bomb.transform.position = bombRandomPos;
        bomb.SetActive(true);
    }

    public void CreateRandomObject()
    {
        // Ground의 바운드 내에서 랜덤 위치 생성 시도
        for (int i = 0; i < 10; i++)
        {
            Vector3 obstacleRandomPos = getRandomPositionOnGround();

            // 해당 위치에 다른 오브젝트가 있는지 체크 (ground 제외)
            Collider[] hits = Physics.OverlapSphere(obstacleRandomPos, 0.5f);
            bool canSpawn = true;
            foreach (var hit in hits)
            {
                if (hit.gameObject != ground)
                {
                    Debug.Log("Cannot spawn: " + hit.gameObject.name + " is already at the position.");
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn)
            {
                GameObject select = Obstacles[Random.Range(0, Obstacles.Count)];
                GameObject newObstacle = Instantiate(select, obstacleRandomPos, Quaternion.identity);
                Debug.Log("Created random object: " + newObstacle.name + " at position: " + obstacleRandomPos);
                return;
            }
        }
        Debug.Log("Failed to spawn object after 5 attempts.");
    }


    public void GameOver()
    {
        Debug.Log("Game Over!");

    }// GameManager.cs 내부에 추가
    public void StartGame()
    {
        // 게임 시작 로직
        Debug.Log("Game Started!");
        UIManager.Instance.OnGameStart();
    }

    public void EndGame()
    {
        // 게임 종료 로직
        Debug.Log("Game Ended!");
        UIManager.Instance.OnReturnToLobby();
    }

    public void BuyItem(ShopItem item)
    {
        // 상점 구매 로직
        Debug.Log("Bought item: " + item);
        switch (item)
        {
            case ShopItem.RandomMenu:
                string menuName = menuBoard[Random.Range(0, menuBoard.Count)].GetComponent<Falling>().FallingName;
                AddToRefriger(CreateRefrigerItemByName(menuName));
                Debug.Log("Bought Random Menu Item: " + menuName);
                break;
            case ShopItem.Hat:
                // 모자 구매 로직
                break;
        }
    }
    RefrigerItem CreateRefrigerItemByName(string itemName)
    {
        RefrigerItem item = new RefrigerItem(friEggItem.objectName, friEggItem.backSpeed, friEggItem.upSpeed, friEggItem.price); // 기본 속성 설정
        switch (itemName)
        {
            case "FriEgg":
                break;
            case "Chicken":
                item.objectName = "Chicken";
                item.price = 11;
                break;
            case "Bomb":
                item.objectName = "Bomb";
                item.upSpeed *= 0.5f; // Bomb의 UpSpeed를 절반으로 줄임
                item.price = 0;
                break;
        }
        return item;
    }

    public void SpawnGoldText(Vector3 position, int amount)
    {
        if (amount < 0)
        {
            goldLoseUI.Spawn(position + new Vector3(0, 1, 0), $"{amount}");
            return;
        }
        goldGetUI.Spawn(position + new Vector3(0, 1, 0), $"+{amount}");
    }
}
