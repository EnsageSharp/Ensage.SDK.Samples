// <copyright file="SimpleAsyncPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace SimpleAsync
{
    using System.Reflection;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Simple Async Sample")]
    internal class SimpleAsyncPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected override void OnActivate()
        {
            Entity.OnParticleEffectAdded += this.OnParticleEffectAdded;
        }

        protected override void OnDeactivate()
        {
            Entity.OnParticleEffectAdded -= this.OnParticleEffectAdded;
        }

        private async void OnParticleEffectAdded(Entity sender, ParticleEffectAddedEventArgs args)
        {
            Log.Warn("Particle added: " + args.Name);

            await Task.Delay(1000);

            Log.Warn("Task completed");
        }
    }
}