class Atmosphere:

    def __init__(self,surfTemp=35,surfDP=28,envLR=6.5,dryLR=9.8,moistLR=4.5,dpLR=2.0,maxAlt=10):

        self.surfTemp=surfTemp
        self.surfDP=surfDP
        self.envLR=envLR
        self.dryLR=dryLR
        self.moistLR=moistLR
        self.dpLR=dpLR
        self.maxAlt=maxAlt

    def calc(self):

        self.alt=[round(0.1*i,2)for i in range(int(self.maxAlt*10)+1)]
        self.envTemp=[round(self.surfTemp-self.envLR*i,2) for i in self.alt]
        self.dp=[round(self.surfDP-self.dpLR*i,2) for i in self.alt]

        self.airTemp=[]
        self.LCL=None
        self.LCLindex=None
        self.LFC=None
        self.EL=None

        for i in range(len(self.alt)):

            t=round(self.surfTemp-self.dryLR*self.alt[i],2)
            self.airTemp.append(t)

            if self.LCL==None and t<=self.dp[i]:
                self.LCL=self.alt[i]
                self.LCLindex=i

        if self.LCLindex!=None:

            base=self.airTemp[self.LCLindex]

            for i in range(self.LCLindex+1,len(self.alt)):

                d=self.alt[i]-self.LCL
                self.airTemp[i]=round(base-self.moistLR*d,2)

        if self.LCLindex!=None:

            for i in range(len(self.alt)):

                if self.airTemp[i]>self.envTemp[i]:
                    self.LFC=self.alt[i]
                    break

        if self.LFC!=None:

            for i in range(len(self.alt)):

                if self.alt[i]>self.LFC:
                    self.EL=self.alt[i]
                    break