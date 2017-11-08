// <copyright file="PudgePlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Pudge
{
    using System.ComponentModel.Composition;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    [ExportPlugin("Pudge", HeroId.npc_dota_hero_pudge)]
    internal class PudgePlugin : Plugin
    {
        private readonly IServiceContext context;

        private readonly Unit owner;

        private OrbwalkingMode orbwalkingMode;

        private Settings settings;

        [ImportingConstructor]
        public PudgePlugin(IServiceContext context)
        {
            this.context = context;
            this.owner = context.Owner;
        }

        protected override void OnActivate()
        {
            this.settings = new Settings(this.owner.Name);
            this.orbwalkingMode = new OrbwalkingMode(this.context, this.settings);
            this.context.Orbwalker.RegisterMode(this.orbwalkingMode);
        }

        protected override void OnDeactivate()
        {
            this.context.Orbwalker.UnregisterMode(this.orbwalkingMode);
            this.settings.Dispose();
        }
    }
}