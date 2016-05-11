using System;

using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;

namespace C2eUtils.Caos
{   
    ///    windows version
    ///     TODO
    public class SharedMemoryInjector : ACaosInjector
    {
        private Mutex mutex;
        private MemoryMappedFile memfile;
        private MemoryMappedViewAccessor memViewAccessor;
        private EventWaitHandle resultEventHandle;
        private EventWaitHandle requestEventHandle;
        
        public SharedMemoryInjector(string game) : base(game) { }
        
        public override bool Init(){    
            bool exito= true;
            try
            {
                Log.Trace("Injector {0} Init", Game);
                mutex = Mutex.OpenExisting(Game + "_mutex");
                memfile = MemoryMappedFile.OpenExisting(Game + "_mem");
                memViewAccessor = memfile.CreateViewAccessor();
                resultEventHandle = EventWaitHandle.OpenExisting(Game + "_result");         
                requestEventHandle = EventWaitHandle.OpenExisting(Game + "_request");
            }
            catch(System.Threading.WaitHandleCannotBeOpenedException ex)
            {
                Log.Error(ex, "Shared Memory not created for: {0}",Game);
                exito = false;
            }
            finally
            {
                Log.Trace("Injector {0} Init : {1}", Game, exito);
            }
            return exito;
        }
        public override bool Stop(){
            bool exito= true;
            try{
                Log.Trace("Injector {0} Stop",Game);   
                requestEventHandle.Dispose();
                resultEventHandle.Dispose();
                memViewAccessor.Dispose();
                memfile.Dispose();
                mutex.Dispose();
            } 
            catch (System.NullReferenceException ex)
            {
                Log.Error(ex, "Shared Memory fail on Stop: {0}",Game);
                exito = false;
            }
            finally
            {
                Log.Trace("Injector {0} Stop : {1}", Game, exito);
            }
            return exito;
        }
        
        public override void SendCaos(string CaosAsString, string Action){
                Log.Trace("Executing Caos for {0}",Game);
                CaosResult caosResult = null;
                mutex.WaitOne();
                BufferLayout CaosBuffer = new BufferLayout();
                CaosBuffer.PrepareBufferLayout(CaosAsString,Action);
                CaosBuffer.SetSharedMemory(memViewAccessor);
                requestEventHandle.Set();
                resultEventHandle.WaitOne();
                CaosBuffer.GetSharedMemory(memViewAccessor);
                mutex.ReleaseMutex();
                caosResult = CaosBuffer.GetCaosResult();
                Log.Trace("Caos result fail : {0} Content: {1} ",caosResult.Failed,caosResult.Content);
        }
    }
}