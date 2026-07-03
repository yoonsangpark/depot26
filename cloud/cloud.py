import pygame

class Cloud:

    def __init__(self):
        self.x_ratio=0.5
        self.y_base=500

    def draw(self,screen,alt,atm,rect):

        cx=rect.x+rect.w*self.x_ratio
        cy=rect.y+rect.h*0.8

        y=cy-alt*35

        if atm.LCL==None:
            pygame.draw.circle(screen,(180,180,180),(int(cx),int(y)),40)
            return

        if alt<atm.LCL:
            pygame.draw.circle(screen,(200,200,200),(int(cx),int(y)),45)

        else:

            size=45+(alt-atm.LCL)*12
            size=min(size,120)

            color=(255,255,255)

            if atm.LFC and alt>=atm.LFC:
                color=(240,240,240)

            if atm.EL and alt>=atm.EL:
                color=(210,210,210)

            pygame.draw.circle(screen,color,(int(cx),int(y)),int(size))
            pygame.draw.circle(screen,color,(int(cx+40),int(y-20)),int(size*0.8))
            pygame.draw.circle(screen,color,(int(cx-40),int(y-20)),int(size*0.8))