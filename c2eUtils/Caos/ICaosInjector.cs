using System;
using NLog;

namespace C2eUtils.Caos
{
    ///     
    /// TODO async
    public interface ICaosInjector
    {   
        bool Init();
        bool Stop();
        void SendCaos(string CaosAsString, string Action); 
    }
    
    public abstract class ACaosInjector : ICaosInjector
    {
        protected Logger Log { get; private set; }
        public string Game {get; private set;}
        protected ACaosInjector (string game)
        {
            Game = game;
            Log = LogManager.GetLogger(GetType().FullName);
            Log.Trace("Injector Created");
        }
        public abstract bool Init();
        public abstract bool Stop();
        public abstract void SendCaos(string CaosAsString, string Action);
    }
}