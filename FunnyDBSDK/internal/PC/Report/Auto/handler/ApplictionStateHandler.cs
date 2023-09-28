using System;
using UnityEngine;

namespace SoFunny.FunnyDB.PC
{
    internal class ApplictionStateHandler
    {

        internal void Init()
        {
            Application.focusChanged += onAppFocusChanged;
            Application.quitting += onAppQuit;
        }

        private void onAppQuit()
        {
            Logger.LogVerbose("App Quit !!!");
        }

        private void onAppFocusChanged(bool hasFocus)
        {
            if (hasFocus)
            {
                AppBackgroundEvent.EnterForgroundInMills = Environment.TickCount;
                AppForgroundEvent.Track();
            }
            else
            {
                AppForgroundEvent.EnterBackgroundInMills = Environment.TickCount;
                AppBackgroundEvent.Track();
            }
        }
    }
}

