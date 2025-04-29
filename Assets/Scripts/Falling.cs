using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falling : MonoBehaviour
{
    Rigidbody myRigid;
    public float UpSpeed = 0.3f;
    public float BackSpeed = 2f;

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
            //GetComponent<BoxCollider>().enabled = false;
            //GetComponent<Rigidbody>().isKinematic = true;
            //버그 일으킴; 발사가 안됨
        }
    }
    void OnTriggerEnter(Collider other) {
        if (!isbeingShot&&!isOnGround&&other.gameObject.CompareTag("Pan"))
        {
            gameObject.SetActive(false);
        }
    }
}