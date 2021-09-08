using Sandbox;
using SprayTime;

[Library( "weapon_pistol", Title = "Pistol", Spawnable = true )]
partial class SprayGun : SprayBaseWeapon
{
	public override string ViewModelPath => "weapons/rust_pistol/v_rust_pistol.vmdl";

	public override float PrimaryRate => 100.0f;
	public override float SecondaryRate => 1.0f;

	public int selectedColor = 0;
	public int selectedSize = 1;
	public int maxSize = 20;

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
		Spray( Owner.EyePos, Owner.EyeRot.Forward, 0.05f, 3.0f);
	}

	public override bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;
		return true;
	}
	public override void Simulate( Client owner )
	{
		base.Simulate( owner );
		if ( Input.Pressed( InputButton.Slot1)){
			selectedColor = 0;
		} else if ( Input.Pressed( InputButton.Slot2)){
			selectedColor = 1;
		} else if ( Input.Pressed( InputButton.Slot3)){
			selectedColor = 2;
		}else if ( Input.Pressed( InputButton.Slot4)){
			selectedColor = 3;
		}else if ( Input.Pressed( InputButton.Slot5)){
			selectedColor = 4;
		}else if ( Input.Pressed( InputButton.Slot6)){
			selectedColor = -1;
		}

		if (selectedSize + Input.MouseWheel > 0 & selectedSize + Input.MouseWheel < maxSize){
			selectedSize += Input.MouseWheel;
		}
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

	public virtual void Spray( Vector3 pos, Vector3 dir, float spread, float bulletSize)
	{
		var forward = dir;
		forward = forward.Normal;
		var tr = TraceSpray( pos, pos + forward * 5000, bulletSize );

		if (IsServer & tr.Entity.IsValid()){
			string[] decals = { "decals/multisplat1.decal" };
			var decalPath = Rand.FromArray(decals);
			if ( decalPath != null )
			{
				if (SprayDecalDefinition.ByPath.TryGetValue( decalPath, out var decal) )
				{
					decal.SprayPlaceUsingTrace( tr, selectedSize, selectedColor);
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


