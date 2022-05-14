namespace Sandbox.CWEP
{
	[Library( "cwep_spawnbarrel", Title = "Spawn Barrel", Description = "Big explosion!!" )]
	public partial class SpawnBarrel : CWEPFireMode
	{
		public override bool SecondaryCanUse => true;
		public override CType Crosshair => CType.ShotGun;

		private void Spawn( int amount = 1 )
		{
			if ( Parent.IsServer )
				using ( Prediction.Off() )
				{
					for ( int i = 0; i < amount; i++ )
					{
						var ent = new Prop();
						ent.SetModel( "models/rust_props/barrels/fuel_barrel" );

						var rand = amount != 1 ? amount * 5 : amount;

						ent.Position = Owner.EyePosition + Owner.EyeRotation.Forward * 80 + Owner.EyeRotation.Left * Rand.Int( -rand, rand ) + Owner.EyeRotation.Down * Rand.Int( -rand, rand );
						ent.Rotation = Owner.EyeRotation;
						ent.Owner = Owner;
						ent.Velocity = Owner.EyeRotation.Forward * 50000;
					}
				}
		}

		public override void AttackPrimary()
		{
			Spawn();
		}

		public override void AttackSecondary()
		{
			Spawn( 5 );
		}
	}
}
