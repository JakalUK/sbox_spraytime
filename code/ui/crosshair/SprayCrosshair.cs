
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

public class SprayCrosshair : Panel
{
	private Entity player;
	private SprayGun gun;
	public SprayCrosshair()
	{
		StyleSheet.Load( "/ui/crosshair/SprayCrosshair.scss" );
		player = Local.Pawn;
	}

	float scale = 5;

	public override void Tick()
	{
		base.Tick();
		gun = player.Inventory.Active as SprayGun;
		scale = gun.selectedSizeExp;
		Style.Width = 10 * scale;
		Style.Height = 10 * scale;
		Style.MarginLeft = -5 * scale;
		Style.MarginTop = -5 * scale;
		Style.Dirty();
		//this.PositionAtCrosshair();
		//scale = scale.LerpTo( 1, Time.Delta * 5 );

	}

}
