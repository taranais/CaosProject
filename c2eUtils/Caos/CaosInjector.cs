using System;

namespace C2eUtils.Caos
{    
   /// <summary>
   /// 
   /// </summary>
   public class CaosInjector{
        private ICaosInjector Injector;
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="injector"></param>
        public CaosInjector(ICaosInjector injector)
        {
            Injector= injector;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action"></param>
        public void SendCaosCommand(string CaosAsString, string Action = "execute")
        {
         Injector.SendCaos(CaosAsString, Action);    
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
          return Injector.Init();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
          return Injector.Stop();
        }
    }
}