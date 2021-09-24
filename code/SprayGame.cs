
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Threading.Tasks;
using SprayTime.UI;


namespace SprayTime
{
	public partial class SprayGame : Sandbox.Game
	{
		public SprayGame()
		{

			Global.TickRate = 100;

			if ( IsServer )
			{
				Log.Info( "SprayTime Was Created Serverside!" );
				new SprayHud();
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

		public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
		{

			var clgun = cl.Pawn.Inventory.Active as SprayGun;
			clgun.ClearSprays();

			base.ClientDisconnect(cl, reason);

		}
	}

}
