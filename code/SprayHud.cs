using Sandbox.UI;


namespace SprayTime
{

	public partial class SprayHudEntity : Sandbox.HudEntity<RootPanel>
	{
		public SprayHudEntity()
		{
			if ( IsClient )
			{
				RootPanel.SetTemplate( "/sprayhud.html" );
			}
		}
	}

}
