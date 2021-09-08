using NativeEngine;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace SprayTime
{
	[Library( "decal" )]
	public partial class SprayDecalDefinition : Asset
	{

		public static Dictionary<string, SprayDecalDefinition> ByPath = new();

		public class DecalEntry
		{
			public Material Material { get; set; }
			public RangedFloat Width { get; set; } = new RangedFloat( 5 );
			public RangedFloat Height { get; set; } = new RangedFloat( 5 );
			public bool KeepAspect { get; set; }
			public RangedFloat Depth { get; set; } = new RangedFloat( 3 );
			public RangedFloat Rotation { get; set; } = new RangedFloat( 0, 360 );
		}

		public DecalEntry[] Decals { get; set; }

		protected override void PostLoad()
		{
			ByPath[Path] = this;
		}

		public void PlaceUsingTrace( TraceResult tr )
		{
			var entry = Rand.FromArray( Decals );
			if ( entry == null )
				return;

			var w = entry.Width.GetValue();
			var h = entry.Height.GetValue();
			var d = entry.Depth.GetValue();
			var r = entry.Rotation.GetValue();

			if ( entry.KeepAspect )
			{
				h = w * (entry.Width.x / entry.Height.x);
			}

			var rot = Rotation.LookAt( tr.Normal ) * Rotation.FromAxis( Vector3.Forward, r );

			var pos = tr.EndPos;

			if ( tr.Entity is ModelEntity me && !me.IsWorld )
			{
				var tx = me.GetBoneTransform( tr.Bone );
				pos = tx.PointToLocal( pos );
				rot = tx.RotationToLocal( rot );
			}

			Place( entry.Material, tr.Entity, tr.Bone, pos, rot, new Vector3( w, h, d ) );
		}

		public void SprayPlaceUsingTrace( TraceResult tr, float scale, int color)
		{
			DecalEntry entry;

			if (color < 0 ){
				entry = Rand.FromArray( Decals );
			} else
			{
				entry = Decals[color];
			}
			
			if ( entry == null )
				return;

			var w = entry.Width.GetValue() * scale;
			var h = entry.Height.GetValue() * scale;
			var d = entry.Depth.GetValue();
			var r = entry.Rotation.GetValue();

			if ( entry.KeepAspect )
			{
				h = w * (entry.Width.x / entry.Height.x);
			}

			var rot = Rotation.LookAt( tr.Normal ) * Rotation.FromAxis( Vector3.Forward, r );

			var pos = tr.EndPos;

			if ( tr.Entity is ModelEntity me && !me.IsWorld )
			{
				var tx = me.GetBoneTransform( tr.Bone );
				pos = tx.PointToLocal( pos );
				rot = tx.RotationToLocal( rot );
			}

			Place( entry.Material, tr.Entity, tr.Bone, pos, rot, new Vector3( w, h, d ) );
		}



		[ClientRpc]
		public static void Place( Material material, Entity ent, int bone, Vector3 localpos, Rotation localrot, Vector3 scale )
		{
			if ( ent is ModelEntity me )
			{
				var tx = me.GetBoneTransform( bone );
				var pos = tx.PointToWorld( localpos );
				var rot = tx.RotationToWorld( localrot );

				Sandbox.Decals.Place( material, ent, bone, pos, scale, rot );
			}
			else
			{
				if ( !ent.IsValid() )
				{
					ent = PhysicsWorld.WorldBody?.Entity;
				}

				if ( ent.IsValid() )
				{
					Sandbox.Decals.Place( material, ent, bone, localpos, scale, localrot );
				}
			}
		}
	}
}
