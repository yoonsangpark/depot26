
# Overview
```
Windows FromView 를 이용한 C# 프로젝트트 만들어죠
project name : SUartPGM

1. Class 구조
SUart : UART 통신
SLog : 로그를 RitchEdit Box와 파일로도 저장되게
  로그단계는 INFO, WARN, ERR

2. UI
listbox : 연결된 UART listup
Button : connect / disconnect 토글되게게
Textbox : UART 전송할 문자열 입력
Button : 입력된 문자 UART로 전송
RichEditBox : 로그출력력

3. 실행환경
Windows OS
.Net8.0
C#

4. 추가요청사항항
C# Project요 .gitignore 파일 만들어죠
```

# *.cab
```
지금 프로그램을 *.cab 형태로 만들고 여러 PC로 배포를 하기 위한 배포서버를 만들고
지금 프로그램이 실행한때마더 배포서버에서 최신버전 가져오도록 구현해죠
```

## 배포 및 업데이트 사용법
```
1. **빌드 & CAB 패키징**: `.\build.ps1` 실행
2. **배포 서버 실행**: `cd DeployServer; dotnet run` (기본 http://localhost:5000)
3. **클라이언트 설정**: `update.config.json`에 `updateServerUrl` 설정 (예: `http://서버IP:5000`)
4. **자동 업데이트**: SUartPGM 실행 시 서버에서 최신 버전 확인 후 업데이트
```

# DeployServer WebUI 에서 배포상황 dashboard 만들어죠
