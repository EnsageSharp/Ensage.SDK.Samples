// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Pudge
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings(string ownerName)
        {
            this.factory = MenuFactory.CreateWithTexture("Pudgerino", ownerName);
            this.ComboKey = this.factory.Item("Combo", new KeyBind('F'));
            this.DrawTargetParticle = this.factory.Item("Draw target particle", true);
            this.Distance = this.factory.Item("Enemy distance", new Slider(2000, 500, 3000));

            var items = new List<AbilityId>
            {
                AbilityId.item_blink,
                AbilityId.item_force_staff,
                AbilityId.item_urn_of_shadows
            };

            this.Items = this.factory.Item("Items", new AbilityToggler(items.ToDictionary(x => x.ToString(), x => true)));
        }

        public MenuItem<KeyBind> ComboKey { get; }

        public MenuItem<Slider> Distance { get; }

        public MenuItem<bool> DrawTargetParticle { get; }

        public MenuItem<AbilityToggler> Items { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}