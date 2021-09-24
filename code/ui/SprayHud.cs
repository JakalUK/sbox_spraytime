using Sandbox.UI;

namespace SprayTime.UI
{

	public partial class SprayHud : Sandbox.HudEntity<RootPanel>
	{
		public SprayHud()
		{
			if ( !IsClient )
				return;

			RootPanel.SetTemplate( "/ui/sprayhud.html" );

			RootPanel.AddChild<ControlsMenu>();
			RootPanel.AddChild<SpraySettings>();
			
		}
				
	}


}
