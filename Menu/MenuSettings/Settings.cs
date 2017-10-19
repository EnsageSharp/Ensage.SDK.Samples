// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace MenuSettings
{
    using System;
    using System.Collections.Generic;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            // main menu
            this.factory = MenuFactory.Create("Sample assembly");

            // toggle button, default enabled
            this.Enabled = this.factory.Item("Enabled", true);
            this.Enabled.Item.Tooltip = "Random tooltip";

            // hold key
            this.HoldKey = this.factory.Item("Combo key", new KeyBind('G'));

            // toggle key
            this.ToggleKey = this.factory.Item("Feature key", new KeyBind('K', KeyBindType.Toggle));

            // sub menu
            var heroesMenu = this.factory.Menu("Heroes");

            // ally hero toggler
            this.AllyHeroes = heroesMenu.Item("Allies:", new HeroToggler(new Dictionary<string, bool>(), false, true, true));

            // enemy hero toggler
            this.EnemyHeroes = heroesMenu.Item("Enemies:", new HeroToggler(new Dictionary<string, bool>(), true, false, true));

            // another sub menu
            var abilitiesMenu = this.factory.Menu("Abilities");

            // abilities
            var abilities = new Dictionary<string, bool>
            {
                // item names: https://developer.valvesoftware.com/wiki/Dota_2_Workshop_Tools/Scripting/Built-In_Item_Names
                // ability names: https://developer.valvesoftware.com/wiki/Dota_2_Workshop_Tools/Scripting/Built-In_Ability_Names

                { "item_blink", true },
                { "abaddon_aphotic_shield", true },
                { "abaddon_borrowed_time", true },
            };

            this.Abilities = abilitiesMenu.Item("Abilities:", new AbilityToggler(abilities));
        }

        public MenuItem<AbilityToggler> Abilities { get; }

        public MenuItem<HeroToggler> AllyHeroes { get; }

        public MenuItem<bool> Enabled { get; }

        public MenuItem<HeroToggler> EnemyHeroes { get; }

        public MenuItem<KeyBind> HoldKey { get; }

        public MenuItem<KeyBind> ToggleKey { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}