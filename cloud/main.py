import pygame
from atmosphere import Atmosphere
from cloud import Cloud
from skewt import SkewT

pygame.init()

W,H=1200,700
screen=pygame.display.set_mode((W,H))
pygame.display.set_caption("Cloud Simulator")

font=pygame.font.SysFont("malgungothic",15)

atm=Atmosphere()
atm.calc()

cloud=Cloud()
skewt=SkewT()

sliderX=50
sliderY=650
sliderW=300
sliderValue=0

clock=pygame.time.Clock()

running=True

while running:

    clock.tick(60)
    screen.fill((180,220,255))

    for event in pygame.event.get():

        if event.type==pygame.QUIT:
            running=False

        if event.type==pygame.MOUSEBUTTONDOWN:
            mx,my=pygame.mouse.get_pos()
            if sliderX<=mx<=sliderX+sliderW:
                sliderValue=(mx-sliderX)/sliderW

        if event.type==pygame.MOUSEMOTION:
            if pygame.mouse.get_pressed()[0]:
                mx,my=pygame.mouse.get_pos()
                if sliderX<=mx<=sliderX+sliderW:
                    sliderValue=(mx-sliderX)/sliderW

    sliderValue=max(0,min(sliderValue,1))

    alt=sliderValue*atm.maxAlt
    i=int(sliderValue*(len(atm.alt)-1))

    T=atm.airTemp[i]
    DP=atm.dp[i]

    state="건조단열" if T>DP else "습윤단열"

    infoH=230
    leftW=W//3
    infoPad=12
    infoRadius=16

    info=[
        f"고도:{round(alt,2)} km",
        f"공기:{T}°C",
        f"이슬점:{DP}°C",
        f"상태:{state}",
        f"LCL:{atm.LCL}",
        f"LFC:{atm.LFC}",
        f"EL:{atm.EL}"
    ]

    # ================= LEFT TOP (INFO) =================
    infoRect=pygame.Rect(infoPad,infoPad,leftW-infoPad*2,infoH-infoPad)
    pygame.draw.rect(screen,(235,235,235),infoRect,border_radius=infoRadius)
    pygame.draw.rect(screen,(180,180,180),infoRect,2,border_radius=infoRadius)

    for idx,text in enumerate(info):
        screen.blit(font.render(text,True,(0,0,0)),(infoPad+15,infoPad+20+idx*25))

    # ================= LEFT BOTTOM (Skew-T) =================
    skewtRect=pygame.Rect(infoPad,infoH+10,leftW-infoPad*2,H-infoH-infoPad)
    pygame.draw.rect(screen,(245,245,245),skewtRect,border_radius=infoRadius)
    pygame.draw.rect(screen,(180,180,180),skewtRect,2,border_radius=infoRadius)
    skewt.draw(screen,atm,alt,skewtRect,infoRadius)

    # ================= RIGHT (CLOUD) =================
    cloudRect=pygame.Rect(leftW+infoPad,infoPad,W-leftW-infoPad*2,H-infoPad*2)
    pygame.draw.rect(screen,(200,220,255),cloudRect,border_radius=infoRadius)
    pygame.draw.rect(screen,(180,180,180),cloudRect,2,border_radius=infoRadius)
    cloud.draw(screen,alt,atm,cloudRect)

    # ================= SLIDER =================
    #Y 
    pygame.draw.line(screen,(0,0,0),(sliderX,sliderY),(sliderX,sliderY-400), 2)

    #X
    pygame.draw.line(screen,(0,0,0),(sliderX,sliderY),(sliderX+sliderW,sliderY),2)

    knob=sliderX+sliderValue*sliderW
    pygame.draw.circle(screen,(255,0,0),(int(knob),sliderY),12)

    pygame.display.update()

pygame.quit()