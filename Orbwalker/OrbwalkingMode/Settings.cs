// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace OrbwalkingMode
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings(MenuFactory parent)
        {
            this.factory = parent.Menu("Zeus Q farm mode");

            this.Active = this.factory.Item("Active", true);
            this.Key = this.factory.Item("Key", new KeyBind('D'));
        }

        public MenuItem<bool> Active { get; }

        public MenuItem<KeyBind> Key { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}