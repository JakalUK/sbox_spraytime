using Sandbox;
using System;
using System.Linq;

namespace SprayTime { 

	partial class SprayPlayer : Player
	{

	public SprayPlayer()
		{
		Inventory = new SprayInventory( this );
		}


	public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();

			Animator = new StandardPlayerAnimator();

			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;


			Inventory.Add( new SprayGun(), true );


			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			SimulateActiveChild( cl, ActiveChild );

			if ( IsServer && Input.Pressed( InputButton.Attack2 ) )
			{
				//SpawnAttack1();
			}

		}

		public void SpawnAttack1()
		{
			var ragdoll = new ModelEntity();
			ragdoll.SetModel( "models/citizen/citizen.vmdl_c" );
			ragdoll.Position = EyePos + EyeRot.Forward * 40;
			ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
		}

		public override void OnKilled()
		{
			base.OnKilled();

			EnableDrawing = false;
		}
	}
}
