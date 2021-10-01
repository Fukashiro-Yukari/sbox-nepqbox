namespace Sandbox.CWEP
{
	[Library( "cwep_spawncar", Title = "Spawn Car", Description = "Flying car" )]
	public partial class SpawnCar : CWEPFireMode
	{
		public override bool PrimaryAutomatic => false;
		public override bool SecondaryAutomatic => false;
		public override bool SecondaryCanUse => true;
		public override CType Crosshair => CType.Pistol;

		[Net,Predicted]
		private CarEntity LastCar { get; set; }

		private void Spawn( bool super = false )
		{
			if ( Parent.IsServer )
			{
				using ( Prediction.Off() )
				{
					LastCar = new CarEntity();
					LastCar.Position = Owner.EyePos + Owner.EyeRot.Forward * 80;
					LastCar.Rotation = Owner.EyeRot;
					LastCar.Owner = Owner;
					LastCar.Velocity = Owner.EyeRot.Forward * (super ? 50000 : 1000);
					LastCar.DeleteAsync( 5 );
				}

				// BUG: Unable to use weapons
				//if ( LastCar.IsValid() && super )
				//{
				//	LastCar.OnUse( Owner );
				//	SpawnCarRpc.FUCKRPC( Owner, LastCar );
				//}
			}
		}

		public override void AttackPrimary()
		{
			Spawn();
		}

		public override void AttackSecondary()
		{
			Spawn( true );
		}
	}

	public partial class SpawnCarRpc
	{
		[ClientRpc]
		public static void FUCKRPC( Entity Owner, CarEntity LastCar )
		{
			if ( LastCar.IsValid() )
			{
				LastCar.OnUse( Owner );
			}
		}
	}
}
