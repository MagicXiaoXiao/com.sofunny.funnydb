using System;

namespace SoFunny.FunnyDB.PC
{
    internal interface ICalibratedTime
    {
        DateTime Get();
        long GetInMills();
    }
}

