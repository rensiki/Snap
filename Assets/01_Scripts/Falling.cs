using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour
{
    Rigidbody myRigid;


    [SerializeField] private string falling_name = "init"; // Falling 오브젝트의 이름을 저장
    public string FallingName { get { return falling_name; } } // 외부에서 이름을 읽을 수 있도록 프로퍼티로 제공

    // Falling 오브젝트의 속성. GameManager에서 스크립트상에서만 관리함.
    private float UpSpeed;
    private float BackSpeed;
    private int price;


    float mySpeed = 0;
    protected bool isOnGround = false;
    protected bool isbeingShot = false;
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
        myRigid.AddForce(Vector3.up * UpSpeed * (mySpeed + 2), ForceMode.Impulse);
        myRigid.AddForce(dir * -BackSpeed * (mySpeed + 1), ForceMode.Impulse);

        Vector3 torqueAxis = new Vector3(dir.z, 0, -dir.x).normalized;
        myRigid.AddTorque(torqueAxis * (mySpeed + 1), ForceMode.Impulse);

        isbeingShot = true;
    }
    public virtual void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            //Debug.Log("Falling object hit the ground!");
            Destroy(gameObject);//바닥에 닿으면 냉장고로 다시 들어가지 않고 영원히 삭제됨
        }
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        if (!isbeingShot && !isOnGround && other.gameObject.CompareTag("Pan"))
        {
            Debug.Log("Falling object caught by Pan! :" + price);
            GameManager.Instance.add_money(price);
            GameManager.Instance.SpawnGoldText(transform.position, price);

            // 오브젝트 정보를 저장하고 바로 삭제
            if (falling_name != "FriEgg") {
                GameManager.Instance.AddToRefriger(falling_name, BackSpeed, UpSpeed, price);
            }
            Destroy(gameObject);
        }
    }

    public void set_Values(RefrigerItem order)
    {
        this.UpSpeed = order.upSpeed;
        this.BackSpeed = order.backSpeed;
        this.price = order.price;
        Debug.Log("Falling object set with UpSpeed: " + this.UpSpeed + ", BackSpeed: " + this.BackSpeed + ", Price: " + this.price);
    }

    void SetPrice(int speed)//속도가 높아질수록 가격이 비례해서 증가하도록 설정
    {
        if (speed < 1) speed = 1; // 최소 속도는 1로 설정
        price = price * speed;
    }
}