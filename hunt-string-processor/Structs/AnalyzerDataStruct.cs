using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuntProcessor.Structs
{
    internal struct AnalyzerDataStruct
    {
        public readonly string HuntData;
        public readonly string PlayersData;

        public AnalyzerDataStruct(string huntData, string playersData)
        {
            HuntData = huntData;
            PlayersData = playersData;
        }
    }
}
