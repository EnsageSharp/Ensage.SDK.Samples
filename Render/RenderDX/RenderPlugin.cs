﻿// <copyright file="RenderPlugin.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace RenderDX
{
    using System;
    using System.ComponentModel.Composition;
    using System.Reflection;

    using Ensage.SDK.Renderer;
    using Ensage.SDK.Service;
    using Ensage.SDK.Service.Metadata;

    using log4net;

    using PlaySharp.Toolkit.Logging;

    using SharpDX;

    using Color = System.Drawing.Color;

    [ExportPlugin("RenderDX Sample")]
    internal class RenderPlugin : Plugin
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private readonly IRendererManager renderer;

        [ImportingConstructor]
        public RenderPlugin([Import] IServiceContext context)
        {
            this.renderer = context.Renderer;
        }

        protected override void OnActivate()
        {
            this.renderer.Draw += this.OnDraw;
        }

        protected override void OnDeactivate()
        {
            this.renderer.Draw -= this.OnDraw;
        }

        private void OnDraw(object sender, EventArgs eventArgs)
        {
            this.renderer.DrawLine(Vector2.Zero, new Vector2(100, 100), Color.Red);
            this.renderer.DrawRectangle(new RectangleF(150, 150, 250, 250), Color.Red);
            this.renderer.DrawText(new Vector2(400, 400), "HELLO WORLD", Color.Red);
            this.renderer.DrawCircle(new Vector2(600, 600), 100, Color.Red);
        }
    }
}