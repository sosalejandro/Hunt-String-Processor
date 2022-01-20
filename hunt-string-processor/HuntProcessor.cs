using HuntProcessor.Structs;
using System;
using System.Collections.Generic;

namespace HuntProcessor
{
    public class HuntProcessor : IDisposable
    {
        private bool disposedValue;

        public Dictionary<string, HuntAnalyzerDataStruct> ProcessHuntAnalyzerData(string huntAnalyzerData)
        {
            try
            { 
                StringProcessor processor = new();
                AnalyzerDataStruct analyzerData = processor.SegmentString(huntAnalyzerData);
                byte size = processor.ValidateSecondSegmentState(analyzerData.PlayersData);
                processor.SegmentPlayerData(analyzerData.PlayersData, ref size);
                return processor.GetHuntResults();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }            
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HuntProcessor()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
