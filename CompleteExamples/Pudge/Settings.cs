// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Pudge
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings(string ownerName)
        {
            this.factory = MenuFactory.CreateWithTexture("Pudgerino", ownerName);
            this.ComboKey = this.factory.Item("Combo", new KeyBind('F'));
        }

        public MenuItem<KeyBind> ComboKey { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}