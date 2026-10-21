using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{
    public GameObject gameOverUI;
    public Text timeText;
    public Text recordText;

    private float surviveTime;
    private bool isGameover;

    void Start(){
        surviveTime = 0f;
        isGameover = false;
    }

    void Update(){
        if(!isGameover){
            surviveTime += Time.deltaTime;
            timeText.text = "Time: " + (int)surviveTime; 
        }else{
            if(Input.GetKeyDown(KeyCode.R)){
                SceneManager.LoadScene("SampleScene");
            }
        }
    }

    public void EndGame(){
        isGameover = true;
        gameOverUI.SetActive(true);
                                    //Get으로 기록 가져오기
        float bestTime = PlayerPrefs.GetFloat("BestTime");

        if(surviveTime > bestTime){
            bestTime = surviveTime; // 최고 기록값을 bestTime에 저장
            PlayerPrefs.SetFloat("BestTime", bestTime);
        }
        recordText.text = "진미안녕 Best Time: " + (int)bestTime;
   }

}


/*
[📌📝MEMO] 399

< LoadScene 쉬운 소스코드 >
public int sceneIndex = 0; // 인스펙터에서 숫자로 수정 가능
void Update(){
    if (Input.GetKeyDown(KeyCode.R)){
        SceneManager.LoadScene(sceneIndex);
    }
}

static 클래스: PlayerPrefs, Time, Random, Input, SceneManager
PlayerPrefs::⁉️ Prefs는 설정 값(메서드)들만 가져와서 컴퓨터에 저장, 게임을 꺼도 메모리 할당됨

벡터, 하나가 회전하면 다른 점들은 잠김 = 오일러 각의 짐벌락 현상(xyz가 한축 90도가 되면 두 축이 겹쳐서 한 방향으로 회전할 수 없음) = 2D게임에서는 상관없지만 3D게임에서는 회전이 자연스럽지 않음
쿼터니언 짐벌락의 현상을 잡아줌 = 회전하면 다른 점들도 같이 회전 = 자연스러움 => 더 자연스러우려면 slerp를 써서 회전 속도를 조절해주면 됨
*/
