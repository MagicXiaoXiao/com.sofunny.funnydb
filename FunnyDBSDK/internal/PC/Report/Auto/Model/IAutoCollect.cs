namespace SoFunny.FunnyDB.PC
{
    internal interface IAutoCollect
    {
        string GetReport();

        string GetEventName();

        bool IsNeedReport();
    }
}