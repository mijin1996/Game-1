using System.IO;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// GrazeIntro.flag 파일이 있으면 컴파일 후 한 번만 실행 :
// Intro 씬에 배경 · 제목 · START 버튼을 만들고, 버튼을 누르면 mmain 씬으로 가게 연결한다.
// 기존 스크립트와 에셋은 수정하지 않고, 새 오브젝트와 새 이미지만 추가한다.
// 메뉴 GRAZE > 시작화면 만들기 로도 다시 실행할 수 있다. (이미 있으면 건너뜀)
[InitializeOnLoad]
public static class GrazeIntroSetup {
    private const string Flag = "Assets/Scrips/Editor/GrazeIntro.flag";
    private const string IntroPath = "Assets/Scene/Intro.unity";
    private const string MainPath = "Assets/Scene/mmain.unity";
    private const string ImageDir = "Assets/Images/Intro/";
    private const string BtnNormal = "Assets/Images/Intro/StartButton_Normal.png";
    private const string BtnOver = "Assets/Images/Intro/StartButton_Pressed.png";
    private const string JoyBg = "Assets/Images/Joystick/Joystick_Background.png";
    private const string JoyHandle = "Assets/Images/Joystick/Joystick_Handle.png";
    private const string FontPath = "Assets/button prefab/05.FONT/Glue Gun.otf";

    static GrazeIntroSetup() {
        if (File.Exists(Flag)) {
            EditorApplication.delayCall += AutoRun;
        }
    }

    private static void AutoRun() {
        if (!File.Exists(Flag) || EditorApplication.isPlayingOrWillChangePlaymode) { return; }
        AssetDatabase.DeleteAsset(Flag);
        if (File.Exists(Flag)) { File.Delete(Flag); }
        Setup();
    }

    [MenuItem("GRAZE/시작화면 만들기")]
    public static void Setup() {
        // 새 이미지 3장을 Sprite로 가져오기
        Sprite bg = LoadSprite(ImageDir + "IntroBackground.png");
        Sprite title = LoadSprite(ImageDir + "IntroTitle.png");
        Sprite subtitle = LoadSprite(ImageDir + "IntroSubtitle.png");
        Sprite btnNormal = LoadSprite(BtnNormal);
        Sprite btnOver = LoadSprite(BtnOver);
        LoadSprite(JoyBg);       // 조이스틱 이미지도 Sprite로 준비만 해 둠 (씬 적용은 직접)
        LoadSprite(JoyHandle);
        Font font = AssetDatabase.LoadAssetAtPath<Font>(FontPath);

        // Build Settings : Intro(0) → mmain(1)
        EditorBuildSettings.scenes = new EditorBuildSettingsScene[] {
            new EditorBuildSettingsScene(IntroPath, true),
            new EditorBuildSettingsScene(MainPath, true)
        };

        // Intro 씬 열기
        if (SceneManager.GetActiveScene().path != IntroPath) {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) { return; }
            EditorSceneManager.OpenScene(IntroPath);
        }
        Scene scene = SceneManager.GetActiveScene();

        // 이미 시작화면이 있으면 : START 버튼 그림만 새 버튼으로 바꿈
        if (GameObject.Find("IntroUI") != null) {
            GameObject oldBtn = GameObject.Find("IntroUI/StartButton");
            if (oldBtn != null) {
                Undo.RecordObject(oldBtn.GetComponent<Image>(), "Start button");
                Undo.RecordObject(oldBtn.GetComponent<Button>(), "Start button");
                Undo.RecordObject(oldBtn.GetComponent<RectTransform>(), "Start button");
                oldBtn.GetComponent<Image>().sprite = btnNormal;
                oldBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(520, 160);
                Button b = oldBtn.GetComponent<Button>();
                b.transition = Selectable.Transition.SpriteSwap;
                SpriteState s = new SpriteState();
                s.highlightedSprite = btnNormal;
                s.pressedSprite = btnOver;
                s.selectedSprite = btnNormal;
                b.spriteState = s;
                Transform tt = oldBtn.transform.Find("Text");
                if (tt != null) {
                    Undo.RecordObject(tt.GetComponent<Text>(), "Start text");
                    Undo.RecordObject(tt.GetComponent<RectTransform>(), "Start text");
                    tt.GetComponent<Text>().color = Color.white;
                    tt.GetComponent<RectTransform>().offsetMin = new Vector2(0, 18);
                    Outline ol = tt.GetComponent<Outline>();
                    if (ol != null) {
                        Undo.RecordObject(ol, "Start outline");
                        ol.effectColor = new Color(0.27f, 0.17f, 0.10f, 1f);
                        ol.effectDistance = new Vector2(3, -3);
                    }
                }
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log("[GRAZE] START 버튼을 새 초록 버튼으로 교체했습니다.");
            }
            return;
        }

        // EventSystem (버튼 클릭에 필요)
        if (Object.FindAnyObjectByType<EventSystem>() == null) {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            Undo.RegisterCreatedObjectUndo(es, "EventSystem");
        }

        // Canvas : 1920×1080 기준, 화면 크기에 맞춰 늘고 줄어듦
        GameObject canvasObj = new GameObject("IntroUI", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasObj, "IntroUI");
        canvasObj.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // 배경 : 화면을 꽉 채우고, 비율은 유지 (남는 부분만 잘림)
        GameObject bgObj = CreateImage("Background", canvasObj.transform, bg);
        RectTransform bgRt = bgObj.GetComponent<RectTransform>();
        Stretch(bgRt);
        bgObj.GetComponent<Image>().raycastTarget = false;
        AspectRatioFitter fitter = bgObj.AddComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        fitter.aspectRatio = 1920f / 1080f;

        // 제목 GRAZE
        GameObject titleObj = CreateImage("Title", canvasObj.transform, title);
        Place(titleObj.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0, -150), title);
        titleObj.GetComponent<Image>().raycastTarget = false;

        // 부제 dodge the bullets!
        GameObject subObj = CreateImage("Subtitle", canvasObj.transform, subtitle);
        Place(subObj.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0, -455), subtitle);
        subObj.GetComponent<Image>().raycastTarget = false;

        // START 버튼
        GameObject btnObj = CreateImage("StartButton", canvasObj.transform, btnNormal);
        RectTransform btnRt = btnObj.GetComponent<RectTransform>();
        btnRt.anchorMin = btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.pivot = new Vector2(0.5f, 0.5f);
        btnRt.sizeDelta = new Vector2(520, 160);
        btnRt.anchoredPosition = new Vector2(0, -230);
        Button button = btnObj.AddComponent<Button>();
        if (btnOver != null) {
            button.transition = Selectable.Transition.SpriteSwap;
            SpriteState ss = new SpriteState();
            ss.highlightedSprite = btnNormal;
            ss.pressedSprite = btnOver;   // 누르면 눌린 그림
            ss.selectedSprite = btnNormal;
            button.spriteState = ss;
        }

        GameObject textObj = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text), typeof(Outline));
        Undo.RegisterCreatedObjectUndo(textObj, "Text");
        textObj.transform.SetParent(btnObj.transform, false);
        Stretch(textObj.GetComponent<RectTransform>());
        Text text = textObj.GetComponent<Text>();
        text.text = "START";
        if (font != null) { text.font = font; }
        text.fontSize = 72;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;
        Outline outline = textObj.GetComponent<Outline>();
        outline.effectColor = new Color(0.27f, 0.17f, 0.10f, 1f);
        outline.effectDistance = new Vector2(3, -3);
        textObj.GetComponent<RectTransform>().offsetMin = new Vector2(0, 18); // 버튼 윗면 가운데에 글자

        // 버튼 누르면 SceneChange.Change(1) → mmain 씬
        SceneChange sceneChange = btnObj.AddComponent<SceneChange>();
        UnityEventTools.AddIntPersistentListener(button.onClick, sceneChange.Change, 1);

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[GRAZE] 시작화면 만들기 완료\n"
            + "- Intro 씬 : IntroUI(배경 · 제목 · 부제 · START 버튼), EventSystem 추가\n"
            + "- START 버튼 : SceneChange.Change(1) → mmain\n"
            + "- Build Settings : 0 = Intro, 1 = mmain\n"
            + "- 새 이미지 : Assets/Images/Intro/");
    }

    private static Sprite LoadSprite(string path) {
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        if (ti != null && ti.textureType != TextureImporterType.Sprite) {
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Single;
            ti.mipmapEnabled = false;
            ti.maxTextureSize = 2048;
            ti.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static GameObject CreateImage(string name, Transform parent, Sprite sprite) {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(go, name);
        go.transform.SetParent(parent, false);
        Image img = go.GetComponent<Image>();
        img.sprite = sprite;
        img.preserveAspect = true;
        return go;
    }

    private static void Stretch(RectTransform rt) {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void Place(RectTransform rt, Vector2 anchor, Vector2 pos, Sprite sprite) {
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = pos;
        if (sprite != null) {
            rt.sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
        }
    }
}
