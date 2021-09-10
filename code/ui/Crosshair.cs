using Sandbox;
using Sandbox.UI;
using System;

namespace SprayTime.UI
{
	public partial class Crosshair : Panel
	{

		public static Crosshair Instance { get; private set; }
		public float Radius = 0;
		public Color Color = Color.White;
		public Crosshair()
		{
			if ( Instance == null )
			{
				Instance = this;
			}
			Style.Opacity = 0.5f;
			Style.Dirty();

		}

		public override void Tick()
		{
			if ( ComputedStyle == null ) return;
			Style.Width = Radius * 50;
			Style.Height = Radius * 50;
			Style.MarginLeft = -(ComputedStyle.Width.Value.Value / 2);
			Style.MarginTop = -(ComputedStyle.Width.Value.Value / 2);
			Style.BackgroundColor = Color;

			Style.Dirty();
			base.Tick();
		}



	}
}
