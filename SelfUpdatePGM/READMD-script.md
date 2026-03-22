
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

```
cd DeployServer; dotnet run
```

# SUartPGM

```
SUartPGM 프로그램은 Windows OS booting과 동시에 실행이 되고
SUartPGM 프로그램 비정상 종료시 다시 실행되도록 해죠

구현 방향

부팅 시 자동 실행

Windows 작업 스케줄러: 로그온 시 실행되도록 작업 등록
레지스트리: HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run에 실행 경로 등록
시작 프로그램 폴더: %APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup에 바로가기 추가
비정상 종료 시 재실행

시스템 서비스: SUartPGM을 Windows 서비스로 구현하고 실패 시 자동 재시작 정책 설정
별도 감시 프로그램(Wrapper): SUartPGM을 실행하고, 종료되면 감시 프로그램이 다시 실행
작업 스케줄러: 1분마다 동작 여부를 확인하고, 종료됐으면 다시 실행하는 작업 등록
이 중에서 어떤 방식으로 구현할지 정하면, 해당 방법 기준으로 구체적인 설정/코드 예시를 정리해 드리겠습니다.

## SUartPGMWrapper (감시 프로그램)

SUartPGM을 실행하고, 비정상/정상 종료 시 2초 후 자동 재시작합니다.

### 사용법
- **직접 실행**: `SUartPGMWrapper.exe` 실행 → SUartPGM 자동 실행 및 감시
- **부팅 시 자동 실행**: 아래 중 하나로 `SUartPGMWrapper.exe` 등록
  - 레지스트리: `HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run`
  - 시작 프로그램: `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup`에 바로가기 추가

### 동작
1. SUartPGM.exe를 같은 폴더에서 실행
2. 종료 시 2초 대기 후 재시작 (크래시 루프 방지)
3. SUartPGM.exe 없으면 `SUartPGMWrapper.log`에 기록 후 종료

### 중지 방법
SUartPGMWrapper.exe 프로세스를 종료하면 SUartPGM도 함께 종료되며 재시작되지 않습니다.


부팅 시 자동 실행 예시 (PowerShell, 관리자 권한):
$exePath = "C:\경로\SUartPGM\SUartPGMWrapper.exe"
Set-ItemProperty -Path "HKCU:\Software\Microsoft\Windows\CurrentVersion\Run" -Name "SUartPGM" -Value $exePath

```