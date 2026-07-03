import pygame

class SkewT:

    def __init__(self):
        self.skew=18

    def draw(self,screen,atm,alt,rect):

        x0=rect.x+20
        y0=rect.y+20
        w=rect.w-40
        h=rect.h-40

        pygame.draw.rect(screen,(255,255,255),rect)

        for i in range(len(atm.alt)-1):

            x1=x0+atm.envTemp[i]*2 + self.skew*atm.alt[i]*8
            y1=y0+h-(atm.alt[i]/atm.maxAlt)*h

            x2=x0+atm.envTemp[i+1]*2 + self.skew*atm.alt[i+1]*8
            y2=y0+h-(atm.alt[i+1]/atm.maxAlt)*h

            pygame.draw.line(screen,(0,0,255),(x1,y1),(x2,y2),1)

        i=int((alt/atm.maxAlt)*(len(atm.alt)-1))

        x=x0+atm.airTemp[i]*2 + self.skew*atm.alt[i]*8
        y=y0+h-(atm.alt[i]/atm.maxAlt)*h

        pygame.draw.circle(screen,(0,0,0),(int(x),int(y)),5)