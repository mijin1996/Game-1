"""이미 빌드된 Unity WebGL 결과물을 GrazeFit 로딩 화면이 붙은 업로드용 ZIP으로 묶는다.

사용법:
    python make_web_zip.py "<빌드의 Build 폴더 경로>" "<게임 이름>"

예:
    python make_web_zip.py "C:/MyGame/WebGL/Build" "My Game"

- Build 폴더에 여러 번 빌드한 파일이 섞여 있어도, 가장 최근 *.loader.js 와 같은 이름의 파일만 골라 담는다.
- 결과: Build 폴더 옆에 <게임 이름>_web.zip (ZIP을 열면 index.html 이 바로 보이는 구조)
- index.html 은 이 스크립트 옆 ../Assets/WebGLTemplates/GrazeFit/index.html 템플릿으로 만든다.
"""
import os, re, sys, json, glob, zipfile

HERE = os.path.dirname(os.path.abspath(__file__))
TEMPLATE = os.path.join(HERE, "..", "Assets", "WebGLTemplates", "GrazeFit", "index.html")


def main():
    if len(sys.argv) < 2:
        print(__doc__); sys.exit(1)
    build = os.path.abspath(sys.argv[1])
    name = sys.argv[2] if len(sys.argv) > 2 else "Game"

    loaders = sorted(glob.glob(os.path.join(build, "*.loader.js")), key=os.path.getmtime)
    if not loaders:
        sys.exit("loader.js 파일을 찾지 못했어요: " + build)
    loader = os.path.basename(loaders[-1])
    prefix = loader[: -len(".loader.js")]

    def find(kind):
        hits = [f for f in os.listdir(build) if f.startswith(prefix + "." + kind)]
        return hits[0] if hits else None

    v = {
        "LOADER_FILENAME": loader,
        "DATA_FILENAME": find("data"),
        "FRAMEWORK_FILENAME": find("framework.js"),
        "CODE_FILENAME": find("wasm"),
        "MEMORY_FILENAME": find("mem"),
        "SYMBOLS_FILENAME": find("symbols.json"),
        "USE_WASM": True,
        "PRODUCT_NAME": name,
        "COMPANY_NAME": "DefaultCompany",
        "PRODUCT_VERSION": "1.0",
    }
    for k in ("DATA_FILENAME", "FRAMEWORK_FILENAME", "CODE_FILENAME"):
        if not v[k]:
            sys.exit(k + " 파일을 찾지 못했어요 (" + prefix + ".*)")

    out, stack = [], []
    for line in open(TEMPLATE, encoding="utf-8").read().splitlines():
        s = line.strip()
        if s.startswith("#if "):
            stack.append(bool(v.get(s[4:].strip()))); continue
        if s == "#endif":
            stack.pop(); continue
        if not all(stack):
            continue
        line = re.sub(r"\{\{\{\s*JSON\.stringify\((\w+)\)\s*\}\}\}", lambda m: json.dumps(v[m.group(1)]), line)
        line = re.sub(r"\{\{\{\s*(\w+)\s*\}\}\}", lambda m: str(v[m.group(1)]), line)
        out.append(line)
    html = "\n".join(out) + "\n"

    files = [v[k] for k in ("LOADER_FILENAME", "DATA_FILENAME", "FRAMEWORK_FILENAME", "CODE_FILENAME",
                            "MEMORY_FILENAME", "SYMBOLS_FILENAME") if v[k]]
    zp = os.path.join(os.path.dirname(build), re.sub(r"[^\w\-]+", "_", name) + "_web.zip")
    with zipfile.ZipFile(zp, "w", zipfile.ZIP_DEFLATED) as z:
        z.writestr("index.html", html)
        for f in files:
            z.write(os.path.join(build, f), "Build/" + f)
        sa = os.path.join(os.path.dirname(build), "StreamingAssets")
        for dp, _, fs in os.walk(sa):
            for f in fs:
                p = os.path.join(dp, f)
                z.write(p, os.path.relpath(p, os.path.dirname(build)).replace("\\", "/"))
    print("만들었어요:", zp)
    for n in zipfile.ZipFile(zp).namelist():
        print("  ", n)


if __name__ == "__main__":
    main()
