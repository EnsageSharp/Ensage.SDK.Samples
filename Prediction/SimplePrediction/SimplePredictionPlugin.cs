// <copyright file="SimplePredictionPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace SimplePrediction
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Reflection;

    using Ensage;
    using Ensage.SDK.Abilities.npc_dota_hero_pudge;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;
    using Ensage.SDK.TargetSelector;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    using SharpDX;

    [ExportPlugin("Simple Prediction Sample", HeroId.npc_dota_hero_pudge)]
    internal class SimplePredictionPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly pudge_meat_hook hook;

        private readonly ITargetSelectorManager targetManager;

        [ImportingConstructor]
        public SimplePredictionPlugin(IServiceContext context)
        {
            this.targetManager = context.TargetSelector;
            this.hook = context.AbilityFactory.GetAbility<pudge_meat_hook>();
        }

        protected override void OnActivate()
        {
            Drawing.OnDraw += this.OnDraw;
        }

        protected override void OnDeactivate()
        {
            Drawing.OnDraw -= this.OnDraw;
        }

        private void OnDraw(EventArgs args)
        {
            var target = this.targetManager.Active.GetTargets().FirstOrDefault(x => this.hook.CanHit(x));
            if (target != null)
            {
                var input = this.hook.GetPredictionInput(target);
                var output = this.hook.GetPredictionOutput(input);

                Vector2 screenPos;
                if (Drawing.WorldToScreen(output.CastPosition, out screenPos))
                {
                    Drawing.DrawCircle(screenPos, 40, 64, Color.Red);
                }

                if (Drawing.WorldToScreen(output.UnitPosition, out screenPos))
                {
                    Drawing.DrawCircle(screenPos, 40, 64, Color.Green);
                }
            }
        }
    }
}