#if UNITY_STANDALONE || UNITY_EDITOR
using System.Collections.Generic;

namespace SoFunny.FunnyDB.PC
{
    internal interface IAutoCollect
    {
        Dictionary<string, object> GetReport();

        string GetEventName();

        bool IsNeedReport();
    }
}
#endif