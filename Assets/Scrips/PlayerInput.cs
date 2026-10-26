using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour{
    public Rigidbody rb;
    public float speed = 8f;
    public float skinWidth = 0.05f; // 벽과 남겨둘 여유 거리 (벽에 파고들지 않게)
    public float stepHeight = 0.3f; // 이 높이 아래(바닥, 낮은 턱)는 벽으로 보지 않음
    CapsuleCollider body;

    Animator ani; // 걷는 애니메이션을 주기위함

    public DynamicJoystick joy;

    [Header("점프")]
    public float jumpPower = 6f;   // 점프 순간 위로 솟는 속도
    public float gravity = 15f;    // 내려오는 힘 (클수록 빨리 떨어짐)
    private float yVelocity;       // 현재 위아래 속도
    private float groundY;         // 바닥 높이 (시작할 때 서 있던 높이)
    private bool isGrounded = true;

    [Header("소리")]
    public AudioSource footstepSource;   // 발소리 전용 스피커 (Loop)
    public AudioSource playerSource;     // 점프 같은 효과음 스피커
    public AudioClip jumpClip;


    void Start(){
        rb = GetComponent<Rigidbody>(); //떨어지지 않게 하기위해 리지드 바디 세팅
        ani =  GetComponent<Animator>();
        body = GetComponent<CapsuleCollider>();
        groundY = transform.position.y; // 시작 높이를 바닥으로 기억
    }

    // 발밑에서 stepHeight만큼 띄운 구를 앞으로 쏴서 벽이 있는지 검사
    // (바닥에 닿은 채로 검사하면 바닥이 먼저 맞아서 벽을 못 찾기 때문에 띄워서 쏨)
    bool IsBlocked(Vector3 dir, float distance) {
        Bounds b = body.bounds;                       // 월드 기준 크기 (스케일 반영)
        float radius = b.extents.x * 0.8f;            // 몸보다 살짝 작은 구
        float feet = b.min.y;
        Vector3 origin = new Vector3(b.center.x, feet + stepHeight + radius, b.center.z);
        float castDist = distance + skinWidth + (b.extents.x - radius);

        foreach (RaycastHit hit in Physics.SphereCastAll(origin, radius, dir, castDist, ~0, QueryTriggerInteraction.Ignore)) {
            if (hit.collider.attachedRigidbody == rb) { continue; } // 내 몸
            if (hit.distance <= 0f) { continue; }                   // 이미 겹쳐 있으면 빠져나갈 수 있게
            if (hit.normal.y >= 0.7f && hit.point.y <= feet + stepHeight) { continue; } // 점프했을 때 건물·나무를 통과하지 않게 함 (발밑 바닥만 통과)            return true;
            return true;   // ← 이 줄! 위의 세 경우가 아니면 "벽이다, 멈춰라"
        }
        return false;
    }

    void Update(){       
        Vector3 move = new Vector3(joy.Horizontal, 0, joy.Vertical);
        // PC 테스트용 : 조이스틱 입력이 없으면 방향키 / WASD로 이동
        if(move == Vector3.zero){
            move = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }
        if( move != Vector3.zero){
            transform.rotation = Quaternion.LookRotation(move);

            // 이동 전에 앞을 미리 검사 : 벽(건물 등)이 있으면 멈춤
            float step = speed * Time.deltaTime;
            if (!IsBlocked(transform.forward, step)) {
                transform.Translate(Vector3.forward * step);
            }

            ani.SetBool("Walk", true);
        }
        else{
            ani.SetBool("Walk", false);
        }

        // 땅에서 움직일 때만 발소리, 멈추거나 점프 중이면 끔
        if(move != Vector3.zero && isGrounded){
            if(!footstepSource.isPlaying){ footstepSource.Play(); }
        }
        else{
            if(footstepSource.isPlaying){ footstepSource.Stop(); }
        }

        // PC 테스트용 : 스페이스바로도 점프
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        // 공중에 있을 때만 중력 적용, 바닥 높이에 닿으면 착지
        if (!isGrounded) {
            yVelocity -= gravity * Time.deltaTime;
            transform.position += Vector3.up * yVelocity * Time.deltaTime;
            if (transform.position.y <= groundY) {
                Vector3 pos = transform.position;
                pos.y = groundY;
                transform.position = pos;
                yVelocity = 0f;
                isGrounded = true;
            }
        }
    }

    // 점프 버튼(OnClick)에서 호출 : 바닥에 있을 때만 한 번 점프
    public void Jump() {
        if (!isGrounded) { return; }
        isGrounded = false;
        yVelocity = jumpPower;
        ani.SetTrigger("Jump");
        playerSource.PlayOneShot(jumpClip); // 점프 소리
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
