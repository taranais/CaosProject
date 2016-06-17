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
        /// <summary>
        /// Configure and Starts CaosInjector
        /// </summary>
        /// <returns></returns>
        bool Init();

        /// <summary>
        /// Stops CaosInjector
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// Send Caos over injector
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        CaosResult SendCaos(string CaosAsString, string Action);
    }

    /// <summary>
    ///
    /// </summary>
    public abstract class ACaosInjector : ICaosInjector
    {
        protected Logger Log { get; private set; }
        /// <summary>
        ///  gets Game's name
        /// </summary>
        /// <returns></returns>
        public string Game {get; private set;}

        /// <summary>
        /// Create CaosInjector for especified engine game
        /// </summary>
        /// <param name="game">especified engine's game name</param>
        protected ACaosInjector (string game)
        {
            Game = game;
            Log = LogManager.GetLogger(GetType().FullName);
            Log.Trace("Injector Created");
        }

        /// <inheritdoc/>
        public abstract bool Init();
        /// <inheritdoc/>
        public abstract bool Stop();
        /// <inheritdoc/>
        public abstract CaosResult SendCaos(string CaosAsString, string Action);
    }
}