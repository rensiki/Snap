using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody myRigid;
    [SerializeField] private GameObject pan;
    [SerializeField] private FixedJoystick joy;

    [SerializeField] private GameObject fallingObject;//상태를 확인하기 위해 serializeField로 지정
    [SerializeField] float player_speed = 10f;

    Transform panTrans;
    Vector3 player_moveVec;


    bool isPanSwinging = false;
    float swingTime = 0.1f;
    float swingPower = 0;
    float x_GyroValue = 0;

    void Awake()
    {
        Input.gyro.enabled = true;
        panTrans = pan.GetComponent<Transform>();
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        x_GyroValue = Input.gyro.rotationRateUnbiased.x;
        SwingPowerFunc();
        PlayerMovingFunction();
        PanSwing();
        PanRotation();
    }
    void PlayerMovingFunction()
    {
        float x = -1*joy.Horizontal;//플레이어의 시점으로 캐릭터를 움직이기 위함
        float z = -1*joy.Vertical;

        player_moveVec = new Vector3(x, 0, z);
        myRigid.velocity = player_moveVec * player_speed;
    }
    void PanRotation()
    {
        if ((panTrans.rotation.x > -2 && x_GyroValue>-2) || x_GyroValue > 0)//-2 전까지는 자유롭게 움직일 수 있음
        {
            panTrans.rotation = Quaternion.Euler(-x_GyroValue * 10, 0, 0);
        }
    }
    void PanSwing()
    {
        if (!isPanSwinging)
        {
            if (x_GyroValue > 2)
            {
                isPanSwinging = true;
                swingPower = 0;
                Debug.Log("swing 시작" + x_GyroValue);
                pan.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                Invoke("PanColorFunc", swingTime);
                Invoke("ShootFalling", swingTime);
            }
        }
        else
        {
            if (x_GyroValue < 0) isPanSwinging = false;
        }
    }
    void ShootFalling()
    {
        set_fallingObject();
        fallingObject.transform.position = transform.position + new Vector3(0, 1, 1);
        fallingObject.transform.rotation = Quaternion.identity;
        fallingObject.SetActive(true);
        Debug.Log(swingPower);
        fallingObject.GetComponent<Falling>().shootFunction(swingPower, Vector3.forward);
        fallingObject = null;
    }
    void set_fallingObject()
    {
        //게임 매니저의 냉장고 맨 앞에서 생성된 요리를 가지고온다.
        fallingObject = GameManager.Instance.get_refriger();
    }
    void PanColorFunc()
    {
        pan.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;
    }
    void SwingPowerFunc()
    {
        if (isPanSwinging)
        {
            if(x_GyroValue > 0)
                swingPower += x_GyroValue*0.1f;
        }
    }
}
