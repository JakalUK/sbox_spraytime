
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;


namespace SprayTime
{
	public partial class SprayGame : Sandbox.Game
	{
		public SprayGame()
		{

			Global.TickRate = 140;

			if ( IsServer )
			{
				Log.Info( "SprayTime Was Created Serverside!" );
				new SprayHudEntity();
			}

			if ( IsClient )
			{
				Log.Info( "SprayTime Was Created Clientside!" );
			}
		}

		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new SprayPlayer();
			client.Pawn = player;

			player.Respawn();
		}
	}

}
