# WebGL 로딩 화면 템플릿 (GrazeFit)

Unity WebGL 게임을 **게임졸트 · itch.io** 같은 웹에 올릴 때 쓰는 웹페이지 템플릿입니다.

- 게임이 로딩되는 동안 **화면 가운데에 게임 이름**과 **초록색 진행 바**가 표시돼요.
- 로딩이 끝나면 로딩 화면이 사라지고 게임이 **브라우저 창을 꽉 채워요**. 스크롤과 여백이 없어요.
- 게임 이름은 Unity의 **Product Name**을 자동으로 가져와요. 그래서 다른 프로젝트에 넣어도 코드를 고칠 필요가 없어요.

```
┌──────────────────────────┐
│                          │
│          GRAZE           │  ← Player Settings 의 Product Name
│   ███████░░░░░░░░░░      │  ← 로딩 진행 바 (초록색)
│                          │
└──────────────────────────┘
```

---

## 들어 있는 파일

```
WebGL_LoadingTemplate_Kit
├─ README.md                               ← 이 설명서
├─ Assets/WebGLTemplates/GrazeFit/index.html  ← 로딩 화면 템플릿 (이것만 있으면 됨)
└─ tools/make_web_zip.py                   ← (선택) 이미 빌드한 결과물을 업로드용 ZIP으로 묶는 도구
```

---

## 1. 다른 프로젝트에 적용하기 (기본 방법)

1. **폴더 복사**
   이 키트의 `Assets/WebGLTemplates/GrazeFit` 폴더를 새 프로젝트의 **`Assets/WebGLTemplates/`** 안에 그대로 복사하세요.
   - `WebGLTemplates` 폴더가 없으면 새로 만드세요. 이름은 **정확히 `WebGLTemplates`**여야 해요(대소문자 포함).
   - 결과 경로: `새프로젝트/Assets/WebGLTemplates/GrazeFit/index.html`

2. **게임 이름 정하기**
   `Edit → Project Settings → Player` 맨 위의 **Product Name**에 로딩 화면에 표시할 이름을 쓰세요. 예: `GRAZE`
   (브라우저 탭 제목에도 같은 이름이 들어가요.)

3. **템플릿 선택**
   `Player → WebGL 탭(지구본 아이콘) → Resolution and Presentation → WebGL Template`에서 **GrazeFit**을 클릭하세요.

4. **(선택) Unity 로고 끄기**
   `Player → Splash Image → Show Splash Screen` 체크를 끄세요.
   Unity 6 이상은 무료 버전에서도 끌 수 있어요.

5. **압축 설정 (중요)**
   `Player → Publishing Settings`
   - **Compression Format: Gzip** 또는 **Disabled** 추천
   - **Decompression Fallback: 체크**
   → 이렇게 해야 게임졸트·itch.io 서버 설정과 상관없이 실행돼요.

6. **빌드**
   `File → Build Settings(Build Profiles) → Web(WebGL) → Build`
   - 출력 폴더는 **매번 새 빈 폴더**로 고르세요. 예: `MyGame_Web_0101`
   - 기존 폴더에 덮어쓰면 예전 빌드 파일이 섞여서 용량이 커지고 헷갈려요.

7. **ZIP 만들기**
   빌드된 폴더 **안으로 들어가서** `index.html`, `Build`, (있으면) `StreamingAssets`를 **함께 선택 → 압축**하세요.
   - ❌ 폴더 자체를 압축하지 마세요. ZIP을 열었을 때 `index.html`이 **바로** 보여야 해요.
   ```
   MyGame.zip
   ├─ index.html
   └─ Build/
      ├─ (이름).loader.js
      ├─ (이름).framework.js
      ├─ (이름).data
      └─ (이름).wasm
   ```

8. **업로드**
   - 게임졸트: Packages → **Browser build (HTML)** 로 ZIP 업로드
   - itch.io: Kind of project **HTML** → ZIP 업로드 → **This file will be played in the browser** 체크

---

## 2. (선택) 이미 빌드한 결과물을 바로 ZIP으로 만들기

다른 템플릿으로 이미 빌드해 버렸거나, Build 폴더에 여러 빌드가 섞여 있을 때 사용해요.
Python이 설치되어 있어야 해요.

```bash
python -X utf8 tools/make_web_zip.py "C:/MyGame/WebGL/Build" "My Game"
```

- 첫 번째 값: 빌드 결과의 **Build 폴더** 경로
- 두 번째 값: 로딩 화면에 표시할 **게임 이름**
- 결과: Build 폴더 옆에 `My_Game_web.zip`이 생겨요. 바로 업로드하면 돼요.
- Build 폴더에 여러 빌드가 섞여 있으면 **가장 최근 빌드 파일만** 골라서 담아요.

> ⚠️ 앱인토스(AITTemplate) 빌드 폴더에는 이 방법을 쓰지 마세요.
> 앱인토스는 자체 도구가 index.html을 만들어요.

---

## 3. 모양 바꾸기

`GrazeFit/index.html`을 메모장이나 VS Code로 열어서 아래 부분만 고치면 돼요.

| 바꾸고 싶은 것 | 찾을 부분 | 예시 |
|---|---|---|
| 배경색 | `html, body { ... background: #000; }` | `#7eb88c` (초록) |
| 진행 바 색 | `#fill { ... background: #9be15d; }` | `#d6489f` (핑크) |
| 진행 바 바탕색 | `#bar { ... background: #333; ... }` | `#ffffff55` |
| 진행 바 굵기 / 길이 | `#bar { width: 50%; max-width: 320px; height: 8px; ...}` | `height: 14px;` |
| 글자 색 / 크기 | `#loading { ... color: #fff; ... font-size: 16px; ...}` | `font-size: 32px;` |
| 표시되는 이름 | Unity **Product Name** (파일 수정 불필요) | — |

### 게임 이름 대신 로고 이미지를 넣고 싶을 때

1. `GrazeFit` 폴더 안에 `TemplateData` 폴더를 만들고 로고를 넣으세요. 예: `TemplateData/logo.png`
   (빌드할 때 함께 복사돼요.)
2. index.html의 이 줄을
   ```html
   <div id="loading"><div>{{{ PRODUCT_NAME }}}</div><div id="bar"><div id="fill"></div></div></div>
   ```
   이렇게 바꾸세요.
   ```html
   <div id="loading"><img src="TemplateData/logo.png" style="width:60%;max-width:420px"><div id="bar"><div id="fill"></div></div></div>
   ```

### 템플릿 이름을 바꾸고 싶을 때

`GrazeFit` 폴더 이름을 바꾸면 Unity의 템플릿 목록에 그 이름으로 보여요. 예: `MyLoading`.
(index.html 내용은 그대로 두면 돼요.)

---

## 4. 자주 생기는 문제

| 증상 | 원인 / 해결 |
|---|---|
| **"Unity Loader 스크립트 로드 실패"**, `%UNITY_WEBGL_LOADER_URL%` 표시 | **AITTemplate(앱인토스용)으로 빌드한 것**을 올렸어요. Template을 **GrazeFit**으로 바꾸고 다시 빌드하세요. |
| 템플릿 목록에 GrazeFit이 안 보여요 | 폴더 위치가 `Assets/WebGLTemplates/GrazeFit/index.html`인지 확인하세요. |
| 로딩 바가 끝까지 찼는데 화면이 검은색이에요 / 압축 관련 오류 | Publishing Settings에서 **Decompression Fallback**을 켜거나 Compression을 **Gzip / Disabled**로 바꾸고 다시 빌드하세요. |
| 업로드했는데 "index.html이 없다"고 나와요 | 폴더째로 압축했어요. 폴더 **안의 파일들**을 선택해서 압축하세요. |
| "로딩 실패 : ..." 글자가 나와요 | 메시지 내용이 오류 원인이에요. Build 폴더 파일이 빠졌는지 확인하세요. |
| 앱인토스 빌드가 이상해졌어요 | WebGL Template은 **공유 설정**이에요. 앱인토스용으로 빌드하기 전에 **AITTemplate**으로 되돌리세요. |

---

## 5. 동작 원리 (참고)

- Unity는 빌드할 때 `index.html` 안의 `{{{ PRODUCT_NAME }}}`, `{{{ LOADER_FILENAME }}}` 같은 표시를 실제 값으로 바꿔 넣어요.
- `createUnityInstance(...)`가 게임을 불러오면서 진행률(0~1)을 알려주면, 그만큼 진행 바(`#fill`)의 너비를 늘려요.
- 로딩이 끝나면 `#loading` 요소를 지워서 게임 화면만 남겨요.
- `matchWebGLToCanvasSize: true`로 게임 해상도를 브라우저 창 크기에 맞추고, `devicePixelRatio`는 최대 2로 제한해서 휴대폰에서도 선명하면서 너무 무겁지 않게 해요.
