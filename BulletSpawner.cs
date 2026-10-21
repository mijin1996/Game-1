using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
    public GameObject bulletPrefab;
    public Transform firePoint;
    private Transform target;

    public float spawnRateMin = 0.5f;
    public float spawnRateMax = 3f;
    
    private float spawnRate;
    private float timeAfterSpawn; //최근 생성 시점에서 경과한 시간

    void Start() {
        timeAfterSpawn = 0f;
        spawnRate = Random.Range(spawnRateMin, spawnRateMax);
        target = FindFirstObjectByType<PlayerInput>().transform;
    }

    void Update() {
        timeAfterSpawn += Time.deltaTime;

        if (timeAfterSpawn >= spawnRate) {
            timeAfterSpawn = 0f; //누적 시간 초기화

            //GameObject bulletObj = Instantiate(bulletPrefab, transform.position, transform.rotation); //자기 위치에서 생성
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation); // 따로 지정한 위치에서 생성            
            bulletObj.transform.LookAt(target);
            spawnRate = Random.Range(spawnRateMin, spawnRateMax); // 재생성 주기 랜덤화

        }
    }
}


 /*
[📌📝MEMO] p.352

<Transform의 기능 >
LookAt::⁉️ 갑자기 바뀜

< Quaternion과 짝인 아이들 >
LookRotation::⁉️ 둘 다 한 번 돌려서 같은 방향 바라봄.
서서히 돌리려면 Quaternion.Slerp를 함께 써야 해요. = 자연스러움

< >
Time.deltaTime::⁉️ 이전 프레임에서 현재 프레임까지 걸린 (perSec마다 로딩시간) = 서로 다른 성능의 컴퓨터나 핸드폰에서 동일한 속도를 유지하기 위함

< >
<Type(컴퍼넌트)>

< 시간인지? 거리인지? 속도인지?>
> 시간
    Time.deltaTime	지난 프레임 이후 경과 시간(초)
    Time.time	게임 시작 후 총 경과 시간(초)
    Time.fixedDeltaTime	물리 업데이트 간격 시간
    Time.timeScale	시간 흐르는 속도 배율 (일시정지, 슬로우모션 등)
    Time.realtimeSinceStartup	앱 실행 후 실제 흐른 시간 (timeScale 영향 안 받음)
    Invoke("함수명", 시간)	몇 초 뒤에 함수 실행
    WaitForSeconds(시간)	코루틴에서 몇 초 대기
    Coroutine	시간 흐름에 따라 순차 실행하는 함수

> 거리/위치
    Vector3.Distance(a, b)	두 지점 사이의 거리 계산
    Vector3.magnitude	벡터의 길이(원점으로부터의 거리)
    Vector3.sqrMagnitude	거리의 제곱 (성능 최적화용, 실제 거리 비교시 루트 계산 생략)
    (a - b).magnitude	Distance와 동일한 결과를 다르게 계산하는 방식
    Physics.OverlapSphere(pos, radius)	특정 반경 안의 오브젝트들 찾기 (거리 기반 감지)
    Vector2.Distance(a, b)	2D 버전 거리 계산

> 속도
    Rigidbody.linearVelocity	물체의 이동 속도 (방향+빠르기)
    Rigidbody.angularVelocity	회전 속도
    Vector3.MoveTowards(a, b, speed)	일정 속도로 점점 이동시키기

p.342
< Random.Range() :: int,float>
    📌Random.Range(0, 3) :: 0, 1, 2중 하나가 int값으로 출력
    📌Random.Range(0f, 3f) :: 0f ~ 3f 사이 float값이 출력


*/
