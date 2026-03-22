
# 1. Overview

```
"Binary Server" 에서 "Industrial PC"로 바이너리를 중계하는 시스템을 만들려고 합니다.

1. Binary Server
  HTTP Get 요청이 있을시 바이너리 전송하는 Web Server
  프로젝트폴더명 : PRMS

  - 바이너리 저장 경로
   C:\PRMS\ABCD1234\ABCD1234-260322.tar.md5
   C:\PRMS\EFGHIK567890\EFGHIK567890-260323.tar.md5
   C:\PRMS\LMNO654321\LMNO654321-NF0324.tar.md5

2. FDS-HQ Server (File Distribution System)
   프로젝트폴더명 : FDSHQ

  2.1 SW Information Table URL
  https://localhost/download?swinfotable=swinfotable.html

  ModelName     SWVersion   BinaryFileName
  ABCD1234      260322      ABCD1234-260322.tar.md5
  EFGHIK567890  260323      EFGHIK567890-260323.tar.md5
  LMNO654321    NF0324      LMNO654321-NF0324.tar.md5

  2.2 Binary Downlod URL    
  https://localhost/download?binfilename=ABCD1234-260322.tar.md5
  
  - Web UI
  - schedule job, manual job 으로 구성되면
    swinfotable.html에 있는 모든 바이너리를 Binary Server로 부터 가져 옴
  - Binary 저장 경로는
    C:\binaryshared\[yy][mm][dd]\ModelName  
  
3. FDS-branch Server (File Distribution System)
  프로젝트폴더명 : FDSBranch

  - Web UI
  - schedule job, manual job 으로 구성되면
    "FDS-HQ"에 있는 모든 바이너리를 가져 옴
  - Binary 저장 경로는
    C:\binaryshared\[yy][mm][dd]\ModelName  

5. DAWorker
  - "FDS-branch Server"에서 바이너리 리스트를 
  
4. Industrial PC
  프로젝트폴더명 : FDSClient



```

# 2. Overview
```

```