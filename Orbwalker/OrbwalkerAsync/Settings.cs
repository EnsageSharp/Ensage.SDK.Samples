// <copyright file="Settings.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace OrbwalkerAsync
{
    using System;

    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    internal class Settings : IDisposable
    {
        private readonly MenuFactory factory;

        public Settings(string ownerName)
        {
            this.factory = MenuFactory.CreateWithTexture("Sample", ownerName);
            this.HoldKey = this.factory.Item("Key", new KeyBind('G'));
        }

        public MenuItem<KeyBind> HoldKey { get; }

        public void Dispose()
        {
            this.factory.Dispose();
        }
    }
}