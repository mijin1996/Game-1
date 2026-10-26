using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 자식 오브젝트는 항상 부모를 따라 움직임.
//주석 그대로 → 자식은 부모에 딱 붙어서 따라만 감
//주석 해제 → 자식은 부모를 따라가면서 혼자 더 돌기도 함
public class Move_n_Rotate : MonoBehaviour {
    //public Transform childTransform;

    void Start() {
        transform.position = new Vector3(0, -1, 0);
        //childTransform.localPosition = new Vector3(0, 2, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 30));
        //childTransform.localRotation = Quaternion.Euler(new Vector3(0, 60, 0));
    }

    void Update() {
        if (Input.GetKey(KeyCode.UpArrow)) {
            transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow)) {
            transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.LeftArrow)) {
            transform.Rotate(new Vector3(0, 0, 180) * Time.deltaTime);
            //childTransform.Rotate(new Vector3(0, 180, 0) * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            transform.Rotate(new Vector3(0, 0, -180) * Time.deltaTime);
            //childTransform.Rotate(new Vector3(0, -180, 0) * Time.deltaTime);
        }
    }
}




/* 
[📌📝MEMO] p.469

공간과 움직임
3D 旋转 = Euler Angles
오일러 각의 Vector3: 欧拉角只是实现 3D 旋转的方法之一

< Gimbal Lock > p. 441
> Gimbal Lock 的 定义:: 3D 空间中两个旋转轴重合，导致失去 1 个自由度的现象。

> Gimbal Lock 的 产生原因 :
    欧拉角是按顺序依次计算各轴旋转的。
    当中间轴旋转 90 度时，导致另外两个旋转轴在空间中完全重合。
    因此两个轴只能做同一种旋转，失去了第三个方向的旋转能力 (失去 1 个自由度)。

> 解决方案 (해결책):
    使用四元数 (Quaternion / 쿼터니언) 进行 3D 旋转计算。

    
*/