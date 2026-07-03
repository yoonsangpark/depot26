import pygame

class SkewT:

    def __init__(self):
        self.skew=18

    def draw(self,screen,atm,alt,rect,borderRadius=16):

        margin=borderRadius+22
        plot=pygame.Rect(margin,margin,rect.w-margin*2,rect.h-margin*2)
        if plot.w<1 or plot.h<1:
            return

        def raw_x(temp,altKm):
            return temp*2+self.skew*altKm*8

        xs=[raw_x(atm.envTemp[i],atm.alt[i]) for i in range(len(atm.alt))]
        xs+=[raw_x(atm.airTemp[i],atm.alt[i]) for i in range(len(atm.alt))]
        xMin,xMax=min(xs),max(xs)
        xSpan=xMax-xMin or 1

        def mapX(temp,altKm):
            return (raw_x(temp,altKm)-xMin)/xSpan*plot.w

        def mapY(altKm):
            return plot.h-(altKm/atm.maxAlt)*plot.h

        plotSurf=pygame.Surface((plot.w,plot.h),pygame.SRCALPHA)
        plotSurf.fill((0,0,0,0))

        for i in range(len(atm.alt)-1):
            pygame.draw.line(
                plotSurf,(0,0,255),
                (mapX(atm.envTemp[i],atm.alt[i]),mapY(atm.alt[i])),
                (mapX(atm.envTemp[i+1],atm.alt[i+1]),mapY(atm.alt[i+1])),1
            )

        iMax=int((alt/atm.maxAlt)*(len(atm.alt)-1))
        for i in range(iMax):
            pygame.draw.line(
                plotSurf,(0,0,0),
                (mapX(atm.airTemp[i],atm.alt[i]),mapY(atm.alt[i])),
                (mapX(atm.airTemp[i+1],atm.alt[i+1]),mapY(atm.alt[i+1])),1
            )

        mx=mapX(atm.airTemp[iMax],atm.alt[iMax])
        my=mapY(atm.alt[iMax])
        r=5
        if r<=mx<=plot.w-r and r<=my<=plot.h-r:
            pygame.draw.circle(plotSurf,(0,0,0),(int(mx),int(my)),r)

        content=pygame.Surface((rect.w,rect.h),pygame.SRCALPHA)
        content.fill((0,0,0,0))
        content.blit(plotSurf,plot.topleft)

        shape=pygame.Surface((rect.w,rect.h),pygame.SRCALPHA)
        shape.fill((0,0,0,0))
        pygame.draw.rect(shape,(255,255,255,255),shape.get_rect(),border_radius=borderRadius)
        shape.blit(content,(0,0),special_flags=pygame.BLEND_RGBA_MULT)

        screen.blit(shape,rect.topleft)
