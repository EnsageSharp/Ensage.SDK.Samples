// <copyright file="BeginInvokePlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace BeginInvoke
{
    using System.Reflection;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Begin Invoke Sample")]
    internal class BeginInvokePlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnActivate()
        {
            UpdateManager.BeginInvoke(() => { Log.Warn("No Async, just 3000ms delay"); }, 3000);

            UpdateManager.BeginInvoke(
                async () =>
                    {
                        await Task.Delay(2000);
                        Log.Warn("Async 3000ms+2000ms delay");
                    },
                3000);

            UpdateManager.BeginInvoke(this.Callback, 6000);
        }

        private async void Callback()
        {
            Log.Warn("Async method 6000ms delay with time task");

            await this.WriteTime();

            Log.Warn("Time awaited");
        }

        private async Task WriteTime()
        {
            Log.Warn("Time: " + Game.RawGameTime);
            await Task.Delay(1000);
        }
    }
}