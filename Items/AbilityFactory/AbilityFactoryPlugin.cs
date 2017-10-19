// <copyright file="AbilityFactoryPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace AbilityFactory
{
    using System.ComponentModel.Composition;
    using System.Reflection;

    using Ensage;
    using Ensage.SDK.Abilities;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Ability Factory Sample (Items)")]
    internal class AbilityFactoryPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AbilityFactory abilityFactory;

        private readonly Hero owner;

        [ImportingConstructor]
        public AbilityFactoryPlugin(IServiceContext context)
        {
            // its recommended to use ItemBinding to get items

            this.abilityFactory = context.AbilityFactory;
            this.owner = context.Owner as Hero;
        }

        protected override void OnActivate()
        {
            // 500ms timeout between calls
            UpdateManager.Subscribe(this.OnUpdate, 500);
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Unsubscribe(this.OnUpdate);
        }

        private void OnUpdate()
        {
            if (!this.owner.IsAlive)
            {
                return;
            }

            // item names: https://developer.valvesoftware.com/wiki/Dota_2_Workshop_Tools/Scripting/Built-In_Item_Names

            var soulRing = this.abilityFactory.GetItem<item_soul_ring>(); // item by name/class (preferred)
            var arcaneBoots = this.abilityFactory.GetItem(AbilityId.item_arcane_boots); // item by id

            if (soulRing != null && soulRing.CanBeCasted)
            {
                Log.Warn("Using soul ring");
                soulRing.UseAbility();
            }

            if (arcaneBoots != null && arcaneBoots.CanBeCasted)
            {
                Log.Warn("Using arcane boots");
                arcaneBoots.Ability.UseAbility();
                // if we get item by id
                // we cant use some specific ability methods/properties (unless casted)
                // like directly UseAbility() in this case
            }
        }
    }
}