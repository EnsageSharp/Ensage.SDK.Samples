// <copyright file="LoopPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Loop
{
    using System;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Async Loop Sample")]
    internal class LoopPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private TaskHandler task;

        protected override void OnActivate()
        {
            // loop1
            // preferred when u need to run/cancel task multiple times (i.e. on combo)
            // check SimpleCombo for better example
            this.task = new TaskHandler(this.AsyncLoop1, true);
            this.task.RunAsync();

            // loop2
            UpdateManager.BeginInvoke(this.AsyncLoop2);
        }

        protected override void OnDeactivate()
        {
            this.task.Cancel();
        }

        private async Task AsyncLoop1(CancellationToken token)
        {
            try
            {
                Log.Warn("Async loop1 start");

                await Task.Delay(2000, token);

                Log.Warn("Async loop1 end");
            }
            catch (TaskCanceledException)
            {
                // task canceled
                // exception can be ignored
            }
            catch (Exception e)
            {
                // if we wont catch exceptions in this loop task will just stop!
                Log.Error(e);
            }
        }

        private async void AsyncLoop2()
        {
            while (this.IsActive)
            {
                Log.Warn("Async loop2 start");

                await Task.Delay(500);

                Log.Warn("Async loop2 end");

                // this loop must have small delay on each cycle or it will freeze the game!
                await Task.Delay(25);
            }
        }
    }
}