using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    List<int> protectedObjects = new List<int>();//폭발로 인해 게임 매니저가 생성한 오브젝트의 ID를 저장
    //GameObject explosionEffect;
    bool isExploding = false;
    bool destroyed = false;
    float timer = 0;

    public void ExplodeCoroutineStart(Vector3 position, Vector3 scale)
    {
        isExploding = true;
        transform.position = position;
        transform.localScale = scale;
    }

    void Update()
    {
        if (isExploding)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                isExploding = false;
                timer = 0;
                if (destroyed) {
                    if (GameManager.Instance.minus_money())// 기본 10 감소
                    {
                        //GameManager.Instance.CreateRandomObject();
                    }
                }
                ExplodeEnd();
            }
        }
    }

    void ExplodeEnd()
    {
        Debug.Log("Explosion effect finished");
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name + " hit by explosion!");
        if (other.gameObject.CompareTag("Player"))
        {
            //other.GetComponent<SpriteRenderer>().color = Color.red; // 플레이어가 폭발에 닿으면 빨간색으로 변경
            GameManager.Instance.GameOver();
        }
        if (other.gameObject.CompareTag("Explodable"))
        {
            Debug.Log("Explodable object hit by explosion!");
            Destroy(other.gameObject);
            destroyed = true;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Explodable"))
        {
            Debug.Log("Explodable object hit by explosion!");
        }
    }
}
