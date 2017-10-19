// <copyright file="AbilityFactoryPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace AbilityFactory
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Ensage;
    using Ensage.SDK.Abilities.npc_dota_hero_zuus;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Ability Factory Sample (Zeus)", HeroId.npc_dota_hero_zuus)]
    internal class AbilityFactoryPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly zuus_arc_lightning arcLightning;

        private readonly zuus_lightning_bolt lightningBolt;

        private readonly Unit owner;

        [ImportingConstructor]
        public AbilityFactoryPlugin(IServiceContext context)
        {
            this.owner = context.Owner;
            var abilityFactory = context.AbilityFactory;

            // some abilities are missing
            // write on discord (#development) if you need them
            // ability names: https://developer.valvesoftware.com/wiki/Dota_2_Workshop_Tools/Scripting/Built-In_Ability_Names
            this.arcLightning = abilityFactory.GetAbility<zuus_arc_lightning>();
            this.lightningBolt = abilityFactory.GetAbility<zuus_lightning_bolt>();

            // also you can get ability by id, but its not preferred
            var thunder = abilityFactory.GetAbility(AbilityId.zuus_thundergods_wrath);
        }

        protected override void OnActivate()
        {
            // 1000ms timeout between calls
            UpdateManager.Subscribe(this.OnUpdate, 1000);
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Unsubscribe(this.OnUpdate);
        }

        private void OnUpdate()
        {
            // its not really a correct way of using multiple abilities and made only for simplicity
            // check SimpleCombo or OrbwalkerAsync for better ability usage

            if (!this.owner.IsAlive)
            {
                return;
            }

            var enemy = EntityManager<Unit>.Entities.FirstOrDefault(x => x.IsValid && x.IsAlive && x.IsEnemy(this.owner));
            if (enemy == null)
            {
                return;
            }

            if (this.arcLightning.CanBeCasted && this.arcLightning.CanHit(enemy))
            {
                this.arcLightning.UseAbility(enemy);
                Log.Warn("Using arc lightning on: " + enemy.GetDisplayName() + "// damage: " + this.arcLightning.GetDamage(enemy));
                return;
            }

            if (this.lightningBolt.CanBeCasted && this.lightningBolt.CanHit(enemy))
            {
                this.lightningBolt.UseAbility(enemy);
                Log.Warn("Using lightning bolt on: " + enemy.GetDisplayName() + "// damage: " + this.lightningBolt.GetDamage(enemy));
            }
        }
    }
}