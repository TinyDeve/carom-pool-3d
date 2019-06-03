using strange.extensions.command.impl;
using UnityEngine;

namespace onur.pool.commands
{
    public class StartAppCommand : Command
    {
        public override void Execute()
        {
            ConfigureApp();
        }

        private void ConfigureApp()
        {
            Application.targetFrameRate = 60;   
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }
    }
}