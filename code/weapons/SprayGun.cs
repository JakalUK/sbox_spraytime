using Sandbox;
using Sandbox.UI;
using SprayTime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Library( "weapon_pistol", Title = "Pistol", Spawnable = true )]
partial class SprayGun : SprayBaseWeapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 100.0f;
	public override float SecondaryRate => 1.0f;

	public int layer = 0;
	
	public static Color[] colors = { Color.Black, Color.White, Color.Red, Color.Green, Color.Blue, Color.Orange };
	public int numOfColors = colors.Length;
	public int selectedColor = 0;
	public Color activeColor = Color.Black;

	public float selectedSize = 1;
	public float selectedSizeExp = 1;

	public static string sizeFormat = "0.0";
	public string sizeString = 1.0f.ToString(sizeFormat);

	public float[] sizeRange = { 0.30f, 8.5f };

	public int selectedStyle = 0;
	public string[] styles = { "Soft Round", "Hard Round", "Square" };
	public int numOfStyles = 3;

	public Sound spraysound;

	public List<SprayPanel> allSprays = new List<SprayPanel>();


	public string[] decals = { "decals/multisplat1.decal",
							   "decals/multismooth1.decal",
							   "decals/multisquare1.decal"};

	public TimeSince TimeSinceDischarge { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "weapons/rust_pistol/rust_pistol.vmdl" );
	}

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		(Owner as AnimEntity)?.SetAnimBool( "b_attack", true );

		//ShootEffects();
		//PlaySound( "rust_pistol.shoot" );
		//Spray( Owner.EyePos, Owner.EyeRot.Forward, 0.05f, 3.0f);
		Spray( Owner.EyePos, Owner.EyeRot.Forward, 0.05f, 3.0f );

	}


	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;
		CrosshairPanel = new SprayCrosshair();
		CrosshairPanel.Parent = Local.Hud;
		CrosshairPanel.AddClass("SprayCrosshair");
	}

	[ClientRpc]
	public virtual void Spray( Vector3 pos, Vector3 dir, float spread, float bulletSize )
	{
		var forward = dir;
		forward = forward.Normal;
		var tr = TraceSpray( pos, pos + forward * 5000, bulletSize );
		if (tr.Entity.IsValid() )
		{

			var decalPath = decals[selectedStyle];
			if ( decalPath != null )
			{
				if ( SprayDecalDefinition.ByPath.TryGetValue( decalPath, out var decal ) )
				{
					Rotation rot = Rotation.LookAt( tr.Normal ) * Rotation.FromAxis( Vector3.Forward, 0);
					Transform t = new Transform( tr.EndPos, rot);
					allSprays.Add(new SprayPanel( t, activeColor, selectedStyle, selectedSizeExp ));
				}
			}
		}

	}

	[Property]
	public string CurrentStyle
	{
		get => styles[selectedStyle];
	}


	public override bool CanReload()
	{
		return false;
	}


	public override bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;
		return true;
	}
	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		if ( Input.MouseWheel != 0 )
		{
			float newSize = selectedSize + Input.MouseWheel / 3.0f;

			if ( newSize < sizeRange[1] & newSize > sizeRange[0] )
			{
				selectedSize = newSize;
				selectedSizeExp = (float)Math.Pow( newSize, 1.5f );
				sizeString = selectedSizeExp.ToString( sizeFormat );
			}
			else if ( newSize <= sizeRange[0] ){sizeString = "Min";}
			else { sizeString = "Max";}
		}


		if (Input.Pressed( InputButton.Attack1 ) )
		{
			spraysound = PlaySound( "spray.loop" );
			
		}

		if ( Input.Released( InputButton.Attack1 ) )
		{
			spraysound.Stop();
		}

		if ( Input.Pressed( InputButton.Flashlight)){
			selectedStyle = (selectedStyle+1) % numOfStyles;
		}

		if ( Input.Pressed( InputButton.Reload) )
		{
			ClearSprays();
		}
	}

	public void ClearSprays()
		{
			 foreach ( SprayPanel spray in allSprays)
			 {
				 spray.Delete();
			 }
			 allSprays.Clear();
		}

	private void Discharge()
	{
		if ( TimeSinceDischarge < 0.5f )
			return;

		TimeSinceDischarge = 0;

		var muzzle = GetAttachment( "muzzle" ) ?? default;
		var pos = muzzle.Position;
		var rot = muzzle.Rotation;

		ShootEffects();
		PlaySound( "rust_pistol.shoot" );
		ShootBullet( pos, rot.Forward, 0.05f, 1.5f, 9.0f, 3.0f );

		ApplyAbsoluteImpulse( rot.Backward * 200.0f );
	}

	protected override void OnPhysicsCollision( CollisionEventData eventData )
	{
		if ( eventData.Speed > 500.0f )
		{
			Discharge();
		}
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 );
		anim.SetParam( "aimat_weight", 1.0f );
		anim.SetParam( "holdtype_handedness", 0 );
	}


	[ClientRpc]
	public virtual void SprayDecal( Vector3 pos, Vector3 dir, float spread, float bulletSize)
	{
		var forward = dir;
		forward = forward.Normal;
		var tr = TraceSpray( pos, pos + forward * 5000, bulletSize );

		if (IsServer & tr.Entity.IsValid()){

			var decalPath = decals[selectedStyle];
			if ( decalPath != null )
			{
				if (SprayDecalDefinition.ByPath.TryGetValue( decalPath, out var decal) )
				{
					//decal.SprayPlaceUsingTrace( tr, selectedSizeExp, selectedColor);
					Log.Info( "try" );
					var t = new Sandbox.SceneObject(null, Transform);
					//var f = new Spraytime.Spray();

				}
			}
		}

	}

	public virtual TraceResult TraceSpray( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();
		return tr;
	}


}


