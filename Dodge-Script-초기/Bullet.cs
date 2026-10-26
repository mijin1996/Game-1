using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float speed = 8f;
    private Rigidbody rb;
    
    public GameObject effect;

    public AudioSource hitSource;
    public AudioClip clipDie;

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "PlayerInput") {
            // 맞으면 소리가 생성
            hitSource.PlayOneShot(clipDie);
            // 맞으면 이펙트 생성
            Instantiate(effect, other.transform.position, Quaternion.identity);

            PlayerInput playerInput = other.GetComponent<PlayerInput>();

            // bullet이 플레이어 컴포넌트(PlayerInput.cs)를 가져오는것에 성공했다면
            if (playerInput != null) {
                playerInput.Die();
                GameObject.Find("Music").GetComponent<AudioSource>().Play();
            }
        }
    }
}


/*
[📌📝MEMO] p.334

< >
Rigidbody::⁉️ 물리 법칙(중력, 힘, 충돌 등)을 적용시켜주는 컴포넌트
linearVelocity::⁉️ 물체가 현재 이동하고 있는 속도(방향 + 빠르기)를 나타내는 프로퍼티

< >      
other :: ⁉️ 충돌한 상대방의 Collider를 의미
other.tag :: ⁉️ 충돌한 상대방의 태그를 의미








Input.GetKey와 Input.GetAxis는 예전 입력방식, 아래 오류 발생시 
InvalidOperationException

Edit → Project Settings → Player → Active Input Handling을 Both 설정
*/
