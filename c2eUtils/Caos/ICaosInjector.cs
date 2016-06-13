using System;
using NLog;

namespace C2eUtils.Caos
{
    /// TODO async


    /// <summary>
    ///
    /// </summary>
    public interface ICaosInjector
    {
        bool Init();
        bool Stop();
        void SendCaos(string CaosAsString, string Action);
    }

    /// <summary>
    ///
    /// </summary>
    public abstract class ACaosInjector : ICaosInjector
    {
        protected Logger Log { get; private set; }
        public string Game {get; private set;}

        /// <summary>
        ///
        /// </summary>
        /// <param name="game"></param>
        protected ACaosInjector (string game)
        {
            Game = game;
            Log = LogManager.GetLogger(GetType().FullName);
            Log.Trace("Injector Created");
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public abstract bool Init();

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public abstract bool Stop();

        /// <summary>
        ///
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action"></param>
        public abstract void SendCaos(string CaosAsString, string Action);
    }
}