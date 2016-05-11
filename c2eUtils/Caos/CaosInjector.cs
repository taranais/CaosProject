using System;

namespace C2eUtils.Caos
{    
   public class CaosInjector{
        private ICaosInjector Injector;
        
        public CaosInjector(ICaosInjector injector)
        {
            Injector= injector;
        }
        
        public void SendCaosCommand(string CaosAsString, string Action = "execute")
        {
         Injector.SendCaos(CaosAsString, Action);    
        }
        
        public bool Init()
        {
          return Injector.Init();
        }
        
        public bool Stop()
        {
          return Injector.Stop();
        }
    }
}