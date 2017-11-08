// <copyright file="OrbwalkingMode.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Pudge
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    using Ensage;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_pudge;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Geometry;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Orbwalker.Modes;
    using Ensage.SDK.Prediction;
    using Ensage.SDK.Service;
    using Ensage.SDK.TargetSelector;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    using SharpDX;

    internal class OrbwalkingMode : KeyPressOrbwalkingModeAsync
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IServiceContext context;

        private readonly pudge_meat_hook hook;

        private readonly IUpdateHandler hookUpdateHandler;

        private readonly pudge_rot rot;

        private readonly Settings settings;

        private readonly ITargetSelectorManager targetSelector;

        private readonly pudge_dismember ult;

        private Vector3 hookCastPosition;

        private float hookStartCastTime;

        private Unit target;

        public OrbwalkingMode(IServiceContext context, Settings settings)
            : base(context, settings.ComboKey)
        {
            var abilities = context.AbilityFactory;
            this.hook = abilities.GetAbility<pudge_meat_hook>();
            this.rot = abilities.GetAbility<pudge_rot>();
            this.ult = abilities.GetAbility<pudge_dismember>();
            this.targetSelector = context.TargetSelector;
            this.settings = settings;
            this.context = context;
            this.hookUpdateHandler = UpdateManager.Subscribe(this.HookHitCheck, 0, false);
        }

        [ItemBinding]
        private item_blink Blink { get; set; }

        [ItemBinding]
        private item_force_staff Force { get; set; }

        [ItemBinding]
        private item_urn_of_shadows Urn { get; set; }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                this.target = this.targetSelector.Active.GetTargets().FirstOrDefault(x => x.Distance2D(this.Owner) <= 2500);
                if (this.target == null)
                {
                    return;
                }

                if (this.Blink?.CanBeCasted == true && this.Owner.Distance2D(this.target) > 600)
                {
                    var pos = this.target.NetworkPosition.Extend(this.Owner.NetworkPosition, 100);
                    this.Blink.UseAbility(pos);

                    if (this.ult.CanBeCasted && pos.Distance2D(this.target.NetworkPosition) < this.ult.CastRange)
                    {
                        // use everything now for smaller delay
                        this.rot.Enabled = true;
                        this.ult.UseAbility(this.target);
                        await Task.Delay(this.ult.GetCastDelay(this.target), token);
                    }
                    else
                    {
                        await Task.Delay(this.Blink.GetCastDelay(this.target), token);
                    }
                }

                if (this.Force?.CanBeCasted == true && this.Owner.Distance2D(this.target) > 600)
                {
                    this.Owner.Move(this.Owner.NetworkPosition.Extend(this.target.NetworkPosition, 50));
                    this.Force.Ability.UseAbility(this.Owner, true);
                    await Task.Delay(this.Force.GetCastDelay() + 500, token);
                    if (this.ult.CanBeCasted && this.Owner.Distance2D(this.target.NetworkPosition) < this.ult.CastRange)
                    {
                        // use everything now for smaller delay
                        this.rot.Enabled = true;
                        this.ult.UseAbility(this.target);
                        await Task.Delay(this.ult.GetCastDelay(this.target), token);
                    }
                    else
                    {
                        await Task.Delay(this.Force.GetCastDelay(this.target), token);
                    }
                }

                if (this.rot.CanBeCasted && !this.rot.Enabled
                    && (this.rot.CanHit(this.target) || this.target.HasModifier(this.hook.TargetModifierName)))
                {
                    this.rot.Enabled = true;
                    await Task.Delay(this.rot.GetCastDelay(), token);
                }

                if (this.Urn?.CanBeCasted == true && this.target.HasAnyModifiers(
                        this.hook.TargetModifierName,
                        this.ult.TargetModifierName,
                        this.rot.TargetModifierName))
                {
                    this.Urn.UseAbility(this.target);
                    await Task.Delay(this.Urn.GetCastDelay(this.target), token);
                }

                if (this.ult.CanBeCasted && (this.ult.CanHit(this.target) || this.target.HasModifier(this.hook.TargetModifierName)))
                {
                    this.ult.UseAbility(this.target);
                    await Task.Delay(this.ult.GetCastDelay(this.target) + 500, token);
                }

                if (this.hook.CanBeCasted && this.hook.CanHit(this.target) && !this.ult.IsChanneling)
                {
                    // we need prediction output so we will do it manually
                    // but hook.UseAbility(target) will also use prediction

                    var input = this.hook.GetPredictionInput(this.target);
                    var output = this.hook.GetPredictionOutput(input);

                    if (this.ShouldCastHook(output))
                    {
                        this.hookCastPosition = output.UnitPosition;
                        this.hook.UseAbility(this.hookCastPosition);
                        await Task.Delay(this.hook.GetHitTime(this.hookCastPosition), token);
                    }
                }

                if (!this.ult.IsChanneling)
                {
                    this.Orbwalker.OrbwalkTo(this.target);
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.context.Inventory.Attach(this);

            Entity.OnBoolPropertyChange += this.EntityOnBoolPropertyChange;
            this.MenuKey.PropertyChanged += this.MenuKeyOnPropertyChanged;
        }

        protected override void OnDeactivate()
        {
            this.Context.Inventory.Detach(this);
            Entity.OnBoolPropertyChange -= this.EntityOnBoolPropertyChange;
            this.MenuKey.PropertyChanged -= this.MenuKeyOnPropertyChanged;
            base.OnDeactivate();
        }

        private void EntityOnBoolPropertyChange(Entity sender, BoolPropertyChangeEventArgs args)
        {
            if (!this.CanExecute)
            {
                return;
            }

            if (sender.Handle != this.hook.Ability.Handle || args.NewValue == args.OldValue || args.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            if (this.target == null || !this.target.IsValid || !this.target.IsVisible)
            {
                return;
            }

            if (args.NewValue)
            {
                // start HookHitCheck task when we casted it
                this.hookStartCastTime = Game.RawGameTime;
                this.hookUpdateHandler.IsEnabled = true;
            }
            else
            {
                this.hookUpdateHandler.IsEnabled = false;
            }
        }

        private void HookHitCheck()
        {
            // cancel hook if it wont hit target anymore

            if (this.target == null || !this.target.IsValid || !this.target.IsVisible)
            {
                return;
            }

            var input = this.hook.GetPredictionInput(this.target);
            input.Delay = Math.Max((this.hookStartCastTime - Game.RawGameTime) + this.hook.CastPoint, 0);
            var output = this.hook.GetPredictionOutput(input);

            if (this.hookCastPosition.Distance2D(output.UnitPosition) > this.hook.Radius)
            {
                this.Owner.Stop();
                this.Cancel();
                this.hookUpdateHandler.IsEnabled = false;
            }
        }

        private void MenuKeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (!this.MenuKey && !this.ult.IsChanneling)
            {
                this.rot.Enabled = false;
            }
        }

        private bool ShouldCastHook(PredictionOutput output)
        {
            if (output.HitChance == HitChance.OutOfRange || output.HitChance == HitChance.Impossible)
            {
                return false;
            }

            if (output.HitChance == HitChance.Collision)
            {
                return false;
            }

            return true;
        }
    }
}