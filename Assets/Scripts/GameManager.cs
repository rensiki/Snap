using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    int stage = 0;
    bool isStage = false;
    double stageTimer = 0;
    int stageMaxTime = 60; // 스테이지 최대 시간 (초 단위)
    public enum Quest
    {
        TOTALSCORE,
        HIGHSCORE,
        MAXFAIL,
        MAXTIME
    }

    int[] questvalue = { 0, 0, 0, 0 }; // 퀘스트 값 초기화

    List<(Quest quest, int questValue)> stageQuests = new List<(Quest quest, int questValue)>();

    private void Awake()
    {
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
        StageTimer();
        if (Input.GetKeyDown(KeyCode.Space) && !isStage) // 스페이스바를 눌러서 스테이지 시작
        {
            InitializeStage();
        }
    }
    void InitializeStage()
    {
        isStage = true;
        stageQuests.Clear(); // Clear the list before adding new quests
        List<(Quest quest, int questValue)> initQuests = new List<(Quest quest, int questValue)>();
        if (stage == 0)
        {
            initQuests.Add((Quest.TOTALSCORE, 100)); // 예시 퀘스트 추가
            initQuests.Add((Quest.HIGHSCORE, 50)); // 예시 퀘스트 추가
        }
        else if (stage == 1)
        {
            initQuests.Add((Quest.MAXFAIL, 3)); // 예시 퀘스트 추가
            initQuests.Add((Quest.MAXTIME, 120)); // 예시 퀘스트 추가
        }
        else if (stage == 2)
        {
            initQuests.Add((Quest.TOTALSCORE, 200)); // 예시 퀘스트 추가
            initQuests.Add((Quest.HIGHSCORE, 100)); // 예시 퀘스트 추가
        }
        else if (stage == 3)
        {
            initQuests.Add((Quest.MAXFAIL, 5)); // 예시 퀘스트 추가
            initQuests.Add((Quest.MAXTIME, 180)); // 예시 퀘스트 추가
        }
        StartStage(initQuests);
    }

    void StartStage(List<(Quest quest, int questValue)> quests)
    {
        foreach (var (quest, questValue) in quests)
        {
            stageQuests.Add((quest, questValue));
            Debug.Log($"Quest: {quest}, Value: {questValue}");
            // canvas에 퀘스트 띄우기
        }

        //시간이 지나면 자동으로 스테이지를 마침
        //스테이지 종료시점까지 퀘스트를 전부 성공시 클리어 판정 출력.
        //클리어시 스테이지 값이 1 증가함
    }
    void StageTimer()
    {
        if (isStage)
        {
            stageTimer += (double)1*Time.deltaTime;
            UpdateQuestValue(Quest.MAXTIME, (int)stageTimer); // 스테이지 타이머 업데이트
            foreach (var (quest, questValue) in stageQuests)
            {
                Debug.Log($"Quest: {quest}, Value: {questValue}, progress: {questvalue[(int)quest]}");
                // canvas에 퀘스트 진행도 갱신
            }
            if (stageTimer >= stageMaxTime) // 1분후에 스테이지 종료
            {
                Debug.Log("Stage time is up!");
                isStage = false;
                stageTimer = 0;
                EndStage(); // 스테이지 정산
            }
        }
    }
    void EndStage()
    {
        // 스테이지 종료시점에 퀘스트를 전부 성공시 클리어 판정 출력.
        // 클리어시 스테이지 값이 1 증가함
        foreach (var (quest, questValue) in stageQuests)
        {
            if (questvalue[(int)quest] >= questValue)
            {
                Debug.Log($"Quest {quest} completed!");
                // canvas에 퀘스트 완료 표시
            }
            else
            {
                Debug.Log($"Quest {quest} failed.");
                Debug.Log("stage failed.");
                return;
                // canvas에 퀘스트 실패 표시
            }
        }
        Debug.Log($"Stage {stage} completed!");
        stage++;
        stageQuests.Clear(); // Clear the list for the next stage
    }

    public void UpdateQuestValue(Quest quest, int value)
    {
        if (quest == Quest.HIGHSCORE)
        {
            questvalue[(int)quest] = value;// 최고 점수는 더하지 않고 갱신함
        }
        else
        {
            questvalue[(int)quest] += value;
        }
        Debug.Log($"Quest {quest} progress updated to {questvalue[(int)quest]}");
    }

    public int getQuestValue(Quest quest)
    {
        return questvalue[(int)quest];
    }

    void InitializeMap()
    {
        // Initialize game state, load resources, etc.
        //실제로는 people은 매 스테이지마다 늘어나고 object는 일정 스테이지에서만 활성화됨. 
        if (stage == 0)
        {
            //people->Active = true; : 1
            //objects->Active = true; : 1
        }
        else if (stage == 1)
        {
            //people->Active = true; : 2
            //objects->Active = true; : 2
        }
        else if (stage == 2)
        {
            //people->Active = true; : 3
            //objects->Active = true; : 3
        }
        else if (stage == 3)
        {
            //people->Active = true; : 4
            //objects->Active = true; : 4
        }

    }
}
