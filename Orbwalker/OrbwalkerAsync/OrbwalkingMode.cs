// <copyright file="OrbwalkingMode.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace OrbwalkerAsync
{
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage.Common.Menu;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_drow_ranger;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    internal class OrbwalkingMode : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly drow_ranger_frost_arrows frostArrows;

        private readonly ITargetSelectorManager targetSelector;

        public OrbwalkingMode(IServiceContext context, MenuItem<KeyBind> key)
            : base(context, key)
        {
            this.targetSelector = context.TargetSelector;
            this.frostArrows = context.AbilityFactory.GetAbility<drow_ranger_frost_arrows>();
            context.Inventory.Attach(this);
        }

        [ItemBinding]
        private item_mask_of_madness Madness { get; set; }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            var target = this.targetSelector.Active.GetTargets().FirstOrDefault(x => x.Distance2D(this.Owner) <= this.Owner.AttackRange());

            if (target != null)
            {
                if (this.frostArrows.CanBeCasted && !target.HasModifier(this.frostArrows.TargetModifierName))
                {
                    // use frost arrows only when target doesnt have slow debuff
                    this.frostArrows.UseAbility(target);
                    await Task.Delay(this.frostArrows.GetCastDelay(target), token);
                }

                if (this.Madness?.CanBeCasted == true)
                {
                    this.Madness.UseAbility();
                }
            }

            // if target == null
            // hero will move to mouse position
            this.Orbwalker.OrbwalkTo(target);
        }

        protected override void OnDeactivate()
        {
            this.Context.Inventory.Detach(this);
            base.OnDeactivate();
        }
    }
}