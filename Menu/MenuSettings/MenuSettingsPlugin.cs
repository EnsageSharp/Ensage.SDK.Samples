// <copyright file="MenuSettingsPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace MenuSettings
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Ensage;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    [ExportPlugin("Menu Settings Sample")]
    internal class MenuSettingsPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Hero owner;

        private Settings settings;

        [ImportingConstructor]
        public MenuSettingsPlugin(IServiceContext context)
        {
            this.owner = context.Owner as Hero; // we can cast owner to hero
        }

        protected override void OnActivate()
        {
            // its better to make own class for settings
            this.settings = new Settings();

            UpdateManager.Subscribe(this.OnUpdate, 100);

            // enabled button changed event
            this.settings.Enabled.PropertyChanged += this.EnabledOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Unsubscribe(this.OnUpdate);
            this.settings.Enabled.PropertyChanged -= this.EnabledOnPropertyChanged;
            this.settings.Dispose();
        }

        private void EnabledOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (this.settings.Enabled)
            {
                Log.Warn("Button enabled");
            }
            else
            {
                Log.Warn("Button disabled");
            }
        }

        private void OnUpdate()
        {
            if (this.settings.ToggleKey) // check toggle key
            {
                var allyHeroes = EntityManager<Hero>.Entities.Where(x => x.IsValid && x.IsAlive && x.IsAlly(this.owner));

                foreach (var allyHero in allyHeroes)
                {
                    if (this.settings.AllyHeroes.Value.IsEnabled(allyHero.Name))
                    {
                        // ally hero enabled in settings
                        Log.Warn(allyHero.Name);

                        if (this.settings.Abilities.Value.IsEnabled("abaddon_aphotic_shield"))
                        {
                            // aphotic shield is enabled
                        }
                    }
                }
            }

            if (this.settings.HoldKey) // check hold key
            {
                var closestEnabledEnemy = EntityManager<Hero>.Entities
                    .Where(x => x.IsValid && x.IsAlive && this.settings.EnemyHeroes.Value.IsEnabled(x.Name))
                    .OrderBy(x => x.Distance2D(this.owner))
                    .FirstOrDefault();

                if (closestEnabledEnemy != null)
                {
                    // closest enemy enabled in settings
                    Log.Warn(closestEnabledEnemy.Name);
                }
            }
        }
    }
}