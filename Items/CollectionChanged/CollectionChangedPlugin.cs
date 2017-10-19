// <copyright file="CollectionChangedPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace CollectionChanged
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Ensage.SDK.Inventory;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Collection Changed Sample (Items)")]
    internal class CollectionChangedPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly List<InventoryItem> inventoryItems = new List<InventoryItem>();

        private readonly IInventoryManager inventoryManager;

        [ImportingConstructor]
        public CollectionChangedPlugin(IServiceContext context)
        {
            this.inventoryManager = context.Inventory;
        }

        protected override void OnActivate()
        {
            this.inventoryManager.CollectionChanged += this.InventoryManagerOnCollectionChanged;
        }

        protected override void OnDeactivate()
        {
            this.inventoryManager.CollectionChanged -= this.InventoryManagerOnCollectionChanged;
        }

        private void InventoryManagerOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in args.NewItems.OfType<InventoryItem>())
                {
                    Log.Warn("Item added: " + item.Id);
                    this.inventoryItems.Add(item);
                }
            }

            if (args.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in args.OldItems.OfType<InventoryItem>())
                {
                    Log.Warn("Item removed: " + item.Id);
                    this.inventoryItems.Remove(item);
                }
            }

            Log.Warn("Items in inventory: " + this.inventoryItems.Count);
        }
    }
}