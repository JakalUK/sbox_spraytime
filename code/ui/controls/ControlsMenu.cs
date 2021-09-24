using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace SprayTime
{
	class ControlsMenu : Panel
	{

		public static ControlsMenu Instance;
		public Panel controlsmenu;
		public Panel controls;
		public Label tabtext;
		public bool isHidden = false;
		TimeSince timeSinceToggled;

		public ControlsMenu()
		{
			StyleSheet.Load( "/ui/controls/ControlsMenu.scss");
			var container = Add.Panel( "controls-container" );
			controlsmenu = container.Add.Panel( "controls-menu" );
			controls = controlsmenu.Add.Panel("controls");
			controls.SetTemplate( "/ui/controls/ControlsMenu.html" );
			var tab = container.Add.Panel( "controls-tab" );
			tabtext = tab.Add.Label( "↓         Controls (tab)         ↓", "tab-text");
		}

		public override void Tick()
		{
			base.Tick();


			bool toggle = Input.Pressed( InputButton.Score );


			if (timeSinceToggled > 0.1f && toggle )
			{
				isHidden = !isHidden;

				if ( !isHidden ){
					tabtext.Text = "↓         Controls (tab)         ↓";
				} 
				else{
					tabtext.Text = "↑         Hide (tab)         ↑";
				}
				controlsmenu.SetClass( "active", isHidden );
				controls.SetClass( "active", isHidden );
				timeSinceToggled = 0;
			}
			
			

		}

	}
}
