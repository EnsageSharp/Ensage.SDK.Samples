// <copyright file="SimpleAsyncPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace ParticleManager
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Renderer.Particle;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    using SharpDX;

    [ExportPlugin("Particle Manager Sample")]
    internal class SimpleAsyncPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Unit owner;

        private readonly IParticleManager particleManager;

        [ImportingConstructor]
        public SimpleAsyncPlugin(IServiceContext context)
        {
            this.owner = context.Owner;
            this.particleManager = context.Particle;
        }

        protected override void OnActivate()
        {
            // draw circle under our hero
            this.particleManager.DrawRange(this.owner, "CircleSample", 111, Color.Red);

            UpdateManager.Subscribe(this.OnUpdate);
        }

        protected override void OnDeactivate()
        {
            this.particleManager.Remove("CircleSample");
            this.particleManager.Remove("TargetSample");

            UpdateManager.Unsubscribe(this.OnUpdate);
        }

        private void OnUpdate()
        {
            var unit = EntityManager<Unit>.Entities.OrderBy(x => x.Distance2D(this.owner))
                .FirstOrDefault(x => x.IsValid && x.IsAlive && x != this.owner);

            if (unit != null)
            {
                // draw target on closest unit
                this.particleManager.DrawTargetLine(this.owner, "TargetSample", unit.Position);
            }
        }
    }
}