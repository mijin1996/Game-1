using UnityEngine;

public class Rotator : MonoBehaviour {
    public float speed = 60f;


    void Update() {
        transform.Rotate(0f, speed * Time.deltaTime, 0f);        
    }
}


/*
transform변수는 자주 쓰여서 미리 변수 만들어짐 == 메모리 할당을 새로 안해줘도된다. + gameObject
이동(position) 회전(rotate) 크기(scale) 가지고있음
map 회전 ⭐🗺️📍

ㄴ*/