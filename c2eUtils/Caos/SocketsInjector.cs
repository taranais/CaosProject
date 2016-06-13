using System;

namespace C2eUtils.Caos
{
    ///     Linux  ¿ and mac ? version
    ///     TODO
    public class SocketsInjector : ACaosInjector
    {
        public SocketsInjector(string game) : base(game) { }

        public override bool Init(){
            Log.Trace("Injector Init");
            return true;
        }
        public override bool Stop(){
            Log.Trace("Injector Stop");
            return true;
        }

        public override void SendCaos(string CaosAsString, string Action){
            Log.Trace("Injector SendCaos");
        }
    }
}