// <copyright file="SimpleMenu.cs" company="Ensage">
//    Copyright (c) 2018 Ensage.
// </copyright>

namespace SimpleSDKMenu
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows.Input;

    using Ensage;
    using Ensage.SDK.Input;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Menu.Attributes;
    using Ensage.SDK.Menu.Items;
    using Ensage.SDK.Menu.ValueBinding;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using NLog;

    // Our submenu class

    // Every menu must have the MenuAttribute
    [Menu("My Menu Name")]
    [ExportPlugin("Simple Menu")]
    public class SimpleMenu : Plugin
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private readonly IServiceContext context;

        [ImportingConstructor]
        public SimpleMenu([Import] IServiceContext context)
        {
            this.context = context;
        }

        // PermaShow items will be displayed always, even if the menu is currently not visible
        [PermaShow]
        // Name of our Item will be "Bool Switch"
        [Item("Bool Switch")]
        // Tooltip when the user hovers over your item
        [Tooltip("Some Tooltip")]
        // DefaultValueAttribute is supported too
        [DefaultValue(true)]
        public bool Switch { get; set; }

        /// If you provide no name, the property name will be taken, in this case <see cref="DisableSwitch"/>
        [Item]
        public bool DisableSwitch
        {
            // You can directly execute your code in the getter and setter
            get
            {
                return this.Switch;
            }

            set
            {
                Log.Info($"Disable Switch is set to {value}");

                this.Switch = false;
            }
        }

        // This will be a submenu since the class is declared with the MenuAttribute
        [Menu]
        public SubMenu OurSubMenu { get; set; }

        // Every public property with the ItemAttribute will be automatically added to your menu class
        [Item("Some string")]
        public string SomeString { get; set; } = " is displayed here";

        // Different hotkeys with different flags
        [Item]
        public HotkeySelector HotkeyPress { get; set; } = new HotkeySelector(Key.B, TestPress, HotkeyFlags.Press);

        [Item]
        public HotkeySelector HotkeyDown { get; set; } = new HotkeySelector(Key.V, TestPress, HotkeyFlags.Down);

        [Item]
        public HotkeySelector HotkeyUp { get; set; } = new HotkeySelector(Key.C, TestPress, HotkeyFlags.Up);

        [Item]
        public HotkeySelector HotkeyDown2 { get; set; } = new HotkeySelector(MouseButtons.Left, TestPress, HotkeyFlags.Down);

        [Item]
        public HotkeySelector HotkeyUp2 { get; set; } = new HotkeySelector(MouseButtons.Left, TestPress, HotkeyFlags.Up);

        protected override void OnActivate()
        {
            // Make sure that all menus and items are actually initialized
            this.OurSubMenu = new SubMenu(this.context.Renderer);

            // We register the object of a menu class
            // In this case we register ourselves
            this.context.MenuManager.RegisterMenu(this);

            // Subscribe to some events of complex menu items
            this.OurSubMenu.SomeSlider.ValueChanging += this.SomeSlider_ValueChanging;
            this.OurSubMenu.OurSelection.ValueChanging += this.OurSelection_ValueChanging;
        }

        protected override void OnDeactivate()
        {
            // Cleanup all events we subscribed to
            this.OurSubMenu.OurSelection.ValueChanging -= this.OurSelection_ValueChanging;
            this.OurSubMenu.SomeSlider.ValueChanging -= this.SomeSlider_ValueChanging;

            // Deregister our menu from the manager
            this.context.MenuManager.DeregisterMenu(this);
        }

        private static void TestPress(MenuInputEventArgs args)
        {
            Log.Info($"A hotkey has been pressed: {args.Key} | {args.MouseButton} > {args.Flag}");

            // You can test for flags:
            if ((args.MouseButton & MouseButtons.Left) == MouseButtons.Left)
            {
                Log.Info($"The left mouse button was either pressed or released!");
            }
        }

        private void OurSelection_ValueChanging(object sender, ValueChangingEventArgs<AbilityId> e)
        {
            Log.Info($"User selected {e.Value} in our selection");
        }

        private void SomeSlider_ValueChanging(object sender, ValueChangingEventArgs<int> e)
        {
            Log.Info($"user wants to change the slider from {e.OldValue} to {e.Value}");

            // You can block these events to keep the old value
            if (e.Value < e.OldValue)
            {
                e.Process = false;
                Log.Error($"Sorry, only increasing is allowed here!");
            }
        }
    }
}