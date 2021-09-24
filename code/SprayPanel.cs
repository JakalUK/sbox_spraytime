using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SprayTime
{
	class SprayPanel : WorldPanel
	{

		public SprayPanel( Transform t, Color color, int style, float scale)
		{
			Transform = t;
			
			PanelBounds = new Rect( -125*scale, -125*scale, 250*scale, 250*scale);
			StyleSheet.Load( "SprayPanel.scss" );
			SetSpray(style, color);
			MaxInteractionDistance = 0.0f;
		}

		public void SetSpray(int style, Color color)
		{
			Color32 c = color.ToColor32();
			string alpha = color.a.ToString();
			string rgb = c.R + "," + c.G + "," + c.B;

			switch ( style )
			{
				case 0:
					string gradient = "radial-gradient(rgba(" + rgb + "," + 0.5 + ") 0%, rgba(" + rgb + "," + 0.3 + ") 15%, rgba(" + rgb + "," + 0 + ") 50%)";
					Style.Set( "background", gradient );
					Style.Set( "opacity", alpha);
					break;
				case 1:
					Style.Set( "border-radius", "50%" );
					Style.Set( "background-color", "rgba(" + rgb + ", " + alpha + ")" );
					break;
				case 2:
					Style.Set( "background-color", "rgba(" + rgb + ", " + alpha + ")" );
					break;
			}
			

		}
		
	}
}

