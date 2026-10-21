using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    public Rigidbody rb;
    public float speed = 8f;

    Animator ani; // 걷는 애니메이션을 주기위함

    public VariableJoystick joy;


    void Start(){
        rb = GetComponent<Rigidbody>(); //떨어지지 않게 하기위해 리지드 바디 세팅
        ani =  GetComponent<Animator>();
    }

    void Update(){       
        Vector3 move = new Vector3(joy.Horizontal, 0, joy.Vertical);
        if( move != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(move);
            transform.Translate(Vector3.forward * Time.deltaTime * speed);

            ani.SetBool("Walk", true);
        }
        else{
            ani.SetBool("Walk", false);
        }

        
    }

    public void Die(){
        gameObject.SetActive(false);
        GameManager gm = FindFirstObjectByType<GameManager>();
        gm.EndGame();
    }   
     
}

/*
[📌📝MEMO]  p.290, p.302,

< GetKey는 Input클래스에서 키보드 입력 감지에 대한 로직 >
> 키를 누르는 그 순간, 딱 한 프레임만 true
    Input.GetKeyDown(KeyCode.Space)

> 키를 누르고 있는 동안 계속 true
    Input.GetKey(KeyCode.Space) 

> 키에서 손을 떼는 그 순간, 딱 한 프레임만 true
    Input.GetKeyUp(KeyCode.Space)


Input.GetAxis:키보드 뿐만아니라 게임 패드의 조이스틱의 수평(좌우)과 수직(앞뒤) 방향에도 대응됨
                숫자로 입력을 받음(-1.0 || +1.0 사이)
AddForce(): 힘을 누적하고 속력을 점진적으로 증가
linearVelocity: 현재 속도 변경




1차 코드의 단점: 
--------------
AddForce 때문에 계속 빨라짐
미끄러짐
방향키만 됨
컴터마다 속도 다름
코드 김
--------------

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


2차 코드의 단점:
--------------
GetAxis덕분에 키보드, 손에 쥐는 게임패드 조이스틱도되지만 화면 조이스틱 X  >>>>>>>>>>>>>>>
--------------
// ✒️ 누르는 즉시 움직이고 떼면 바로 멈춤이 잘 작동됨
// 수평축과 수직축을 감지하여 저장
float h = Input.GetAxis("Horizontal");
float v = Input.GetAxis("Vertical");

// 입력값 * 이동 속도 = 실 이동 속도 결정
float hSpeed = h * speed; //x방향
float vSpeed = v * speed; //z방향

Vector3 newVelocity = new Vector3(hSpeed, 0f, vSpeed); 
rb.linearVelocity = newVelocity; // Rigidbody의 속도에 적용


*/
