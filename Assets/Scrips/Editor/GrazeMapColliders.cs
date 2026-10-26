using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// 움직이지 않는 오브젝트(건물 · 나무 · 바닥 등)의 Box / Sphere / Capsule Collider를
// 모양 그대로의 Mesh Collider(Convex 끔)로 바꾼다.
// - 대상 : 지금 열린 mmain 씬 전체
// - 제외 : Rigidbody가 있는 것(플레이어 · 총알), 몬스터(BulletSpawner), 애니메이션 캐릭터(Animator),
//          Is Trigger 콜라이더, 모양(Mesh)이 없어서 바꿀 수 없는 콜라이더
// GrazeMapColliders.flag 파일이 있으면 mmain 씬이 열려 있을 때 한 번만 자동 실행.
// 메뉴 GRAZE > 고정 오브젝트 콜라이더를 Mesh Collider로 변경 으로도 다시 실행할 수 있다. (Ctrl+Z로 되돌리기)
[InitializeOnLoad]
public static class GrazeMapColliders {
    private const string Flag = "Assets/Scrips/Editor/GrazeMapColliders.flag";
    private const string MainPath = "Assets/Scene/mmain.unity";

    static GrazeMapColliders() {
        if (File.Exists(Flag)) {
            EditorApplication.delayCall += AutoRun;
            EditorSceneManager.sceneOpened += (scene, mode) => AutoRun();
        }
    }

    private static void AutoRun() {
        if (!File.Exists(Flag) || EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        if (SceneManager.GetActiveScene().path != MainPath) { return; } // mmain을 열면 그때 실행
        AssetDatabase.DeleteAsset(Flag);
        if (File.Exists(Flag)) { File.Delete(Flag); }
        Convert();
    }

    private static bool IsMoving(Component c) {
        return c.GetComponentInParent<Rigidbody>() != null
            || c.GetComponentInParent<BulletSpawner>() != null
            || c.GetComponentInParent<Animator>() != null;
    }

    [MenuItem("GRAZE/고정 오브젝트 콜라이더를 Mesh Collider로 변경")]
    public static void Convert() {
        Scene scene = SceneManager.GetActiveScene();
        Undo.SetCurrentGroupName("고정 오브젝트 콜라이더 → Mesh Collider");
        int group = Undo.GetCurrentGroup();
        int removed = 0, added = 0, kept = 0;

        foreach (GameObject root in scene.GetRootGameObjects()) {
            foreach (Collider col in root.GetComponentsInChildren<Collider>(true)) {
                if (col == null || col is MeshCollider || col.isTrigger) { continue; }
                if (!(col is BoxCollider || col is SphereCollider || col is CapsuleCollider)) { continue; }
                if (IsMoving(col)) { continue; }

                // 이 콜라이더가 붙은 오브젝트(또는 그 자식)의 모양을 찾는다
                List<MeshFilter> meshes = new List<MeshFilter>();
                foreach (MeshFilter mf in col.GetComponentsInChildren<MeshFilter>(true)) {
                    if (mf.sharedMesh != null && !IsMoving(mf)) { meshes.Add(mf); }
                }
                if (meshes.Count == 0) { kept++; continue; } // 모양이 없으면 막는 기능을 잃지 않게 그대로 둠

                Undo.DestroyObjectImmediate(col);
                removed++;
                foreach (MeshFilter mf in meshes) {
                    MeshCollider mc = mf.GetComponent<MeshCollider>();
                    if (mc == null) {
                        mc = Undo.AddComponent<MeshCollider>(mf.gameObject);
                        added++;
                    }
                    else {
                        Undo.RecordObject(mc, "Mesh Collider 설정");
                    }
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = false;
                    mc.isTrigger = false;
                }
            }
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(scene);
        string msg = "Box/Sphere/Capsule Collider " + removed + "개를 지우고\nMesh Collider " + added + "개를 붙였어요."
                   + (kept > 0 ? "\n(모양이 없어 그대로 둔 콜라이더 " + kept + "개)" : "")
                   + "\n\nCtrl+S로 씬을 저장하세요. (되돌리기: Ctrl+Z)";
        Debug.Log("[GRAZE] " + msg.Replace("\n", " "));
        EditorUtility.DisplayDialog("GRAZE", msg, "확인");
    }
}
