using Sandbox.UI;
using SprayTime.UI;

namespace SprayTime
{

	public partial class SprayHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public SprayHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/sprayhud.html" );
				RootPanel.AddChild<Crosshair>();
				RootPanel.StyleSheet.Load( "/code/ui/stylesheet.scss" );
			}
		}
	}

}
