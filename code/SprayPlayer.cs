using Sandbox;
using Sandbox.UI;
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

			Dress();

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

			if ( Input.Pressed( InputButton.View ) )
			{
				if ( Camera is ThirdPersonCamera )
				{
					Camera = new FirstPersonCamera();
				}
				else
				{
					Camera = new ThirdPersonCamera();
				}
			}


		}


		public void SpawnAttack1()
		{
			var ragdoll = new ModelEntity();
			ragdoll.SetModel( "models/ball/ball.vmdl_c" );
			//ragdoll.SetModel( "models/citizen/citizen.vmdl_c" );
			ragdoll.Position = EyePos + EyeRot.Forward * 40;
			ragdoll.Rotation = Rotation.LookAt( Vector3.Random.Normal );
			ragdoll.SetupPhysicsFromModel( PhysicsMotionType.Dynamic, false );
			ragdoll.PhysicsGroup.Velocity = EyeRot.Forward * 1000;
			ragdoll.SetMaterialGroup(3);
		}

		public override void OnKilled()
		{
			base.OnKilled();
			EnableDrawing = false;
		}
	}
}
