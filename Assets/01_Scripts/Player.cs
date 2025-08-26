using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID || UNITY_IOS
using UnityEngine.Android;
#endif

public class Player : MonoBehaviour
{
    Rigidbody myRigid;
    [SerializeField] private GameObject pan;
    [SerializeField] private FixedJoystick joy;

    [SerializeField] private GameObject fallingObject;//상태를 확인하기 위해 serializeField로 지정
    [SerializeField] float player_speed = 10f;
    [SerializeField] Transform panPosition;

    Transform panTrans;
    Vector3 player_moveVec;


    bool isPanSwinging = false;
    float swingTime = 0.1f;
    float swingPower = 0;
    float x_GyroValue = 0;

    void Awake()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        panTrans = pan.GetComponent<Transform>();
        myRigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    void Update()
    {
        x_GyroValue = Input.gyro.rotationRateUnbiased.x;
    }

    void FixedUpdate()
    {
        SwingPowerFunc();
        PlayerMovingFunction();
        PanSwing();
        PanRotation();
    }
    void PlayerMovingFunction()
    {
        float x = -1 * joy.Horizontal;//플레이어의 시점으로 캐릭터를 움직이기 위함
        float z = -1 * joy.Vertical;

        player_moveVec = new Vector3(x, 0, z);
        myRigid.velocity = player_moveVec * player_speed;
        transform.LookAt(transform.position + player_moveVec);
    }
    void PanRotation()
    {
        if ((panTrans.rotation.z > -2 && x_GyroValue > -2) || x_GyroValue > 0)//-2 전까지는 자유롭게 움직일 수 있음
        {
            panTrans.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y-90, x_GyroValue * 10);
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
                //pan.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.green;
                pan.GetComponent<MeshRenderer>().material.color = Color.green;
                VibrateDevice(); // 핸드폰 진동
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
        Vector3 dir = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * Vector3.forward;
        Debug.Log("swingPower: " + swingPower + ", dir: " + dir);
        set_fallingObject();
        fallingObject.transform.position = transform.position + dir * 0.5f + Vector3.up * 2f;
        fallingObject.transform.rotation = Quaternion.LookRotation(dir);
        fallingObject.SetActive(true);
        //Debug.Log(swingPower);
        fallingObject.GetComponent<Falling>().shootFunction(swingPower, dir);
        fallingObject = null;
    }
    void set_fallingObject()
    {
        //게임 매니저의 냉장고 맨 앞에서 생성된 요리를 가지고온다.
        fallingObject = GameManager.Instance.get_refriger();
    }
    void PanColorFunc()
    {
        //pan.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.black;
        pan.GetComponent<MeshRenderer>().material.color = Color.white;
    }
    void SwingPowerFunc()
    {
        if (isPanSwinging)
        {
            if(x_GyroValue > 0)
                swingPower += x_GyroValue*0.1f;
        }
    }

    // 모바일 진동 함수 추가
    void VibrateDevice()
    {
#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();//**더 세밀한 진동 제어(시간, 패턴 등)**를 원한다면, Android의 자바 진동 API를 직접 호출해야 합니다.
#endif
    }
}
