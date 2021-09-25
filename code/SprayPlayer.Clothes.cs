using Sandbox;
using System;
using System.Linq;


namespace SprayTime
{
	public class ClothingEntity : ModelEntity
	{

	}

	partial class SprayPlayer
	{
		ModelEntity jacket;
		ModelEntity hat;
		ModelEntity trousers;

		bool dressed = false;

		public void Dress()
		{
			if ( dressed ) return;
			dressed = true;

			jacket = new ClothingEntity();
			jacket.SetModel( "models/citizen_clothes/jacket/labcoat.vmdl" );
			jacket.SetParent( this, true );
			jacket.EnableShadowInFirstPerson = true;
			jacket.EnableHideInFirstPerson = true;

			hat = new ClothingEntity();
			hat.SetModel( "models/citizen_clothes/hat/hat_beret.black.vmdl" );
			hat.SetParent( this, true );
			hat.EnableShadowInFirstPerson = true;
			hat.EnableHideInFirstPerson = true;

			trousers = new ClothingEntity();
			trousers.SetModel( "models/citizen_clothes/trousers/trousers.police.vmdl" );
			trousers.SetParent( this, true );
			trousers.EnableShadowInFirstPerson = true;
			trousers.EnableHideInFirstPerson = true;

		}
	}
}
