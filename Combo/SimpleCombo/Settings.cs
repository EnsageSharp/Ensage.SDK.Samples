// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace SimpleCombo
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings()
        {
            this.factory = MenuFactory.Create("Combo Sample");
            this.HoldKey = this.factory.Item("Key", new KeyBind('D'));
        }

        public MenuItem<KeyBind> HoldKey { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}