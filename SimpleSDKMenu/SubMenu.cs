// <copyright file="SubMenu.cs" company="Ensage">
//    Copyright (c) 2018 Ensage.
// </copyright>

namespace SimpleSDKMenu
{
    using Ensage;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Menu.Items;
    using Ensage.SDK.Renderer;

    public class SubMenu
    {
        public SubMenu(IRendererManager renderer)
        {
            // we need to make sure that the textures we are using are actually loaded
            // you can use any method from the texture manager and could also load it in your main class before
            renderer.TextureManager.LoadFromDota("item_black_king_bar", @"resource\flash3\images\items\black_king_bar.png");
            renderer.TextureManager.LoadFromDota("item_blade_mail", @"resource\flash3\images\items\blade_mail.png");

            // use the texture key you were loading before
            this.Toggler = new ImageToggler(true, "item_black_king_bar", "item_blade_mail");

            // if no default value is given, when you can't disable items in the priority changer but only change the order
            this.PriorityChanger = new PriorityChanger("item_black_king_bar", "item_blade_mail");
        }

        // You can basically make a selection of any datatype here. ToString() will be called to display the selected item
        [Item("Invoker Orb Selection")]
        public Selection<AbilityId> OurSelection { get; set; } = new Selection<AbilityId>(0, AbilityId.invoker_quas, AbilityId.invoker_wex, AbilityId.invoker_exort);

        [Item("Test PriorityChanger")]
        public PriorityChanger PriorityChanger { get; set; }

        [Item("Some Percentage Slider")]
        public Slider SomeSlider { get; set; } = new Slider(50, 0, 100);

        [Item("Test Toggler")]
        public ImageToggler Toggler { get; set; }
    }
}