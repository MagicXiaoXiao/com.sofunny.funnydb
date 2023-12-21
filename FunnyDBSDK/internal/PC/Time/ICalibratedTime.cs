#if UNITY_STANDALONE || UNITY_EDITOR
using System;

namespace SoFunny.FunnyDB.PC
{
    internal interface ICalibratedTime
    {
        DateTime Get();
        long GetInMills();
    }
}
#endif
