using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    public Rigidbody rb;
    public float speed = 8f;

    void Start(){
        rb = GetComponent<Rigidbody>(); //떨어지지 않게 하기위해 리지드 바디 세팅
    }

    void Update(){
        // 수평축과 수직축을 감지하여 저장
        float xInput = Input.GetAxis("Horizontal");
        float zInput = Input.GetAxis("Vertical");

        // 입력값 * 이동 속도 = 실 이동 속도 결정
        float xSpeed = xInput * speed;
        float zSpeed = zInput * speed;
        
        Vector3 newVelocity = new Vector3(xSpeed, 0f, zSpeed); 
        rb.linearVelocity = newVelocity; // Rigidbody의 속도에 적용

        /*
        if (Input.GetKey(KeyCode.UpArrow)){
            rb.AddForce(0f, 0f, speed);
        }
        if(Input.GetKey(KeyCode.DownArrow)){
            rb.AddForce(0f, 0f, -speed);
        }
        if(Input.GetKey(KeyCode.RightArrow)){
            rb.AddForce(speed, 0f, 0f);
        }
        if(Input.GetKey(KeyCode.LeftArrow)){
            rb.AddForce(-speed, 0f, 0f);
        }
        */
    }

    public void Die(){
        gameObject.SetActive(false);
        GameManager gm = FindFirstObjectByType<GameManager>();
        gm.EndGame();
    }   
     
}

/*
[📌📝MEMO]  p.290, p.302,

< Getkey는 Input클래스에서 키보드 입력 감지에 대한 로직 >
> 키를 누르는 그 순간, 딱 한 프레임만 true
    Input.GetKeyDown(KeyCode.Space)

> 키를 누르고 있는 동안 계속 true
    Input.GetKey(KeyCode.Space) 

> 키에서 손을 떼는 그 순간, 딱 한 프레임만 true
    Input.GetKeyUp(KeyCode.Space)


Input.GetAxis:키보드 뿐만아니라 게임 패드의 조이스틱의 수평 방향에도 대응됨
                숫자로 입력을 받(-1.0 || +1.0 사이)
AddForce(): 힘을 누적하고 속력을 점진적으로 증
linearVelocity: 현재 속도 변경
*/
