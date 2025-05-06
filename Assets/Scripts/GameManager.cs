using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    int stage = 0;
    bool isStage = false;
    double stageTimer = 0;
    int stageMaxTime = 60; // 스테이지 최대 시간 (초 단위)
    enum Quest
    {
        TOTALSCORE,
        HIGHSCORE,
        MAXFAIL,
        MAXTIME
    }

    int[] questvalue = { 0, 0, 0, 0 }; // 퀘스트 값 초기화

    List<(Quest quest, int questValue)> stageQuests = new List<(Quest quest, int questValue)>();

    void Update()
    {
        StageTimer();
        if(Input.GetKeyDown(KeyCode.Space)&&!isStage) // 스페이스바를 눌러서 스테이지 시작
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

    void Init()
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
