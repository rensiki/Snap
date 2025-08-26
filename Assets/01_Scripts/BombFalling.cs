using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombFalling : Falling
{
    [SerializeField] private GameObject Explosion; // 폭발 프리팹
    [SerializeField] private Vector3 ExplosionRange = new Vector3(5f, 5f, 5f); // 폭탄 폭발 범위

    public override void OnCollisionEnter(Collision collision)
    {
        if (!isbeingShot)
        {
            Explode();
        }
    }
    void Start()
    {
        //Explosion = transform.GetChild(0).gameObject; // 폭발 오브젝트를 자식으로 설정
    }


    public override void OnTriggerEnter(Collider other)
    {
        if (!isbeingShot && !isOnGround && other.gameObject.CompareTag("Pan"))
        {
            //Debug.Log("Bomb falling object caught by Pan!");
            Explode();
            GameManager.Instance.GameOver();
        }
    }

    void Explode()
    {
        // 폭발 효과를 생성하고 오브젝트를 삭제
        //Debug.Log("Bomb exploded!");

        Explosion = Instantiate(Explosion, transform.position, Quaternion.identity);
        Explosion.GetComponent<Explosion>().ExplodeCoroutineStart(transform.position, ExplosionRange);
        Destroy(gameObject);
    }
    
}
