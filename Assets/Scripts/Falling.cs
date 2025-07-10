using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour
{
    Rigidbody myRigid;


    [SerializeField] private float UpSpeed = 0.3f;
    [SerializeField] private float BackSpeed = 2f;
    [SerializeField] private int price = 1;

    float mySpeed = 0;
    bool isOnGround = false;
    bool isbeingShot = false;
    //int refrigerCode;


    void Update()
    {
        if (isbeingShot)
        {
            if (transform.position.y > 4)
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
        mySpeed = Speed;
        SetPrice((int)mySpeed);
        Debug.Log("Falling object cur_price: " + price);
        myRigid.AddForce(Vector3.up * UpSpeed * (mySpeed + 3), ForceMode.Impulse);
        myRigid.AddForce(dir * -BackSpeed * (mySpeed + 3), ForceMode.Impulse);
        myRigid.AddTorque(Vector3.right * mySpeed, ForceMode.Impulse);
        isbeingShot = true;
        //test
        foreach (RefrigerItem item in GameManager.Instance.refrigerItems)
        {
            Debug.Log("Refrigerator contains: " + item.objectName + ", BackSpeed: " + item.backSpeed + ", UpSpeed: " + item.upSpeed + ", Price: " + item.price);
        }
    }
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            Debug.Log("Falling object hit the ground!");
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (!isbeingShot && !isOnGround && other.gameObject.CompareTag("Pan"))
        {
            Debug.Log("Falling object caught by Pan! :" + mySpeed);
            GameManager.Instance.add_money(price);
            
            // 오브젝트 정보를 저장하고 바로 삭제 (BackSpeed와 UpSpeed 전달)
            GameManager.Instance.AddToRefriger(gameObject.name, BackSpeed, UpSpeed, price);
            
            Destroy(gameObject);
        }
    }


    void SetPrice(int speed)
    {
        price = price * speed;
    }
}