using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour
{
    Rigidbody myRigid;
    public float UpSpeed = 0.3f;
    public float BackSpeed = 2f;

    int mySpeed = 0;
    bool isOnGround = false;
    bool isbeingShot = false;

    void Update()
    {
        if (isbeingShot)
        {
            if(transform.position.y > 4)
            {
                isbeingShot = false;
            }
        }
    }

    void Awake()
    {
        myRigid = GetComponent<Rigidbody>();
    }
    public void shootFunction(float Speed, Vector3 dir)
    {
        mySpeed = (int)Speed;
        myRigid.AddForce(Vector3.up * UpSpeed * (Speed + 3), ForceMode.Impulse);
        myRigid.AddForce(dir * -BackSpeed * (Speed + 3), ForceMode.Impulse);
        myRigid.AddTorque(Vector3.right * Speed, ForceMode.Impulse);
        isbeingShot = true;
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            GameManager.Instance.UpdateQuestValue(GameManager.Instance.Quest.MAXFAIL, 1);// 실패 횟수 증가
            Debug.Log("Falling object hit the ground! :");
            //GetComponent<BoxCollider>().enabled = false;
            //GetComponent<Rigidbody>().isKinematic = true;
            //버그 일으킴; 발사가 안됨
        }
    }
    void OnTriggerEnter(Collider other) {
        if (!isbeingShot && !isOnGround && other.gameObject.CompareTag("Pan"))
        {
            gameObject.SetActive(false);
            Debug.Log("Falling object caught by Pan! :" + mySpeed);
            GameManager.Instance.UpdateQuestValue(GameManager.Instance.Quest.TOTALSCORE, mySpeed);// 총 점수 증가
        if (GameManager.Instance.getQuestValue(GameManager.Instance.Quest.HIGHSCORE) < mySpeed)
            {
                GameManager.Instance.UpdateQuestValue(Quest.HIGHSCORE, mySpeed);// 최고 점수 변경
            }
        }
    }
}