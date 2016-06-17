using System;

namespace C2eUtils.Caos
{
    /// <summary>
    ///
    /// </summary>
    public class CaosInjector{
        private ICaosInjector Injector;

        /// <summary>
        /// Create CaosInjector with especified ICaosInjector
        /// </summary>
        /// <param name="injector"></param>
        public CaosInjector(ICaosInjector injector)
        {
            Injector= injector;
        }

        /// <summary>
        /// Send Caos over injector
        /// </summary>
        /// <param name="CaosAsString"></param>
        /// <param name="Action">default: execute</param>
        public CaosResult SendCaosCommand(string CaosAsString, string Action = "execute")
        {
            return Injector.SendCaos(CaosAsString, Action);
        }

        /// <summary>
        /// Configure and Starts CaosInjector
        /// </summary>
        /// <returns></returns>
        public bool Init()
        {
            return Injector.Init();
        }

        /// <summary>
        /// Stops CaosInjector
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            return Injector.Stop();
        }
    }
}