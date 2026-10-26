using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

    public void Change(int index) {
        // SceneManager에서 LoadScene를 불러와라 ()안에 숫자 또는 Scene 이름
        SceneManager.LoadScene(index);
    }
}


/*
무조건 해당 씬으로 가고싶다면 이렇게
public void Change(){
    SceneManager.LoadScene(1);
}

*/
