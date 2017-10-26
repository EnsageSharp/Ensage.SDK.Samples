// <copyright file="ItemBindingPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace ItemBinding
{
    using System.ComponentModel.Composition;
    using System.Reflection;

    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Item Binding Sample")]
    internal class ItemBindingPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IInventoryManager inventoryManager;

        private readonly Unit owner;

        [ImportingConstructor]
        public ItemBindingPlugin(IServiceContext context)
        {
            // ItemBinding is recommended way of getting items

            this.owner = context.Owner;
            this.inventoryManager = context.Inventory;
        }

        [ItemBinding] // must have this attribute above each item
        private item_ward_observer Observer { get; set; } // must be a property with get; set;

        [ItemBinding]
        private item_soul_ring SoulRing { get; set; } // "accessor is never used" warning can be ignored

        protected override void OnActivate()
        {
            // attach to make item binding work
            this.inventoryManager.Attach(this);

            UpdateManager.Subscribe(this.OnUpdate, 500);
        }

        protected override void OnDeactivate()
        {
            this.inventoryManager.Detach(this);
        }

        private void OnUpdate()
        {
            if (!this.owner.IsAlive)
            {
                return;
            }

            if (this.SoulRing != null && this.SoulRing.CanBeCasted)
            {
                Log.Warn("Using soul ring");
                this.SoulRing.UseAbility();
                return;
            }

            if (this.Observer?.CanBeCasted == true) // merged null && canBeCasted check
            {
                var position = this.owner.InFront(200);
                this.Observer.UseAbility(position); // place ward in front
                Log.Warn("Ward placed at: " + position);
            }
        }
    }
}