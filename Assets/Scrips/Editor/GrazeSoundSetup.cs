using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// GrazeSound.flag 파일이 있으면 컴파일 후 한 번만 실행 :
// Player에 AudioSource 2개(발소리 Loop / 효과음)를 붙이고 PlayerInput 칸에 연결한다.
// 메뉴 GRAZE > 플레이어 소리 연결 로도 다시 실행할 수 있다. (Ctrl+Z로 되돌리기 가능)
[InitializeOnLoad]
public static class GrazeSoundSetup {
    private const string Flag = "Assets/Scrips/Editor/GrazeSound.flag";
    private const string MainScenePath = "Assets/Scene/mmain.unity";
    private const string FootstepPath = "Assets/Sounds/Footstep.mp3";
    private const string JumpPath = "Assets/Sounds/Jump.mp3";

    static GrazeSoundSetup() {
        if (File.Exists(Flag)) {
            EditorApplication.delayCall += AutoRun;
        }
    }

    private static void AutoRun() {
        if (!File.Exists(Flag) || EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        if (SceneManager.GetActiveScene().path != MainScenePath) {
            Debug.LogWarning("[GRAZE] mmain 씬을 열고 메뉴 GRAZE > 플레이어 소리 연결 을 눌러 주세요.");
            return;
        }
        AssetDatabase.DeleteAsset(Flag);
        if (File.Exists(Flag)) { File.Delete(Flag); }
        Setup();
    }

    [MenuItem("GRAZE/플레이어 소리 연결")]
    public static void Setup() {
        AudioClip footstepClip = AssetDatabase.LoadAssetAtPath<AudioClip>(FootstepPath);
        AudioClip jumpClip = AssetDatabase.LoadAssetAtPath<AudioClip>(JumpPath);
        if (footstepClip == null || jumpClip == null) {
            Debug.LogError("[GRAZE] 소리 파일을 찾지 못했습니다 : " + FootstepPath + " , " + JumpPath);
            return;
        }

        PlayerInput input = null;
        PlayerInput[] inputs = Object.FindObjectsByType<PlayerInput>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (inputs.Length > 0) { input = inputs[0]; }
        if (input == null) {
            Debug.LogError("[GRAZE] PlayerInput이 붙은 Player를 찾지 못했습니다.");
            return;
        }

        GameObject player = input.gameObject;
        Undo.SetCurrentGroupName("GRAZE 플레이어 소리 연결");
        AudioSource[] sources = player.GetComponents<AudioSource>();
        while (sources.Length < 2) {
            Undo.AddComponent<AudioSource>(player);
            sources = player.GetComponents<AudioSource>();
        }

        AudioSource footstepSource = sources[0];
        Undo.RecordObject(footstepSource, "Footstep source");
        footstepSource.clip = footstepClip;
        footstepSource.loop = true;
        footstepSource.playOnAwake = false;

        AudioSource playerSource = sources[1];
        Undo.RecordObject(playerSource, "Player source");
        playerSource.clip = null;
        playerSource.loop = false;
        playerSource.playOnAwake = false;

        SerializedObject so = new SerializedObject(input);
        so.FindProperty("footstepSource").objectReferenceValue = footstepSource;
        so.FindProperty("playerSource").objectReferenceValue = playerSource;
        so.FindProperty("jumpClip").objectReferenceValue = jumpClip;
        so.ApplyModifiedProperties();

        EditorSceneManager.MarkSceneDirty(player.scene);
        EditorSceneManager.SaveScene(player.scene);
        Debug.Log("[GRAZE] 플레이어 소리 연결 완료\n"
            + "- " + player.name + " : AudioSource 1 = 발소리(Footstep, Loop), AudioSource 2 = 효과음\n"
            + "- PlayerInput : Footstep Source / Player Source / Jump Clip 연결\n"
            + "- 씬 저장 완료");
    }
}
