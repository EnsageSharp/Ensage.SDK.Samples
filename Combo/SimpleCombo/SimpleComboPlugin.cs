// <copyright file="SimpleComboPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace SimpleCombo
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_zuus;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;
    using Ensage.SDK.TargetSelector;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Simple Combo Sample (Zeus)", HeroId.npc_dota_hero_zuus)]
    internal class SimpleComboPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly zuus_arc_lightning arcLightning;

        private readonly IInventoryManager inventoryManager;

        private readonly zuus_lightning_bolt lightningBolt;

        private readonly Unit owner;

        private readonly ITargetSelectorManager targetSelector;

        private readonly zuus_thundergods_wrath thundergodsWrath;

        private TaskHandler comboTask;

        private Settings settings;

        [ImportingConstructor]
        public SimpleComboPlugin(IServiceContext context)
        {
            // combo example on key press without orbwalker!
            // check OrbwalkerAsync sample if u need orbwalker in combo

            this.owner = context.Owner;
            this.inventoryManager = context.Inventory;
            this.targetSelector = context.TargetSelector;

            this.arcLightning = context.AbilityFactory.GetAbility<zuus_arc_lightning>();
            this.lightningBolt = context.AbilityFactory.GetAbility<zuus_lightning_bolt>();
            this.thundergodsWrath = context.AbilityFactory.GetAbility<zuus_thundergods_wrath>();
        }

        [ItemBinding]
        private item_veil_of_discord Veil { get; set; }

        protected override void OnActivate()
        {
            this.inventoryManager.Attach(this);
            this.settings = new Settings();
            this.comboTask = new TaskHandler(this.Combo, true);
            this.settings.HoldKey.PropertyChanged += this.HoldKeyOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            this.inventoryManager.Detach(this);
            this.settings.HoldKey.PropertyChanged -= this.HoldKeyOnPropertyChanged;
            this.settings.Dispose();
        }

        private async Task Combo(CancellationToken cancellationToken)
        {
            try
            {
                if (!this.owner.IsAlive)
                {
                    return;
                }

                var target = this.targetSelector.Active.GetTargets().FirstOrDefault(x => x.Distance2D(this.owner) < 1000);
                if (target == null)
                {
                    return;
                }

                var combo = new Combo(this.Veil, this.arcLightning, this.lightningBolt, this.thundergodsWrath);
                if (!combo.IsInRange(target))
                {
                    return;
                }

                if (combo.GetDamage(target) > target.Health)
                {
                    Log.Warn("Target should die");
                }

                await combo.Execute(target, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                // must catch exceptions or task will stop
                Log.Error(e);
            }
        }

        private void HoldKeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.HoldKey)
            {
                Log.Warn("executing combo");
                this.comboTask.RunAsync();
            }
            else
            {
                Log.Warn("combo stopped");
                this.comboTask.Cancel();
            }
        }
    }
}