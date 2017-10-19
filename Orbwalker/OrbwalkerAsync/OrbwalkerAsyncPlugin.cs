// <copyright file="OrbwalkerAsyncPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace OrbwalkerAsync
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Reflection;

    using Ensage;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Orbwalker Async Sample (Drow)", HeroId.npc_dota_hero_drow_ranger)]
    internal class OrbwalkerAsyncPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private readonly Unit owner;

        private OrbwalkingMode orbwalkingMode;

        private Settings settings;

        [ImportingConstructor]
        public OrbwalkerAsyncPlugin(IServiceContext context)
        {
            this.context = context;
            this.owner = context.Owner;
        }

        protected override void OnActivate()
        {
            this.settings = new Settings(this.owner.Name);
            this.orbwalkingMode = new OrbwalkingMode(this.context, this.settings.HoldKey);
            this.context.Orbwalker.RegisterMode(this.orbwalkingMode);

            this.settings.HoldKey.PropertyChanged += this.HoldKeyOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            this.settings.HoldKey.PropertyChanged -= this.HoldKeyOnPropertyChanged;
            this.context.Orbwalker.UnregisterMode(this.orbwalkingMode);
            this.settings.Dispose();
        }

        private void HoldKeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.HoldKey != this.orbwalkingMode.Key)
            {
                // change orbwalking mode key if settings changed
                this.orbwalkingMode.Key = this.settings.HoldKey;
            }
        }
    }
}